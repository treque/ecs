using UnityEngine;
public struct InputMessage : IComponent
{
   public int inputMessageID;
   public bool handled;
   public Vector2 inputs;
   public int clientTime;
   public uint entityID;
}
