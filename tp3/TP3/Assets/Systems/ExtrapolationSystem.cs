using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrapolationSystem : ISystem
{
    private bool _done = false;
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    public void UpdateSystem()
    {
        if (!_done && ECSManager.Instance.NetworkManager.isClient && ECSManager.Instance.Config.enablDeadReckoning)
        {
            UpdateSystemClient();
        }
    }
    public void UpdateSystemClient()
    {
        ulong rtt = ECSManager.Instance.NetworkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(
            ECSManager.Instance.NetworkManager.LocalClientId
        );

        ulong latency = rtt/2;
        int framesToExtrapolate = (int)(((latency) / 1000f) / Time.deltaTime);
        for (int i = 0; i < framesToExtrapolate; ++i)
        {
            SimulateNonClientEntities();
        }
    }

    private void SimulateNonClientEntities()
    {
        foreach(ISystem system in ECSManager.Instance.SystemsToExtrapolate)
        {
            system.UpdateSystem();
        }

        // We do this once at the start only. Eventually, the simulation should be ran again when the server's
        // state does not match the client's
        // We should do that correction in a new system that saves the previous states so that we can compare
        // to ours, and interpolate between them for smoothing.
        _done = true;
    }
}
