using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorBehaviour
{
    public static void UpdateColor(EntityComponent entity, World.StateName state)
    {
        ColorComponent colorComponent = new ColorComponent(Color.white);

        if (World.States[state].Dynamics.ContainsKey(entity))
        {
            if (World.States[state].Collisions.ContainsKey(entity))
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

        if (state == World.StateName.Current)
        {
            ECSManager.Instance.UpdateShapeColor(entity.id, colorComponent.Color);

        }

        if (!World.States[state].Colors.ContainsKey(entity))
        {
            World.States[state].Colors.Add(entity, colorComponent);
        }
    }
}
