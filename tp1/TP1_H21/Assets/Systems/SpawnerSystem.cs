using System.Collections;
using System.Collections.Generic;
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
            SpawnEntities();
            _areEntitiesSpawned = true;
        }
    }

    private void SpawnEntities()
    {
        Config config = ECSManager.Instance.Config;

        for (uint i = 0; i < config.allShapesToSpawn.Count; ++i)
        {
            EntityComponent entity = new EntityComponent { id = i };
            World.Entities.Add(entity);

            TransformComponent transform = new TransformComponent(config.allShapesToSpawn[(int)i].initialPos, config.allShapesToSpawn[(int)i].size);

            World.Transforms.Add(entity, transform);

            // Setting first 3/4 entities dynamic (ask LP if it needs to be random)
            if (i < Mathf.Floor(config.allShapesToSpawn.Count / 4 * 3 ))
            {
                DynamicComponent dynamic = new DynamicComponent();
                World.Dynamics.Add(entity, dynamic);

                if (transform.Size >= config.minSize)
                {
                    CollisionComponent collision = new CollisionComponent();
                    World.Collisions.Add(entity, collision);
                }
            }

            ECSManager.Instance.CreateShape(entity.id, config.allShapesToSpawn[(int)i]);
        }
    }


}
