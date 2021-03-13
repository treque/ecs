using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolAllocator
{
    private int _index = 0;
    private IComponent[] _pool; 
    private IComponent[] _entities;
    private int[] _indirectionTable;

    public IComponent this[EntityComponent entity]
    {
        get { return _pool[_indirectionTable[entity.id]]; }
        set 
        { 
            if (ContainsKey(entity.id))
            {
                _pool[_indirectionTable[entity.id]] = value;
            }
            else
            {
                Allocate((int)entity.id, value);        
            }
        }
    }

    public PoolAllocator()
    {
        InitializeIndirectionTable();
        InitializePool();
    }
    
    private void InitializeIndirectionTable()
    {
        _indirectionTable = new int[ComponentsManager.maxEntities];
        
        for (int i = 0; i < ComponentsManager.maxEntities; ++i)
            _indirectionTable[i] = -1;
    }

    private void InitializePool()
    {
        _pool = new IComponent[ComponentsManager.maxEntities];
        _entities = new IComponent[ComponentsManager.maxEntities];
    }

    private void Allocate(int key, IComponent component)
    {
        _pool[_index] = component;
        _indirectionTable[key] = _index;
        _index++;
    }

    public void Remove(EntityComponent entity)
    {
        if (_indirectionTable[entity.id] != _index - 1)
        {
            IComponent lastItem = _pool[_index - 1];
            int lastItemKey = Array.IndexOf(_indirectionTable, _index-1);
            int removedEntityPoolIndex = _indirectionTable[entity.id];
            _pool[removedEntityPoolIndex] = lastItem; // swap
            _indirectionTable[lastItemKey] = removedEntityPoolIndex;
        }

        _pool[_index-1] = null;
        _indirectionTable[entity.id] = -1;
        _index--;
    }

    public void Clear()
    {
        InitializeIndirectionTable();
        InitializePool();
        _index = 0;
    }

    public bool ContainsKey(EntityComponent entity)
    {
        return _indirectionTable[entity.id] != -1 && _pool[_indirectionTable[entity.id]] != null;
    }

    public IComponent[] Entities
    {
        get
        {
            if (_index > 0)
                _entities = _pool.Take(_index).ToArray();

            return _entities;
        }
    }
}
