using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;

public class CustomNetworkManager : NetworkingManager
{
    public void Awake()
    {
        OnClientConnectedCallback += OnClientConnected;
        OnServerStarted += OnStartServer;
    }
    
    public void OnClientConnected(ulong clientId)
    {
        if (isServer)
        {
            bool spawnFound = ComponentsManager.Instance.TryGetComponent(new EntityComponent(0), out SpawnInfo spawnInfo);

            if (!spawnFound)
            {
                spawnInfo = new SpawnInfo(false);
            }
            spawnInfo.playersToSpawn.Add((uint)clientId);
            ComponentsManager.Instance.SetComponent<SpawnInfo>(new EntityComponent(0), spawnInfo);
        }
        else
        {
            RegisterClientNetworkHandlers();
        }
    }

    public void OnStartServer()
    {
        RegisterServerNetworkHandlers();
    }

// server sends to client
    public void SendReplicationMessage(ReplicationMessage msg)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(msg.messageID);
                writer.WriteInt32(msg.timeCreated);
                writer.WriteUInt32(msg.entityId);
                writer.WriteInt16((byte)msg.shape);
                writer.WriteVector2(msg.pos);
                writer.WriteVector2(msg.speed);
                writer.WriteDouble(msg.size);
                CustomMessagingManager.SendNamedMessage("Replication", null, stream, "customChannel");
            }
        }
    }

// client message to server
    public void SendClientInputMessage(InputMessage msg)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(msg.inputMessageID);
                writer.WriteBool(msg.handled);
                writer.WriteVector2(msg.inputs);
                writer.WriteInt32(msg.clientTime);
                writer.WriteUInt32(msg.entityID);

                CustomMessagingManager.SendNamedMessage("Input", this.ServerClientId, stream, "customChannel");
            }
        }
    }

    public void SendServerAcknowledgementMessage(ServerAcknowledgeMessage msg)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(msg.ackMessageID);
                writer.WriteVector2(msg.confirmedPosition);
                writer.WriteInt32(msg.clientTime);
                writer.WriteUInt32(msg.entityID);

                CustomMessagingManager.SendNamedMessage("Acknowledge", null, stream, "customChannel");
            }
        }
    }

// client receives server msg
    private void HandleServerReplicationMessage(ulong clientId, Stream stream)
    {
        ReplicationMessage replicationMessage = new ReplicationMessage();
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            replicationMessage.messageID = reader.ReadInt32();
            replicationMessage.timeCreated = reader.ReadInt32();
            replicationMessage.entityId = reader.ReadUInt32();
            replicationMessage.shape = (Config.Shape)reader.ReadInt16();
            replicationMessage.pos = reader.ReadVector2();
            replicationMessage.speed = reader.ReadVector2();
            replicationMessage.size = (float)reader.ReadDouble();

            ComponentsManager.Instance.SetComponent<ReplicationMessage>(replicationMessage.entityId, replicationMessage);

            if (!ComponentsManager.Instance.EntityContains<EntityComponent>(replicationMessage.entityId))
            {
                bool spawnFound = ComponentsManager.Instance.TryGetComponent(new EntityComponent(0), out SpawnInfo spawnInfo);

                if (!spawnFound)
                {
                    spawnInfo = new SpawnInfo(false);
                }
                spawnInfo.replicatedEntitiesToSpawn.Add(replicationMessage);
                ComponentsManager.Instance.SetComponent<SpawnInfo>(new EntityComponent(0), spawnInfo);
            }
        }
    }

    private void HandleClientInputMessage(ulong clientId, Stream stream)
    {
        InputMessage inputMessage = new InputMessage();
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            inputMessage.inputMessageID = reader.ReadInt32();
            inputMessage.handled = reader.ReadBool();
            inputMessage.inputs = reader.ReadVector2();
            inputMessage.clientTime = reader.ReadInt32();
            inputMessage.entityID = reader.ReadUInt32();
            
            ComponentsManager.Instance.SetComponent<InputMessage>(inputMessage.entityID, inputMessage);
        }
    }

    public void HandleServerAcknowledgementMessage(ulong clientId, Stream stream)
    {
        ServerAcknowledgeMessage ackMessage = new ServerAcknowledgeMessage();
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            ackMessage.ackMessageID = reader.ReadInt32();
            ackMessage.confirmedPosition = reader.ReadVector2();
            ackMessage.clientTime = reader.ReadInt32();
            ackMessage.entityID = reader.ReadUInt32();

            if (ackMessage.entityID == (uint)ECSManager.Instance.NetworkManager.LocalClientId)
            {
                // Only add the message to the concerned client's components list
                ComponentsManager.Instance.SetComponent<ServerAcknowledgeMessage>(ackMessage.entityID, ackMessage);
            }
        }
    }

    public void RegisterClientNetworkHandlers()
    {
        CustomMessagingManager.RegisterNamedMessageHandler("Replication", HandleServerReplicationMessage);
        CustomMessagingManager.RegisterNamedMessageHandler("Acknowledge", HandleServerAcknowledgementMessage);
    }

    public void RegisterServerNetworkHandlers()
    {
        CustomMessagingManager.RegisterNamedMessageHandler("Input", HandleClientInputMessage);
    }


    public new bool isServer { get { return GetConnectionStatus() == ConnectionStatus.isServer; } }
    public new bool isClient { get { return GetConnectionStatus() == ConnectionStatus.isClient; } }

    public enum ConnectionStatus
    {
        isClient,
        isServer,
        notConnected
    }

    public ConnectionStatus GetConnectionStatus()
    {
        if (IsConnectedClient)
        {
            return ConnectionStatus.isClient;
        }
        else if (IsServer && IsListening)
        {
            return ConnectionStatus.isServer;
        }
        else
        {
            return ConnectionStatus.notConnected;
        }
    }
}
