using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using XDTGame.Core;

namespace XDT.Utility
{
    interface IReusableObjectPool
    {
        bool Push(_ReusableObjectBase obj);
        
        void Purge();
    }
    
    // TODO: Capacity
    public class ReusableObjectPool<T> : IReusableObjectPool where T : _ReusableObjectBase, new()
    {
        private static int _version = 0;
        public static readonly ReusableObjectPool<T> Instance = new ReusableObjectPool<T>();
        
        private ReusableObjectPool() 
        {
            ReusableObjectFactory.RegisterPool(this);
        }

        private readonly List<T> _pool = new List<T>();

        public bool Push(_ReusableObjectBase obj)
        { 
            if (!ReusableObjectFactory.UsePool) return false;
            var tObj = obj as T;
            if(null == tObj) Debug.Assert(null != tObj);
            _pool.Add(tObj);
            return true;
        }
        
        public void Purge()
        {
            _pool.Clear();
        }

        public T Pop(out int token)
        {
            T obj = null;
            if (0 < _pool.Count)
            {
               obj = _pool[_pool.Count - 1];
                _pool.RemoveAt(_pool.Count - 1);
            }
            else
            {
               obj = new T();
            }
            token = ++_version;
            if(_version >= int.MaxValue - 10) _version = 0;
            return obj;
        }
    }
    
    public class ReusableObjectConcurrentQueuePool<T> : IReusableObjectPool where T : _ReusableObjectBase, new()
    {
        private static int _version = 0;
        public static readonly ReusableObjectConcurrentQueuePool<T> Instance = new ReusableObjectConcurrentQueuePool<T>();
        
        private ReusableObjectConcurrentQueuePool() 
        {
            ReusableObjectFactory.RegisterPool(this);
        }

        private readonly ConcurrentQueue<T> _pool = new ConcurrentQueue<T>();
        private readonly object _lock = new object();
        
        public void Purge()
        {
            while (_pool.TryDequeue(out _))
            {
                
            }
        }
        
        public bool Push(_ReusableObjectBase obj)
        {
            lock (_lock)
            {
                if (!ReusableObjectFactory.UsePool) return false;
                var tObj = obj as T;
           
                if(null == tObj) Debug.Assert(null != tObj);
                _pool.Enqueue(tObj);
                return true;
            }
        }

        public T Pop(out int token)
        {
            lock (_lock)
            {
                if (!_pool.TryDequeue(out T obj))
                {
                    obj = new T();
                }
                token = ++_version;
                if(_version >= int.MaxValue - 10) _version = 0;
                return obj;
            }
        }
    }
}