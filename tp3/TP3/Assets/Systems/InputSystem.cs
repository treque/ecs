using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : ISystem
{
    private const float _SPEED = 15.0f;
    
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
                    shape.speed = _SPEED * inputMessage.inputs;
                    inputMessage.handled = true;
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(inputMessage.message.entityId, shape);
                }
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
            ECSManager.Instance.NetworkManager.SendClientInputReplicationMessage(message);

            if (ComponentsManager.Instance.TryGetComponent<ShapeComponent>(clientId, out ShapeComponent shape))
            {
                ComponentsManager.Instance.InputPositionHistory.Enqueue(
                    new KeyValuePair<Vector2, Vector2>(
                        message.inputs,
                        PositionUpdateSystem.GetNewPosition(shape.pos, message.inputs * _SPEED)
                    )
                );
            }

            //ComponentsManager.Instance.SetComponent<ReplicationMessage>(clientId, message);
        }
    }
}
