using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RegisterSystemsToExtrapolate
{
    public static List<ISystem> GetListOfSystemsToExtrapolate(List<ISystem> allSystems)
    {
        List<ISystem> toExtrapolate = new List<ISystem>();
        toExtrapolate.Add(
            allSystems.Find(system => system.Name == "WallCollisionDetectionSystem" ));
        toExtrapolate.Add(
            allSystems.Find(system => system.Name == "CircleCollisionDetectionSystem" ));
        toExtrapolate.Add(
            allSystems.Find(system => system.Name == "BounceBackSystem" ));
        toExtrapolate.Add(
            allSystems.Find(system => system.Name == "PositionUpdateSystem" ));
        return toExtrapolate;
    }
}