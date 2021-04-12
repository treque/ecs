using UnityEngine;
public struct InputMessage : IComponent
{
   public ReplicationMessage message;
   public bool handled;
   public Vector2 inputs;

}
