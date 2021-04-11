using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : ISystem
{
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
                    if (inputMessage.message.inputKeyW == 1)
                    {
                        shape.speed.y = 5;
                    }

                    if (inputMessage.message.inputKeyA == 1)
                    {
                        shape.speed.x = -5;
                    }

                    if (inputMessage.message.inputKeyS == 1)
                    {
                        shape.speed.y = -5;
                    }

                    if (inputMessage.message.inputKeyD == 1)
                    {
                        shape.speed.x = 5;
                    }

                    inputMessage.handled = true;
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(inputMessage.message.entityId, shape);
                }
            }
        });
    }

    public void UpdateSystemClient()
    {
        ReplicationMessage message = new ReplicationMessage();

        message.timeCreated = Utils.SystemTime;
        message.entityId = (uint)ECSManager.Instance.NetworkManager.LocalClientId;
        message.inputKeyW = 0;
        message.inputKeyA = 0;
        message.inputKeyS = 0;
        message.inputKeyD = 0;
        
        bool inputDetected = false;
        // TODO fix code dup 
        if (Input.GetKey(KeyCode.W))
        {
            message.inputKeyW = 1;
            inputDetected = true;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            message.inputKeyA = 1;
            inputDetected = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            message.inputKeyS = 1;
            inputDetected = true;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            message.inputKeyD = 1;
            inputDetected = true;
        }

        if (inputDetected)
        {
            ECSManager.Instance.NetworkManager.SendClientInputReplicationMessage(message);
        }
    }
}
