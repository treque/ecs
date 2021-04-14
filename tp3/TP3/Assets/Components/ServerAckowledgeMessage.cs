using UnityEngine;

public struct ServerAcknowledgeMessage : IComponent
{
    public int ackMessageID;
    public Vector2 confirmedPosition;
    public int clientTime;
    public uint entityID;
}