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
                writer.WriteVector2(msg.inputs);
                writer.WriteUInt32(msg.isAck);
                writer.WriteInt32(msg.inputMessageID);
                writer.WriteUInt32(msg.isInput);
                CustomMessagingManager.SendNamedMessage("Replication", null, stream, "customChannel");
            }
        }
    }

// client message to server
    public void SendClientInputReplicationMessage(ReplicationMessage msg)
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
                writer.WriteVector2(msg.inputs);
                writer.WriteUInt32(msg.isAck);
                writer.WriteInt32(msg.inputMessageID);
                writer.WriteUInt32(msg.isInput);
                CustomMessagingManager.SendNamedMessage("Replication", this.ServerClientId, stream, "customChannel");
            }
        }
    }

// client receives server msg
    private void HandleReplicationMessage(ulong clientId, Stream stream)
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
            replicationMessage.inputs = reader.ReadVector2();
            replicationMessage.isAck = reader.ReadUInt32();
            replicationMessage.inputMessageID = reader.ReadInt32();
            replicationMessage.isInput = reader.ReadUInt32();

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
            
            if (replicationMessage.isAck == 1) // client receives ack and adds component to deal with
            {
                Debug.Log("received ack msg from server");
                ServerAcknowledgeMessage acknowledgeMessage = new ServerAcknowledgeMessage();
                acknowledgeMessage.inputMessageID = replicationMessage.inputMessageID;
                acknowledgeMessage.confirmedPosition = replicationMessage.pos;
                acknowledgeMessage.clientTime = replicationMessage.timeCreated; // double check time is ok here
                ComponentsManager.Instance.SetComponent<ServerAcknowledgeMessage>(replicationMessage.entityId, acknowledgeMessage);
            }
        }
    }

// server receives client msg
    private void HandleClientInputReplicationMessage(ulong clientId, Stream stream)
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
            replicationMessage.inputs = reader.ReadVector2();
            replicationMessage.isAck = reader.ReadUInt32();
            replicationMessage.inputMessageID = reader.ReadInt32();
            replicationMessage.isInput = reader.ReadUInt32(); // todo change to bool

            if (replicationMessage.isInput == 1) 
            {
                InputMessage inputMessage = new InputMessage();
                inputMessage.inputMessageID = replicationMessage.inputMessageID;
                inputMessage.message = replicationMessage;
                inputMessage.handled = false;
                inputMessage.inputs = replicationMessage.inputs;
                inputMessage.clientTime = replicationMessage.timeCreated;
                ComponentsManager.Instance.SetComponent<InputMessage>(replicationMessage.entityId, inputMessage);
            }
            else // TODO ?
            {
                ComponentsManager.Instance.SetComponent<ReplicationMessage>(replicationMessage.entityId, replicationMessage);
            }
        }
    }

    public void RegisterClientNetworkHandlers()
    {
        CustomMessagingManager.RegisterNamedMessageHandler("Replication", HandleReplicationMessage);
    }

    public void RegisterServerNetworkHandlers()
    {
        // TODO
        CustomMessagingManager.RegisterNamedMessageHandler("Replication", HandleClientInputReplicationMessage);
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
