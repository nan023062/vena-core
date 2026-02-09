using System;
using System.Runtime.CompilerServices;
using XDFramework.Core;

namespace XDT.Utility
{
    interface IPoolObject
    {
        void Construct(int token);
        
        void Deconstruct();
    }
    
    public abstract class _ReusableObjectBase : IPoolObject, IDisposable
    {
        /// <summary>
        /// The token of the object. each time the object is created, the token will be increased.
        /// </summary>
        public int token
        {
            private set;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        
#if DEBUG
        public bool _constructing_DontUse;
#endif
        internal IReusableObjectPool _pool_DontUse;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool (_ReusableObjectBase obj)
        {
            return obj is { token: > 0 };
        }
        
        public void Dispose()
        {
            if (token <= 0)
            {
                DebugSystem.LogWarning(LogCategory.GameLogic,$"Duplicate Dispose: {GetType()}");
                return;
            }
            ReusableObjectFactory.DisposeReusableObject(this);
        }
        
        void IPoolObject.Construct(int token)
        {
            this.token = token;
            //Construct();
        }

        void IPoolObject.Deconstruct()
        {
            token = 0;
            Deconstruct();
        }
        
        public abstract void Deconstruct();
    }
    
    public abstract class ReusableObjectBase : _ReusableObjectBase
    {
        public abstract void Construct();
    }
    
    public abstract class ReusableObjectBase<P1> : _ReusableObjectBase
    {
        public abstract void Construct(P1 p1);
    }
    
    [Obsolete ("废弃代码")]
    public abstract class ReusableObjectBase<P1, P2> : _ReusableObjectBase
    {
        public abstract void Construct(P1 p1, P2 p2);
    }
    
    [Obsolete ("废弃代码")]
    public abstract class ReusableObjectBase<P1, P2, P3> : _ReusableObjectBase
    {
        public abstract void Construct(P1 p1, P2 p2, P3 p3);
    }
    
    [Obsolete ("废弃代码")]
    public abstract class ReusableObjectBase<P1, P2, P3, P4> : _ReusableObjectBase
    {
        public abstract void Construct(P1 p1, P2 p2, P3 p3, P4 p4);
    }
    
    [Obsolete ("废弃代码")]
    public abstract class ReusableObjectBase<P1, P2, P3, P4, P5> : _ReusableObjectBase
    {
        public abstract void Construct(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);
    }
}
