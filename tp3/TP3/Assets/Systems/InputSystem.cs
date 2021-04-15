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
            if (ComponentsManager.Instance.TryGetComponent(entityID, out ShapeComponent shape))
            {
                shape.speed = Vector2.zero;
                if (!inputMessage.handled)
                {           
                    shape.speed = ECSManager.Instance.SPEED * inputMessage.inputs;
                    inputMessage.handled = true;
                    ComponentsManager.Instance.SetComponent<InputMessage>(entityID, inputMessage);
                    
                    // send acknowledgement to client
                    ServerAcknowledgeMessage ackMessage = new ServerAcknowledgeMessage();
                    ackMessage.clientTime = inputMessage.clientTime;
                    ackMessage.confirmedPosition = PositionUpdateSystem.GetNewPosition(shape.pos, shape.speed);
                    ackMessage.entityID = entityID;
                    ComponentsManager.Instance.SetComponent<ServerAcknowledgeMessage>(entityID, ackMessage);
                }

                ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shape);
            }
        });
    }

    public void UpdateSystemClient()
    {           
        InputMessage message = new InputMessage();
        uint clientId = (uint)ECSManager.Instance.NetworkManager.LocalClientId;
        message.clientTime = Utils.SystemTime;
        message.entityID = clientId;
        message.inputs = Vector2.zero;
        message.handled = false;
 
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
            if (ComponentsManager.Instance.TryGetComponent<ShapeComponent>(clientId, out ShapeComponent shape))
            {
                // storing the necessary information in an InputInfo for history
                InputInfo inputInfo = new InputInfo();
                inputInfo.input = message.inputs;
                inputInfo.clientTime = message.clientTime; 

                Vector2 newPos = PositionUpdateSystem.GetNewPosition(shape.pos, message.inputs * ECSManager.Instance.SPEED);
                inputInfo.pos = newPos;
                ComponentsManager.Instance.InputPositionHistoryList.Add(inputInfo);
                shape.pos = newPos;

                if (ECSManager.Instance.Config.enableInputPrediction)
                {
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(clientId, shape);
                }
            }

            ComponentsManager.Instance.SetComponent<InputMessage>(clientId, message);
        }
    }
}
