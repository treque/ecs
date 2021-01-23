using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Transform System";
        }
    }

    public void UpdateSystem()
    {
        for (uint i = 0; i < World.Entities.Count; ++i)
        {
            EntityComponent entity = World.Entities[(int)i];
            Vector2 currentPosition = World.Transforms[entity].Position;
            Config config = ECSManager.Instance.Config;

            if (World.Dynamics.ContainsKey(entity))
            { 
                World.Transforms[entity].Position = currentPosition + World.Velocities[entity].Velocity * Time.deltaTime;
            }

            ECSManager.Instance.UpdateShapePosition(entity.id, World.Transforms[entity].Position);
            ECSManager.Instance.UpdateShapeSize(entity.id, World.Transforms[entity].Size);

            if (World.Transforms[entity].Size < config.minSize && World.Collisions.ContainsKey(entity))
            {
                World.Collisions.Remove(entity);
            }
        }
    }

}
