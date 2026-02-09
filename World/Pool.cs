using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace XDTGame.Core;

public sealed partial class World
{
    /// <summary>
    /// world use pool
    /// </summary>
    public bool UseWorldObjectPool = true;
    
    private readonly Dictionary<Type, Stack<WorldObject>> _pools = new();
    
    public void Purge()
    {
        // Purge all pools
        _pools.Clear();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T Get<T>(Type type) where T : WorldObject
    {
        Stack<WorldObject> pool = null;
        
        if (UseWorldObjectPool && !_pools.TryGetValue(type, out pool))
        {
            pool = new Stack<WorldObject>();
            
            _pools.Add(type, pool);
        }

        T obj;
        
        if (UseWorldObjectPool && pool is { Count: > 0 })
        {
            WorldObject.Checker.ThrowIsUnSafe(WorldObject.CallFunc.Create);
                
            obj = (T)pool.Pop();
        }
        else
        {
            obj = (T)Activator.CreateInstance(type);
        }
        
        // 版本号变正数且数值+1
        int ver = Math.Abs(obj._version);
        if(ver == int.MaxValue) ver = 0;
        obj._version = ver + 1;
        
        return obj;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Release(WorldObject obj)
    {
        if (obj._version > 0)
        {
            // 版本号变负数
            obj._version = -obj._version;
            
            WorldObject.Checker.ThrowIsUnSafe(WorldObject.CallFunc.Destroy);
            
            if (UseWorldObjectPool)
            {
                Type type = obj.GetType();
                
                if (!_pools.TryGetValue(type, out Stack<WorldObject> pool))
                {
                    pool = new Stack<WorldObject>();
            
                    _pools.Add(type, pool);
                }
        
                pool.Push(obj);
            }
        }
    }
}