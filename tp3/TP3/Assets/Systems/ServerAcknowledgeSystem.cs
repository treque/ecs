using System.Collections.Generic;
using UnityEngine;

public class ServerAcknowledgeSystem : ISystem
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
        if (ECSManager.Instance.NetworkManager.isClient && ECSManager.Instance.Config.enableInputPrediction)
        {
            UpdateSystemClient();
        }
    }
    public static void UpdateSystemClient()
    {
        ComponentsManager.Instance.ForEach<ServerAcknowledgeMessage>((entityID, ackMsg) =>
        {
            List<InputInfo> history;
            ClearOlderEntries(ackMsg.clientTime, out history);

            if (history != null)
            {
                InputInfo inputHistorique = history[0];
                Vector2 positionHistorique = inputHistorique.pos; 
                Vector2 positionServeur = ackMsg.confirmedPosition;

                if (positionHistorique != positionServeur)
                {
                    if (ComponentsManager.Instance.TryGetComponent<ShapeComponent>(entityID, out ShapeComponent shape))
                    {
                        Vector2 finalPos = positionServeur;

                        for (int i = 1; i < history.Count; ++i)
                        {
                            finalPos = PositionUpdateSystem.GetNewPosition(finalPos, history[i].input * ECSManager.Instance.SPEED);
                            InputInfo updatedInfo = new InputInfo();
                            updatedInfo.pos = finalPos;
                            updatedInfo.input = history[i].input;
                            updatedInfo.clientTime = history[i].clientTime;
                            history[i] = updatedInfo;
                        }
                        
                        shape.pos = finalPos;
                        ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shape);
                    }
                }
            }       
        });
        ComponentsManager.Instance.ClearComponents<ServerAcknowledgeMessage>();
    }

    public static void ClearOlderEntries(int acknowledgedTime, out List<InputInfo> list)
    { 
        list = ComponentsManager.Instance.InputPositionHistoryList;

        if (list == null)
            return;

        List<InputInfo> listCopy = new List<InputInfo>(list);
        
        foreach (InputInfo entry in listCopy)
        {
            if (entry.clientTime < acknowledgedTime) 
            {
                list.Remove(entry);
            }
        }
    }
}