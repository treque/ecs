using UnityEngine;

public class TransformComponent : IComponent
{
    public Vector2 Position;
    public float Size;
    public float InitalSize;
    public TransformComponent(Vector2 position, float size)
    {
        this.Position = position;
        this.Size = size;
        this.InitalSize = size;
    }
}
