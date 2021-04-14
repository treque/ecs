using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : ISystem
{
    private const float _SPEED = 20.0f;
    
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.isServer)
        {
            UpdateSystemServer();
        }
        else if (ECSManager.Instance.NetworkManager.isClient)
        {
            UpdateSystemClient();
        }
    }

    public void UpdateSystemServer()
    {
        ComponentsManager.Instance.ForEach<InputMessage>((entityID, inputMessage) =>
        {
            if (ComponentsManager.Instance.TryGetComponent(inputMessage.message.entityId, out ShapeComponent shape))
            {
                shape.speed = Vector2.zero;
                if (!inputMessage.handled)
                {
                    Debug.Log("THERE IS AN INPUT");
                    shape.speed = _SPEED * inputMessage.inputs;
                    inputMessage.handled = true;
                    ComponentsManager.Instance.SetComponent<InputMessage>(inputMessage.message.entityId, inputMessage);
                    
                    // send acknowledgement to client
                    ReplicationMessage message = new ReplicationMessage();
                    message.isAck = 1;
                    message.timeCreated = inputMessage.clientTime;
                    message.inputMessageID = inputMessage.inputMessageID;
                    message.pos = PositionUpdateSystem.GetNewPosition(shape.pos, shape.speed);
                    //ComponentsManager.Instance.SetComponent<ReplicationMessage>(inputMessage.message.entityId, message);
                    ECSManager.Instance.NetworkManager.SendReplicationMessage(message);
                }

                ComponentsManager.Instance.SetComponent<ShapeComponent>(inputMessage.message.entityId, shape);
            }
        });
    }

    public void UpdateSystemClient()
    {        
        ReplicationMessage message = new ReplicationMessage();
        uint clientId = (uint)ECSManager.Instance.NetworkManager.LocalClientId;
        message.timeCreated = Utils.SystemTime;
        message.entityId = clientId;
        message.inputs = Vector2.zero;
        message.isInput = 1;

        // TODO fix code dup 
        if (Input.GetKey(KeyCode.W))
        {
            message.inputs.y = 1.0f;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            message.inputs.x = -1.0f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            message.inputs.y = -1.0f;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            message.inputs.x = 1.0f;
        }

        if (message.inputs != Vector2.zero)
        {
            // try setting replication component instead
            ComponentsManager.Instance.SetComponent<ReplicationMessage>(clientId, message);
            //ECSManager.Instance.NetworkManager.SendClientInputReplicationMessage(message);

            if (ComponentsManager.Instance.TryGetComponent<ShapeComponent>(clientId, out ShapeComponent shape))
            {
                // storing the necessary information in an InputMessage for history
                InputMessage inputMsg = new InputMessage();
                inputMsg.inputs = message.inputs;
                inputMsg.clientTime = message.timeCreated; 

                Vector2 newPos = PositionUpdateSystem.GetNewPosition(shape.pos, message.inputs * _SPEED);
                
                ComponentsManager.Instance.InputPositionHistoryList.Add(
                    new KeyValuePair<InputMessage, Vector2>(
                        inputMsg,
                        newPos
                    )
                );
                Debug.Log("added to history: time : " + inputMsg.clientTime + " pos: " + newPos);

                //todo: simuler cote client
                //shape.speed = _SPEED * message.inputs; // positioN??????
                shape.pos = newPos;
                // this calls it twice
                ComponentsManager.Instance.SetComponent<ShapeComponent>(clientId, shape);
            }
        }
    }
}
