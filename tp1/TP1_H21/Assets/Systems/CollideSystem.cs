using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Collide System";
        }
    }


    public void UpdateSystem()
    {
        for (uint i = 0; i < World.Entities.Count; ++i)
        {
            EntityComponent entity = World.Entities[(int)i];

            if (World.Dynamics.ContainsKey(entity))
            {
                if (CollidesWithEdges(World.Transforms[entity]))
                {
                    InverseVelocity(entity);                 
                    
                    if (World.Transforms[entity].Size != World.Transforms[entity].InitalSize)
                    {
                        // When we reset the size, the circle doubles in size at the edge, so we add an offset
                        Vector3 offset = Vector3.Normalize(World.Velocities[entity].Velocity) * World.Transforms[entity].InitalSize/2;
                        Vector2 offsetVector2 = new Vector2(offset.x, offset.y);

                        World.Transforms[entity].Position += offsetVector2;
                        World.Transforms[entity].Size = World.Transforms[entity].InitalSize;
                    }

                    if (!World.Collisions.ContainsKey(entity))
                    {
                        World.Collisions.Add(entity, new CollisionComponent());
                    }
                }
            }

            if (World.Collisions.ContainsKey(entity))
            {
                Dictionary<EntityComponent, CollisionComponent> collisionsCopy = new Dictionary<EntityComponent, CollisionComponent>(World.Collisions);         
            
                // collision
                foreach (EntityComponent otherEntity in collisionsCopy.Keys)
                {
                    if (otherEntity != entity)
                    {
                        Vector2 position1 =  World.Transforms[entity].Position;
                        Vector2 position2 =  World.Transforms[otherEntity].Position;

                        float distanceBetweenCircles = Mathf.Sqrt((position2.x - position1.x) * (position2.x - position1.x) + (position2.y - position1.y) * (position2.y - position1.y));

                        if (distanceBetweenCircles < (World.Transforms[entity].Size/2 + World.Transforms[otherEntity].Size/2))
                        {
                            InverseVelocity(entity);
                            InverseVelocity(otherEntity);

                            ModifySizeAfterCollision(entity);
                            ModifySizeAfterCollision(otherEntity);

                            World.Collisions.Remove(entity);
                            World.Collisions.Remove(otherEntity);
                        }
                    }
                }
            }
        }
    }

    private bool CollidesWithEdges(TransformComponent transform)
    {
        float radius = transform.Size/2;
        float left_x = Camera.main.WorldToScreenPoint(new Vector3(transform.Position.x - radius, 0, 0)).x;
        float right_x = Camera.main.WorldToScreenPoint(new Vector3(transform.Position.x + radius, 0, 0)).x;
        float top_y = Camera.main.WorldToScreenPoint(new Vector3(0, transform.Position.y + radius, 0)).y;
        float bottom_y = Camera.main.WorldToScreenPoint(new Vector3(0, transform.Position.y - radius, 0)).y;

        return (left_x <= 0 || right_x >= Camera.main.pixelWidth || top_y >= Camera.main.pixelHeight || bottom_y <= 0);
    }

    private void InverseVelocity(EntityComponent entity)
    {
        World.Velocities[entity].Velocity *= -1;
    }

    private void ModifySizeAfterCollision(EntityComponent entity)
    {
        if (World.Dynamics.ContainsKey(entity))
        {
            World.Transforms[entity].Size /= 2;
        } 
    }
}
