using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSystem : ISystem
{
    public string Name
    {
        get
        {
            return "Color System";
        }
    }
    public void UpdateSystem()
    {
        for (uint i = 0; i < World.Entities.Count; ++i)
        {
            EntityComponent entity = World.Entities[(int)i];
            ColorComponent colorComponent = new ColorComponent(Color.white);

            if (World.Dynamics.ContainsKey(entity))
            {
                if (World.Collisions.ContainsKey(entity))
                {
                    colorComponent.Color = Color.blue;
                }
                else
                {
                    colorComponent.Color = Color.green;
                }
            }
            else
            {
                colorComponent.Color = Color.red;
            }

            ECSManager.Instance.UpdateShapeColor(i, colorComponent.Color);

            if (!World.Colors.ContainsKey(entity))
            {
                World.Colors.Add(entity, colorComponent);
            }
        }
    }

}
