using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformBehaviour
{
    // Start is called before the first frame update
    public static void UpdateTransform(EntityComponent entity, World.StateName state)
    {
        Vector2 currentPosition = World.States[state].Transforms[entity].Position;
        Config config = ECSManager.Instance.Config;

        if (World.States[state].Dynamics.ContainsKey(entity))
        {
            World.States[state].Transforms[entity].Position = currentPosition + World.States[state].Velocities[entity].Velocity * Time.deltaTime;
        }

        if (World.States[state].Transforms[entity].Position.y >= 0 && !World.States[state].TopHalf.ContainsKey(entity))
        {
            World.States[state].TopHalf.Add(entity, new TopHalfComponent());
        }
        else
        {
            World.States[state].TopHalf.Remove(entity);
        }

        if (state == World.StateName.Current)
        {
            ECSManager.Instance.UpdateShapePosition(entity.id, World.States[state].Transforms[entity].Position);
            ECSManager.Instance.UpdateShapeSize(entity.id, World.States[state].Transforms[entity].Size);
        }

        if (World.States[state].Transforms[entity].Size < config.minSize && World.States[state].Collisions.ContainsKey(entity))
        {
            World.States[state].Collisions.Remove(entity);
        }
    }
}
