//#define BAD_PERF // TODO CHANGEZ MOI. Mettre en commentaire pour utiliser votre propre structure

using System;
using UnityEngine;

#if BAD_PERF
using InnerType = System.Collections.Generic.Dictionary<uint, IComponent>;
using AllComponents = System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Dictionary<uint, IComponent>>;
#else
using InnerType = PoolAllocator; // TODO CHANGEZ MOI, UTILISEZ VOTRE PROPRE TYPE ICI
using AllComponents = System.Collections.Generic.Dictionary<uint, PoolAllocator>; // TODO CHANGEZ MOI, UTILISEZ VOTRE PROPRE TYPE ICI
#endif

// Appeler GetHashCode sur un Type est couteux. Cette classe sert a precalculer le hashcode
public static class TypeRegistry<T> where T : IComponent
{
    public static uint typeID = (uint)Mathf.Abs(default(T).GetRandomNumber()) % ComponentsManager.maxEntities;
}

public class Singleton<V> where V : new()
{
    private static bool isInitiated = false;
    private static V _instance;
    public static V Instance
    {
        get
        {
            if (!isInitiated)
            {
                isInitiated = true;
                _instance = new V();
            }
            return _instance;
        }
    }
    protected Singleton() { }
}

internal class ComponentsManager : Singleton<ComponentsManager>
{
    private AllComponents _allComponents = new AllComponents();

    public const int maxEntities = 950;

    public void DebugPrint()
    {
        string toPrint = "";
        var allComponents = Instance.DebugGetAllComponents();
        foreach (var type in allComponents)
        {
            toPrint += $"{type}: \n";
#if !BAD_PERF
            foreach (var component in type.Value.Entities)
#else
            foreach (var component in type.Value)
#endif
            {
#if BAD_PERF
                toPrint += $"\t{component.Key}: {component.Value}\n";
#else
                toPrint += $"\t{component}: {component}\n";
#endif
            }
            toPrint += "\n";
        }
        Debug.Log(toPrint);
    }

    // CRUD
    public void SetComponent<T>(EntityComponent entityID, IComponent component) where T : IComponent
    {
        if (!_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            _allComponents[TypeRegistry<T>.typeID] = new InnerType();
        }
        _allComponents[TypeRegistry<T>.typeID][entityID] = component;   
    }
    public void RemoveComponent<T>(EntityComponent entityID) where T : IComponent
    {
        _allComponents[TypeRegistry<T>.typeID].Remove(entityID);
    }
    public T GetComponent<T>(EntityComponent entityID) where T : IComponent
    {
        return (T)_allComponents[TypeRegistry<T>.typeID][entityID];
    }
    public bool TryGetComponent<T>(EntityComponent entityID, out T component) where T : IComponent
    {
        if (_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            if (_allComponents[TypeRegistry<T>.typeID].ContainsKey(entityID))
            {
                component = (T)_allComponents[TypeRegistry<T>.typeID][entityID];
                return true;
            }
        }
        component = default;
        return false;
    }

    public bool EntityContains<T>(EntityComponent entity) where T : IComponent
    {
        return _allComponents[TypeRegistry<T>.typeID].ContainsKey(entity);
    }

    public void ClearComponents<T>() where T : IComponent
    {
        if (!_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            _allComponents.Add(TypeRegistry<T>.typeID, new InnerType());
        }
        else
        {
           _allComponents[TypeRegistry<T>.typeID].Clear();
        }
    }

    public void ForEach<T1>(Action<EntityComponent, T1> lambda) where T1 : IComponent
    {
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Entities;
        
        PoolAllocator poolAllocator_1 = _allComponents[TypeRegistry<T1>.typeID];
        
        foreach (EntityComponent entity in allEntities)
        {
            if (!poolAllocator_1.ContainsKey(entity))
            { 
                continue;
            }
            lambda(entity, (T1)poolAllocator_1[entity]);
        }
    }

    public void ForEach<T1, T2>(Action<EntityComponent, T1, T2> lambda) where T1 : IComponent where T2 : IComponent
    {
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Entities;

        PoolAllocator poolAllocator_1 = _allComponents[TypeRegistry<T1>.typeID];
        PoolAllocator poolAllocator_2 = _allComponents[TypeRegistry<T2>.typeID];

        foreach(EntityComponent entity in allEntities)
        {
            if (!poolAllocator_1.ContainsKey(entity) ||
                !poolAllocator_2.ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)poolAllocator_1[entity], (T2)poolAllocator_2[entity]);
        }
    }

    public void ForEach<T1, T2, T3>(Action<EntityComponent, T1, T2, T3> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Entities;

        PoolAllocator poolAllocator_1 = _allComponents[TypeRegistry<T1>.typeID];
        PoolAllocator poolAllocator_2 = _allComponents[TypeRegistry<T2>.typeID];
        PoolAllocator poolAllocator_3 = _allComponents[TypeRegistry<T3>.typeID];

        foreach (EntityComponent entity in allEntities)
        {
            if (!poolAllocator_1.ContainsKey(entity) ||
                !poolAllocator_2.ContainsKey(entity) ||
                !poolAllocator_3.ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)poolAllocator_1[entity], (T2)poolAllocator_2[entity], (T3)poolAllocator_3[entity]);
        }
    }

    public void ForEach<T1, T2, T3, T4>(Action<EntityComponent, T1, T2, T3, T4> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Entities;

        PoolAllocator poolAllocator_1 = _allComponents[TypeRegistry<T1>.typeID];
        PoolAllocator poolAllocator_2 = _allComponents[TypeRegistry<T2>.typeID];
        PoolAllocator poolAllocator_3 = _allComponents[TypeRegistry<T3>.typeID];
        PoolAllocator poolAllocator_4 = _allComponents[TypeRegistry<T4>.typeID];

        foreach (EntityComponent entity in allEntities)
        {
            if (!poolAllocator_1.ContainsKey(entity) ||
                !poolAllocator_2.ContainsKey(entity) ||
                !poolAllocator_3.ContainsKey(entity) ||
                !poolAllocator_4.ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)poolAllocator_1[entity], (T2)poolAllocator_2[entity], (T3)poolAllocator_3[entity], (T4)poolAllocator_4[entity]);
        }
    }

    public AllComponents DebugGetAllComponents()
    {
        return _allComponents;
    }
}
