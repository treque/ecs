using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocitySystem : ISystem
{
    public string Name
    {
        get
        {
            return "Velocity System";
        }
    }

    private bool _areVelocitiesSet = false;

    public void UpdateSystem()
    {
        if (!_areVelocitiesSet)
        {
            InitializeVelocities();
            _areVelocitiesSet = true;
        }

        UpdateVelocities();
    }

    private void InitializeVelocities()
    {
        for (int i = 0; i < World.Entities.Count; ++i)
        {
            EntityComponent entity = World.Entities[i];      
            Config config = ECSManager.Instance.Config;

            if (World.Dynamics.ContainsKey(entity))
            {
                VelocityComponent velocity = new VelocityComponent(config.allShapesToSpawn[i].initialSpeed);
                World.Velocities.Add(entity, velocity);
            }
        }
    }
    
    private void UpdateVelocities()
    {
        for (int i = 0; i < World.Entities.Count; ++i)
        {
            
            //EntityComponent entity = World.Entities[i];
        }
    }
}
