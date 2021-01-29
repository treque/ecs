using UnityEngine;

public class SpawnerSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Spawner System";
        }
    }

    private bool _areEntitiesSpawned = false;

    public void UpdateSystem()
    {
        if (!_areEntitiesSpawned)
        {
            World.InitializeWorld();
            SpawnEntities();
            _areEntitiesSpawned = true;
        }
    }

    private void SpawnEntities()
    {
        Config config = ECSManager.Instance.Config;

        for (uint i = 0; i < config.allShapesToSpawn.Count; ++i)
        {
            // Create entity
            EntityComponent entity = new EntityComponent { id = i };
            World.Entities.Add(entity);

            // Create transform components for previous state and current state
            TransformComponent transform = new TransformComponent(config.allShapesToSpawn[(int)i].initialPos, 
                                                                  config.allShapesToSpawn[(int)i].size, 
                                                                  config.allShapesToSpawn[(int)i].size); 
            TransformComponent transformCopy = new TransformComponent(config.allShapesToSpawn[(int)i].initialPos, 
                                                                      config.allShapesToSpawn[(int)i].size, 
                                                                      config.allShapesToSpawn[(int)i].size); 

            // Add tranform components to the previous state and the current state of the entity
            World.States[World.StateName.Previous].Transforms.Add(entity, transformCopy);
            World.States[World.StateName.Current].Transforms.Add(entity, transform);

            // Create dynamic components for the first 3 entities out of 4 for previous state and current state
            if (i < Mathf.Floor(config.allShapesToSpawn.Count / 4 * 3 ))
            {
                DynamicComponent dynamic = new DynamicComponent();
                DynamicComponent dynamicCopy = new DynamicComponent();

                // Add dynamic components to the previous state and the current state of the entity
                World.States[World.StateName.Previous].Dynamics.Add(entity, dynamicCopy);
                World.States[World.StateName.Current].Dynamics.Add(entity, dynamic);

                // Create collision components for previous state and current state only when
                // dynamic entity size is greater or equal to the config's minSize
                if (transform.Size >= config.minSize)
                {
                    CollisionComponent collision = new CollisionComponent();
                    CollisionComponent collisionCopy = new CollisionComponent();
                    World.States[World.StateName.Previous].Collisions.Add(entity, collisionCopy);
                    World.States[World.StateName.Current].Collisions.Add(entity, collision);
                }
            }

            // Create and add collision component to static entities for the previous state
            if (!World.States[World.StateName.Previous].Dynamics.ContainsKey(entity))
            {
                CollisionComponent collisionCopy = new CollisionComponent();
                World.States[World.StateName.Previous].Collisions.Add(entity, collisionCopy);
            }

            // Create and add collision component to static entities for the current state
            if (!World.States[World.StateName.Current].Dynamics.ContainsKey(entity))
            {
                CollisionComponent collision = new CollisionComponent();
                World.States[World.StateName.Current].Collisions.Add(entity, collision);
            }

            ECSManager.Instance.CreateShape(entity.id, config.allShapesToSpawn[(int)i]);

            ColorBehaviour.UpdateColor(entity, World.StateName.Previous);
            ColorBehaviour.UpdateColor(entity, World.StateName.Current);
        }
    }
}
