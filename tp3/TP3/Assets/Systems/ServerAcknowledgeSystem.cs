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
        if (ECSManager.Instance.NetworkManager.isClient)
        {
            UpdateSystemClient();
        }
    }
    public static void UpdateSystemClient()
    {
        ComponentsManager.Instance.ForEach<ServerAcknowledgeMessage>((entityID, ackMsg) =>
        {
            List<KeyValuePair<InputMessage, Vector2>> list;
            ClearOlderEntries(ackMsg.clientTime, out list);
            
            if (list != null)
            {        
                // ack msg time will always correspond to the oldest element in list (the first one) given we cleared cleaned up the list
                KeyValuePair<InputMessage, Vector2> inputHistorique = list[0];
                Vector2 positionHistorique = inputHistorique.Value; 
                Vector2 positionServeur = ackMsg.confirmedPosition;
                /*ServerAcknowledgeSystem.GetHistoryPositionFromTime(
                    ComponentsManager.Instance.InputPositionHistoryList,
                    ackMsg.clientTime
                );*/
                // clear all history of stuff older than acknowledge

                if (positionHistorique != positionServeur)
                {
                    if (ComponentsManager.Instance.TryGetComponent<ShapeComponent>(entityID, out ShapeComponent shape))
                    {
                        shape.pos = positionServeur;
                        Vector2 finalPos = shape.pos;

                        for (int i = 0; i < ComponentsManager.Instance.InputPositionHistoryList.Count; ++i)
                        {
                            KeyValuePair<InputMessage, Vector2> input = ComponentsManager.Instance.InputPositionHistoryList[i];
                                Debug.Log("RATTRAPAGE DES INPUTS");
                                // simuler aussi tous les autres systemes
                                finalPos = PositionUpdateSystem.GetNewPosition(finalPos, input.Key.inputs * 15.0f);
                                KeyValuePair<InputMessage, Vector2> toReplace = new KeyValuePair<InputMessage, Vector2>(
                                    input.Key,
                                    finalPos
                                );
                            
                            input = toReplace;
                        }
                        
                        shape.pos = finalPos;
                        ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shape);
                    }
                }
                else
                {
                    Debug.Log("BONNE POSITION");
                }
            }
            
            //if (ackMsg.confirmedPosition == ComponentsManager.Instance.InputPositionHistory.)
        });

        ComponentsManager.Instance.ClearComponents<ServerAcknowledgeMessage>();
    }

    public static Vector2 GetHistoryPositionFromTime(List<KeyValuePair<InputMessage, Vector2>> list, int t)
    {
        foreach(KeyValuePair<InputMessage, Vector2> pair in list)
        {
            if (pair.Key.clientTime == t)
                return pair.Value;
        }
        return Vector2.zero;
    }

    public static void ClearOlderEntries(int acknowledgedTime, out List<KeyValuePair<InputMessage, Vector2>> list)
    { 
        list = ComponentsManager.Instance.InputPositionHistoryList;

        if (list == null)
            return;

        List<KeyValuePair<InputMessage, Vector2>> listCopy = new List<KeyValuePair<InputMessage, Vector2>>(list);
        
        foreach (KeyValuePair<InputMessage, Vector2> entry in listCopy)
        {
            if (entry.Key.clientTime > acknowledgedTime)
                list.Remove(entry);
        }
    }
}