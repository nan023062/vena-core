using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace XDT.Utility
{
    [Obsolete ("废弃代码， 请优先使用PoolManager")]
    public static class ReusableObjectFactory
    {
        private static readonly HashSet<IReusableObjectPool> _pools = new HashSet<IReusableObjectPool>();
        
        internal static void RegisterPool(IReusableObjectPool pool)
        {
            _pools.Add(pool);
        }
        
        public static void Purge()
        {
            foreach (var pool in _pools)
            {
                pool.Purge();
            }
        }
        
        /// <summary>
        /// 从ConcurrentQueue中获取对象，线程安全高效 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateConcurrentReusableObject<T>() where T : ReusableObjectBase, new()
        {
            var obj = _CreateOrReuseConcurrent<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct();
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        public static T CreateReusableObject<T>() where T : ReusableObjectBase, new()
        {
            var obj = _CreateOrReuse<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct();
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        /// <summary>
        /// 从ConcurrentQueue中获取对象，线程安全高效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateConcurrentReusableObject<T, P1>(P1 p1) where T : ReusableObjectBase<P1>, new()
        {
            var obj = _CreateOrReuseConcurrent<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct(p1);
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        public static T CreateReusableObject<T, P1>(P1 p1) where T : ReusableObjectBase<P1>, new()
        {
            var obj = _CreateOrReuse<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct(p1);
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        [Obsolete ("废弃代码， 请优先使用IPoolable<T>")]
        public static T CreateReusableObject<T, P1, P2>(P1 p1, P2 p2) where T : ReusableObjectBase<P1, P2>, new()
        {
            var obj = _CreateOrReuse<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct(p1, p2);
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        [Obsolete ("废弃代码， 请优先使用IPoolable<T>")]
        public static T CreateReusableObject<T, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where T : ReusableObjectBase<P1, P2, P3>, new()
        {
            var obj = _CreateOrReuse<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct(p1, p2, p3);
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        [Obsolete ("废弃代码， 请优先使用IPoolable<T>")]
        public static T CreateReusableObject<T, P1, P2, P3, P4>(P1 p1, P2 p2, P3 p3, P4 p4) where T : ReusableObjectBase<P1, P2, P3, P4>, new()
        {
            var obj = _CreateOrReuse<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct(p1, p2, p3, p4);
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        [Obsolete ("废弃代码， 请优先使用IPoolable<T>")]
        public static T CreateReusableObject<T, P1, P2, P3, P4, P5>(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5) where T : ReusableObjectBase<P1, P2, P3, P4, P5>, new()
        {
            var obj = _CreateOrReuse<T>();
#if DEBUG
            obj._constructing_DontUse = true;
#endif
            obj.Construct(p1, p2, p3, p4, p5);
#if DEBUG
            obj._constructing_DontUse = false;
#endif
            return obj;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static void DisposeReusableObject(_ReusableObjectBase obj)
        {
#if DEBUG
            if (obj._constructing_DontUse)
            {
                throw new Exception("ErrorCode: Dispose self in Construct");
            }
#endif
            var pool = obj._pool_DontUse;
            try
            {
                ((IPoolObject)obj).Deconstruct();
            }
            finally
            {
                obj._pool_DontUse = null;
                pool?.Push(obj);
            }
        }
        
        public static bool UsePool = true;
        
        private static T _CreateOrReuse<T>() where T : _ReusableObjectBase, new()
        {
            var obj = ReusableObjectPool<T>.Instance.Pop( out var token);
            obj._pool_DontUse = ReusableObjectPool<T>.Instance;
            ((IPoolObject)obj).Construct(token);
            return obj;
        }
        
        // 线程安全
        private static T _CreateOrReuseConcurrent<T>() where T : _ReusableObjectBase, new()
        {
            var obj = ReusableObjectConcurrentQueuePool<T>.Instance.Pop( out var token);
            obj._pool_DontUse = ReusableObjectConcurrentQueuePool<T>.Instance;
            ((IPoolObject)obj).Construct(token);
            return obj;
        }
    }
    
    /// <summary>
    /// a weak reference PoolObject with no parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct WeakRef<T> : IEquatable<WeakRef<T>> where T : _ReusableObjectBase
    {
        /// <summary>
        /// The token of the object.
        /// </summary>
        public readonly int token;

        /// <summary>
        /// The object.
        /// </summary>
        private readonly T _object;
    
        public WeakRef(T obj)
        {
            _object = obj;

            token = _object.token;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(WeakRef<T> wref)
        {
            if (wref.token > 0 && wref._object)
            {
                return wref.token == wref._object.token;
            }

            return false;
        }

        public bool Equals(WeakRef<T> other)
        {
            return token == other.token && Equals(_object, other._object);
        }

        public override bool Equals(object obj)
        {
            return obj is WeakRef<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return token + (_object != null ? _object.GetHashCode() : 0);
        }

        /// <summary>
        /// Try to get the reference object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(out T value)
        {
            // check if the object is still valid and the token is the same.
            if (token > 0 && null != _object && _object.token == token)
            {
                value = _object;
                return true;
            }

            value = default;
            return false;
        }
    }
}