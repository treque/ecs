using System.Collections.Generic;
using UnityEngine;


public class ReplicationSystem : ISystem
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
        if (ECSManager.Instance.NetworkManager.isServer)
        {
            UpdateSystemServer();
        }
        else if (ECSManager.Instance.NetworkManager.isClient)
        {
            UpdateSystemClient();
        }
    }

    public static void UpdateSystemServer()
    {
        // creates messages from current state

        ComponentsManager.Instance.ForEach<ShapeComponent>((entityID, shapeComponent) => {
            ReplicationMessage msg = new ReplicationMessage() {
                messageID = 0,
                timeCreated = Utils.SystemTime,
                entityId = entityID.id,
                shape = shapeComponent.shape,
                pos = shapeComponent.pos,
                speed = shapeComponent.speed,
                size = shapeComponent.size
            };
            ComponentsManager.Instance.SetComponent<ReplicationMessage>(entityID, msg);
        });
    }

    public static void UpdateSystemClient()
    {
        // apply state from server
        // can receive only one replication message per entity for simplicity
        ComponentsManager.Instance.ForEach<ReplicationMessage>((entityID, msgReplication) => {
            // Updating entity info from message's state
            var component = ComponentsManager.Instance.GetComponent<ShapeComponent>(msgReplication.entityId);

            if (component.shape != msgReplication.shape)
            {
                // needs to respawn entity to change its shape
                bool spawnFound = ComponentsManager.Instance.TryGetComponent(new EntityComponent(0), out SpawnInfo spawnInfo);

                if (!spawnFound)
                {
                    spawnInfo = new SpawnInfo(false);
                }
                spawnInfo.replicatedEntitiesToSpawn.Add(msgReplication);
                ComponentsManager.Instance.SetComponent<SpawnInfo>(new EntityComponent(0), spawnInfo);
            }
            else
            {
                // On préfère que vous évaluez notre travail sur le input prediction séparément de notre travail sur
                // l'extrapolation parce qu'il nous manquait du temps pour combiner les deux.
                if (!ECSManager.Instance.Config.enableInputPrediction && 
                    ECSManager.Instance.Config.enablDeadReckoning && 
                    msgReplication.entityId == ECSManager.Instance.NetworkManager.LocalClientId)
                {
                    component.pos = msgReplication.pos;
                    component.speed = msgReplication.speed;
                    component.size = msgReplication.size;
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(msgReplication.entityId, component);
                }
                
                if (ECSManager.Instance.Config.enableInputPrediction && 
                    !ECSManager.Instance.Config.enablDeadReckoning && 
                    msgReplication.entityId != ECSManager.Instance.NetworkManager.LocalClientId)
                {
                    component.pos = msgReplication.pos;
                    component.speed = msgReplication.speed;
                    component.size = msgReplication.size;
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(msgReplication.entityId, component);
                }
            }
        });
    }
}
