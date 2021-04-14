using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrapolationSystem : ISystem
{
  
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.isClient && ECSManager.Instance.Config.enablDeadReckoning)
        {
            UpdateSystemClient();
        }
    }
    public void UpdateSystemClient()
    {
        ulong rtt = ECSManager.Instance.NetworkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(
            ECSManager.Instance.NetworkManager.LocalClientId
        );
        int framesToExtrapolate = (int)(((rtt) / 1000f) / Time.deltaTime);
        for (int i = 0; i < framesToExtrapolate; ++i)
        {
            SimulateNonClientEntities();
        }
    }

    private void SimulateNonClientEntities()
    {
        // either at every frame you set the client's position/speed to what it was before the simulation
        // either you change the position
        foreach(ISystem system in ECSManager.Instance.SystemsToExtrapolate)
        {
            system.UpdateSystem();
        }
    }
}
