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
            // t4
            // t1 t3 t4
            // t1
            
            if (list != null)
            {        
                //list.Sort( (entry1, entry2)=>entry1.Key.clientTime.CompareTo(entry2.Key.clientTime));
                // ack msg time will always correspond to the oldest element in list (the first one) given we cleared cleaned up the list
                KeyValuePair<InputMessage, Vector2> inputHistorique = list[0];
                Vector2 positionHistorique = inputHistorique.Value; 
                Vector2 positionServeur = ackMsg.confirmedPosition;
                /*ServerAcknowledgeSystem.GetHistoryPositionFromTime(
                    ComponentsManager.Instance.InputPositionHistoryList,
                    ackMsg.clientTime
                );*/
                // clear all history of stuff older than acknowledge
                Debug.Log("Server ACK for time: " + ackMsg.clientTime);
                Debug.Log("historique: " + positionHistorique + "  serveur:  " + positionServeur);
                foreach(KeyValuePair<InputMessage, Vector2> entry in list)
                {
                    Debug.Log("time: +  " + entry.Key.clientTime + "  pos: " + entry.Value);
                }
                if (positionHistorique != positionServeur)
                {
                    Debug.Log("NOT EQUAL");
                    /*if (ComponentsManager.Instance.TryGetComponent<ShapeComponent>(entityID, out ShapeComponent shape))
                    {
                        shape.pos = positionServeur;
                        Vector2 finalPos = shape.pos;

                        for (int i = 0; i < ComponentsManager.Instance.InputPositionHistoryList.Count; ++i)
                        {
                            KeyValuePair<InputMessage, Vector2> input = ComponentsManager.Instance.InputPositionHistoryList[i];
                                //Debug.Log("RATTRAPAGE DES INPUTS");
                                // simuler aussi tous les autres systemes
                                finalPos = PositionUpdateSystem.GetNewPosition(finalPos, input.Key.inputs * 20.0f);
                                KeyValuePair<InputMessage, Vector2> toReplace = new KeyValuePair<InputMessage, Vector2>(
                                    input.Key,
                                    finalPos
                                );
                            
                            input = toReplace;
                        }
                        
                        shape.pos = finalPos;
                        ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shape);
                    }*/
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
            if (entry.Key.clientTime < acknowledgedTime) 
            {
                list.Remove(entry);
                Debug.Log("Clearing at time: " + entry.Key.clientTime);
            }
        }
    }
}