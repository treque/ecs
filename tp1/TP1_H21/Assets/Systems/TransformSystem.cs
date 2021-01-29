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
        foreach (EntityComponent entity in World.Entities)
        {
            UpdateTransform(entity, World.StateName.Current);
        }
    }

    public static void UpdateTransform(EntityComponent entity, World.StateName state)
    {
        TransformBehaviour.UpdateTransform(entity, state);
    }
}
