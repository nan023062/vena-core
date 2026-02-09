
#define UNITY_IOS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using XDFramework.Core;
// ReSharper disable StaticMemberInGenericType
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace XDTGame.Core;

#region PoolObject


public interface IPoolable
{
    void Construct();

    void Deconstruct();
}

public interface IPoolable<TArg>
{
    void Construct(in TArg arg);

    void Deconstruct();
}

internal interface IObjectPool
{
    void Purge();
    
    PoolInfo Info { get; }
}

[StructLayout( LayoutKind.Explicit )]
public readonly struct PoolObjectToken : IEquatable<PoolObjectToken>
{
    [FieldOffset( 0)]
    public readonly uint guid;
    
    [FieldOffset( 4)]
    public readonly int gen;
    
    [FieldOffset( 0)]
    private readonly long _long;
    
    public PoolObjectToken(uint guid, int gen)
    {
        _long = 0;
        this.guid = guid;
        this.gen = gen;
    }
    
    private PoolObjectToken(long value)
    {
        guid = 0;
        gen = 0;
        _long = value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator PoolObjectToken(long value)
    {
        return new PoolObjectToken(value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator long(PoolObjectToken token)
    {
        return token._long;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(PoolObjectToken token)
    {
        return token._long != 0 && token.gen > 0;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(PoolObjectToken left, PoolObjectToken right)
    {
        return left._long == right._long;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(PoolObjectToken left, PoolObjectToken right)
    {
        return left._long != right._long;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(PoolObjectToken other)
    {
        return _long == other._long;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj)
    {
        return obj is PoolObjectToken other && Equals(other);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return _long.GetHashCode();
    }
}

abstract class PoolObject
{
    public readonly uint guid;
    internal int gen;
    
    protected PoolObject(uint guid)
    {
        this.guid = guid;
        gen = 0;
    }
    
    internal abstract object impl { get; }

#if UNITY_EDITOR
    protected enum OperationType : byte
    {
        Null,
        Construct,
        Deconstruct,
    }
    
    protected readonly struct Operation : IDisposable
    {
        private readonly PoolObject _object;
        
        public Operation(PoolObject obj, OperationType type)
        {
            if(obj._operationType != OperationType.Null)
            {
                throw new Exception("busy in operation {obj._operationType}.");
            }
            
            _object = obj;
            _object._operationType = type;
        }
        
        public void Dispose()
        {
            _object._operationType = 0;
        }
    }
    
    protected OperationType _operationType;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(PoolObject obj)
    {
        return obj is { gen: > 0 };
    }
    
    public abstract void Deconstruct();
    
    internal static void Return(in PoolObject wrapper)
    {
#if UNITY_EDITOR
        using var operation = new Operation(wrapper, OperationType.Deconstruct);
#endif
        try
        {
            wrapper.Deconstruct();
        }
        finally
        {
            wrapper.gen = -wrapper.gen;
        }
    }
}

sealed class PoolObject<T> : PoolObject where T : class, IPoolable, new()
{
    static uint _guidAlloc;
    
    public PoolObject() : base(++_guidAlloc)
    {
        ObjectPool<PoolObject<T>>.OnNew(this);
    }
#if UNITY_EDITOR
    ~PoolObject()
    {
        ObjectPool<PoolObject<T>>.OnDelete(this);
    }
#endif

    #region Instance

    public readonly T value = new T();

    internal override object impl
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        get => value;
    }

    private void Construct()
    {
        ObjectPool<PoolObject<T>>.OnSpawn(this);
        
        value.Construct();
    }
    
    public override void Deconstruct()
    {
        try
        {
            value.Deconstruct();
        }
        finally
        {
            ObjectPool<PoolObject<T>>.OnUnspawn(this);
        }
    }

    #endregion

    #region Pool

    internal static PoolObject<T> Rent()
    {
        PoolObject<T> wrapper = ObjectPool<PoolObject<T>>.Rent();

#if UNITY_EDITOR
        using var operation = new Operation(wrapper, OperationType.Construct);
#endif
        wrapper.Construct();
        return wrapper;
    }
    
    #endregion
}

sealed class PoolObject<T, TArg> : PoolObject where T : class, IPoolable<TArg>, new()
{
    static uint _guidAlloc;
    
    public PoolObject() : base(++_guidAlloc)
    {
        ObjectPool<PoolObject<T, TArg>>.OnNew(this);
    }

#if UNITY_EDITOR
    ~PoolObject()
    {
        ObjectPool<PoolObject<T, TArg>>.OnDelete(this);
    }
#endif
    
    public readonly T value = new T();
    
    internal override object impl
    {
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        get => value;
    }

    private void Construct(in TArg arg)
    {
        ObjectPool<PoolObject<T, TArg>>.OnSpawn(this);
        
        value.Construct(arg);
    }
    
    public override void Deconstruct()
    {
        try
        {
            value.Deconstruct();
        }
        finally
        {
            ObjectPool<PoolObject<T, TArg>>.OnUnspawn(this);
        }
    }

    internal static PoolObject<T, TArg> Rent(in TArg arg)
    {
        PoolObject<T, TArg> wrapper = ObjectPool<PoolObject<T, TArg>>.Rent();

#if UNITY_EDITOR
        using var operation = new Operation(wrapper, OperationType.Construct);
#endif
        wrapper.Construct(arg);
        return wrapper;
    }
}

#endregion

#region PoolPofiler

class PoolInfo : IComparable<PoolInfo>
{
    public readonly Type type;
    private readonly string name;
    public int usedCount;
    public int totalCount;

    public override string ToString()
    {
        return $"{name}( used: {usedCount}, total: {totalCount} )";
    }

    public PoolInfo(Type type)
    {
        this.type = type;
        name = GetTypeName(type);
        usedCount = 0;
        totalCount = 0;
    }

    int IComparable<PoolInfo>.CompareTo(PoolInfo other)
    {
        int result = other.totalCount.CompareTo(totalCount);

        if (result != 0) return result;

        int unusedCount = totalCount - usedCount;

        int otherUnusedCount = other.totalCount - other.usedCount;

        return otherUnusedCount.CompareTo(unusedCount);
    }

    static string GetTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        StringBuilder sb = new StringBuilder();
        sb.Clear();

        // 获取泛型类型的定义
        string name = type.GetGenericTypeDefinition().FullName;
        if (string.IsNullOrEmpty(name)) return type.Name;
        int nameSpaceIndex = name.LastIndexOf('.');
        if (nameSpaceIndex > 0)
            name = name.Substring(nameSpaceIndex + 1);

        // 获取泛型参数的类型
        Type[] genericArguments = type.GetGenericArguments();
        sb.Append($"{name}<{string.Join(",", genericArguments.Select(t => t.Name))}>");
        return sb.ToString();
    }
}

class ObjectPool<T> : IObjectPool where T : PoolObject, new()
{
    private static readonly ObjectPool<T> _instance;

    private static readonly List<T> _pool;

    private static readonly Dictionary<object, T> _usedImpls;
    
    private static readonly PoolInfo _info;
    
    static ObjectPool()
    {
        _pool = new List<T>();

        _usedImpls = new Dictionary<object, T>();

        _instance = new ObjectPool<T>();
        
        _info = new PoolInfo(typeof(T));

        PoolManager.Register(_instance);
    }

    void IObjectPool.Purge()
    {
        Purge();
    }

    PoolInfo IObjectPool.Info
    {
        get
        {
            /*
            int poolCount = _pool.Count;
            int usedCount = _usedImpls.Count;
            int totalCount = poolCount + usedCount;
            if( _info.totalCount != totalCount || _info.usedCount != usedCount)
            {
                _info.totalCount = totalCount;
                _info.usedCount = usedCount;
            }*/

            return _info;
        }
    }
    
    
    internal static void Purge()
    {
        _pool.Clear();
    }
    
    internal static T Rent()
    {
        T poolObject;
        
        if (!PoolManager.UsePool)
        {
            poolObject = new T();
            poolObject.gen = 1;
            return poolObject;
        }
        
        int count = _pool.Count;

        if (0 < count)
        {
            poolObject = _pool[count - 1];

            _pool.RemoveAt(count - 1);
        }
        else
        {
            poolObject = new T();
        }

        int gen = Math.Abs(poolObject.gen);
        if (gen == int.MaxValue) gen = 0;
        poolObject.gen = gen + 1;
        
        _usedImpls.Add(poolObject.impl, poolObject);
        
        return poolObject;
    }
    
    internal static PoolObject GetObject(in object impl) 
    {
        _usedImpls.TryGetValue(impl, out var obj);
        return obj;
    }
    
    internal static void Return(in T wrapper)
    {
        var impl = wrapper.impl;
        if (_usedImpls.TryGetValue(impl, out var obj))
        {
            if (wrapper != obj)
            {
                throw new ArgumentException($"recycle error: {impl} mismatched.");
            }
                
            try
            {
                PoolObject.Return(wrapper);
                return;
            }
            finally
            {
                _usedImpls.Remove(impl);
                _pool.Add(obj);
            }
        }
            
        throw new ArgumentException($"Invalid object: {impl}");
    }
    
    [Conditional("UNITY_EDITOR")]
    internal static void OnNew(T instance)
    {
        _info.totalCount++;
    }

    [Conditional("UNITY_EDITOR")]
    internal static void OnSpawn(T instance)
    {
        _info.usedCount++;
    }

    [Conditional("UNITY_EDITOR")]
    internal static void OnUnspawn(T instance)
    {
        _info.usedCount--;
    }

    [Conditional("UNITY_EDITOR")]
    internal static void OnDelete(T instance)
    {
        _info.totalCount--;
    }
}

public static class PoolManager
{
    public static bool UsePool = true;

    static readonly Dictionary<Type, IObjectPool> _pools = new();
    
    const string POOL_INFO = "[ POOL_INFO ]: ";

    /// <summary>
    /// Register the pool.
    /// </summary>
    internal static void Register<T>(ObjectPool<T> pool) where T : PoolObject, new()
    {
        Type type = typeof(T);

        _pools.Add(type, pool);
    }

    /// <summary>
    /// dump the pool info.
    /// </summary>
    public static void Dump()
    {
        int usedCount = 0, totalCount = 0;

        List<PoolInfo> poolInfos = new List<PoolInfo>();
        foreach (var objectPool in _pools.Values)
        {
            PoolInfo poolInfo = objectPool.Info;
            poolInfos.Add(poolInfo);
            usedCount += poolInfo.usedCount;
            totalCount += poolInfo.totalCount;
        }

        poolInfos.Sort();

        DebugSystem.LogWarning(LogCategory.Framework,
            $"{POOL_INFO}******************* Pool Profiler Begin ( {usedCount} / {totalCount} ) ****************");
        foreach (var poolInfo in poolInfos)
        {
            DebugSystem.LogWarning(LogCategory.Framework, $"{POOL_INFO} {poolInfo}");
        }

        DebugSystem.LogWarning(LogCategory.Framework,
            $"{POOL_INFO}******************* Pool Profiler End ( {usedCount} / {totalCount} ) ****************");
    }

    /// <summary>
    /// Purge all objects in the pool.
    /// </summary>
    public static void Purge()
    {
        foreach (var objectPool in _pools.Values)
        {
            objectPool.Purge();
        }
    }

    /// <summary>
    /// Return the object with token.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Rent<T>() where T : class, IPoolable, new()
    {
        return PoolObject<T>.Rent().value;
    }
    
    /// <summary>
    /// Rent the object with token.
    /// </summary>
    /// <param name="arg"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TArg"></typeparam>
    /// <returns></returns>
    public static T Rent<T, TArg>(in TArg arg) where T : class, IPoolable<TArg>, new()
    {
        return PoolObject<T, TArg>.Rent(arg).value;
    }
    
    /// <summary>
    /// Return the object token.
    /// </summary>
    /// <param name="impl"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PoolObjectToken GetToken<T, TArg>(in T impl) where T : class, IPoolable<TArg> , new()
    {
        if (!UsePool) return 0;
        
        var poolObject = ObjectPool<PoolObject<T, TArg>>.GetObject(impl);
        
        if (null == poolObject) return 0;
        
        return new PoolObjectToken(poolObject.guid, poolObject.gen);
    }
    
    /// <summary>
    /// Return the object token.
    /// </summary>
    /// <param name="impl"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PoolObjectToken GetToken<T>(in T impl) where T : class, IPoolable , new()
    {
        if (!UsePool) return 0;
        
        var poolObject = ObjectPool<PoolObject<T>>.GetObject(impl);
        
        if (null == poolObject) return 0;
        
        return new PoolObjectToken(poolObject.guid, poolObject.gen);
    }

    /// <summary>
    /// Return the object impl.
    /// </summary>
    /// <param name="impl"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return<T>(T impl) where T : class, IPoolable, new()
    {
        if (!UsePool)
        {
            impl.Deconstruct();
            return;
        }
        
        var poolObject = ObjectPool<PoolObject<T>>.GetObject(impl);
        if (poolObject is PoolObject<T> obj)
        {
            ObjectPool<PoolObject<T>>.Return(obj);
            return;
        }
        throw new ArgumentException($"Invalid object: {impl}");
    }
    
    /// <summary>
    /// Return the object impl.
    /// </summary>
    /// <param name="impl"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return<T, TArg>(T impl) where T : class, IPoolable<TArg>, new()
    {
        if (!UsePool)
        {
            impl.Deconstruct();
            return;
        }
        
        var poolObject = ObjectPool<PoolObject<T, TArg>>.GetObject(impl);
        if (poolObject is PoolObject<T, TArg> obj)
        {
            ObjectPool<PoolObject<T, TArg>>.Return(obj);
        }
    }
}

#endregion