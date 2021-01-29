using UnityEngine;

public class RewindSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Rewind System";
        }
    }

    private const float _COOLDOWN_TIME = 2.0f;
    private float _timer = _COOLDOWN_TIME;
    
    public void UpdateSystem()
    {
        if (_timer > 0.0f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Rewind unavailable");
            }

            _timer -= Time.deltaTime;
            return;
        }
        
        TickPreviousState();
        _timer = 0.0f;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _timer = _COOLDOWN_TIME;
            Rewind();
        }
    }

    private void TickPreviousState()
    {
        foreach (EntityComponent entity in World.Entities)
        {
            ColorBehaviour.UpdateColor(entity, World.StateName.Previous);
            CollisionBehaviour.UpdateCollision(entity, World.StateName.Previous);
            TransformBehaviour.UpdateTransform(entity, World.StateName.Previous);
            TopHalfBehaviour.UpdateTopHalf(entity, World.StateName.Previous);
        }
    }

    private void ClearCurrentState()
    {
        World.States[World.StateName.Current].Transforms.Clear();
        World.States[World.StateName.Current].Collisions.Clear();
        World.States[World.StateName.Current].Colors.Clear();
        World.States[World.StateName.Current].TopHalf.Clear();
        World.States[World.StateName.Current].Velocities.Clear();
    }
    
    private void Rewind()
    {
        ClearCurrentState();

        foreach(var item in World.States[World.StateName.Previous].Transforms)
        {
            TransformComponent transformCopy = new TransformComponent(item.Value.Position, item.Value.Size, item.Value.InitalSize);
            World.States[World.StateName.Current].Transforms.Add(item.Key, transformCopy);
        }

        foreach(var item in World.States[World.StateName.Previous].Collisions)
        {
            CollisionComponent collisionCopy = new CollisionComponent();
            World.States[World.StateName.Current].Collisions.Add(item.Key, collisionCopy);
        }

        foreach(var item in World.States[World.StateName.Previous].Colors)
        {
            ColorComponent colorCopy = new ColorComponent(item.Value.Color);
            World.States[World.StateName.Current].Colors.Add(item.Key, colorCopy);
        }

        foreach(var item in World.States[World.StateName.Previous].TopHalf)
        {
            TopHalfComponent topHalfCopy = new TopHalfComponent();
            World.States[World.StateName.Current].TopHalf.Add(item.Key, topHalfCopy);
        }

        foreach(var item in World.States[World.StateName.Previous].Velocities)
        {
            VelocityComponent velocityCopy = new VelocityComponent(item.Value.Velocity);
            World.States[World.StateName.Current].Velocities.Add(item.Key, velocityCopy);
        }
    }
}
