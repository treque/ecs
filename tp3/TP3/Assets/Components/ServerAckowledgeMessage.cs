using UnityEngine;

public struct ServerAcknowledgeMessage : IComponent
{
    public int inputMessageID;
    public Vector2 confirmedPosition;

    public int clientTime;

}