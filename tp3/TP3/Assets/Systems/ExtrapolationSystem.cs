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
        ulong rtt = 0;// ECSManager.Instance.GetLastRTT();
        int framesToExtrapolate = (int)(((rtt) / 1000f) / Time.deltaTime);
        for (int i = 0; i < framesToExtrapolate; ++i)
        {
            SimulateNonClientEntities();
        }
    }

    private void SimulateNonClientEntities()
    {
        //Todo // either at every frame you set the client's position/speed to what it was before the simulation
        // either you change the position
        //PositionUpdateSystem.UpdateSystem(); 
    }
}
