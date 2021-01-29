using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionBehaviour
{
    private static void InverseVelocity(EntityComponent entity, World.StateName state)
    {
        if (World.States[state].Velocities.ContainsKey(entity))
        {
            World.States[state].Velocities[entity].Velocity *= -1;
        }
    }

    private static void ModifySizeAfterCollision(EntityComponent entity, World.StateName state)
    {
        if (World.States[state].Dynamics.ContainsKey(entity))
        {
            World.States[state].Transforms[entity].Size /= 2;
        } 
    }

    public static void UpdateCollision(EntityComponent entity, World.StateName state)
    {
        if (World.States[state].Dynamics.ContainsKey(entity))
        {
            if (CollisionWithEdgesBehaviour.CollidesWithEdges(World.States[state].Transforms[entity]))
            {      
                InverseVelocity(entity, state); 

                if (World.States[state].Transforms[entity].Size != World.States[state].Transforms[entity].InitalSize)
                {
                    // When we reset the size, the circle doubles in size at the edge, so we add an offset
                    Vector3 offset = Vector3.Normalize(World.States[state].Velocities[entity].Velocity) * World.States[state].Transforms[entity].InitalSize/2;
                    Vector2 offsetVector2 = new Vector2(offset.x, offset.y);

                    World.States[state].Transforms[entity].Position += offsetVector2;
                    World.States[state].Transforms[entity].Size = World.States[state].Transforms[entity].InitalSize;
                }

                if (!World.States[state].Collisions.ContainsKey(entity))
                {
                    World.States[state].Collisions.Add(entity, new CollisionComponent());
                }
            }
        }

        if (World.States[state].Collisions.ContainsKey(entity))
        {
            Dictionary<EntityComponent, CollisionComponent> collisionsCopy = new Dictionary<EntityComponent, CollisionComponent>(World.States[state].Collisions);         
        
            // collision
            foreach (EntityComponent otherEntity in collisionsCopy.Keys)
            {
                if (otherEntity != entity)
                {
                    Vector2 position1 =  World.States[state].Transforms[entity].Position;
                    Vector2 position2 =  World.States[state].Transforms[otherEntity].Position;

                    float distanceBetweenCircles = Mathf.Sqrt((position2.x - position1.x) * (position2.x - position1.x) + (position2.y - position1.y) * (position2.y - position1.y));

                    if (distanceBetweenCircles < (World.States[state].Transforms[entity].Size/2 + World.States[state].Transforms[otherEntity].Size/2))
                    {
                        InverseVelocity(entity, state);
                        InverseVelocity(otherEntity, state);

                        ModifySizeAfterCollision(entity, state);
                        ModifySizeAfterCollision(otherEntity, state);

                        if (World.States[state].Dynamics.ContainsKey(entity))
                            World.States[state].Collisions.Remove(entity);
                        
                        if (World.States[state].Dynamics.ContainsKey(otherEntity))
                            World.States[state].Collisions.Remove(otherEntity);
                    }
                }
            }
        }
    }
}
