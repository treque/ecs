using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Dynamicity System";
        }
    }

    private bool _isDynamicitySet = false;
    public void UpdateSystem()
    {
        if (!_isDynamicitySet)
        {
            SetDynamicities();
            _isDynamicitySet = true;
        }
    }

    private void SetDynamicities()
    {
        Config config = ECSManager.Instance.Config;
        for (uint i = 0; i < Mathf.Floor(config.allShapesToSpawn.Count / 4 * 3); ++i)
        {
            EntityComponent entity = World.Entities[(int)i];
            DynamicComponent dynamic = new DynamicComponent();
            World.Dynamics.Add(entity, dynamic);
        }

    }
}
