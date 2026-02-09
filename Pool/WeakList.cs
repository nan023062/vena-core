using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using XDFramework.Core;

namespace XDTGame.Core;

/// <summary>
/// A list that can be reused.
/// The list is created when the first element is added, and is cleared when the Clear method is called.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct WeakList<T> : IDisposable
{
    private readonly int _capacity;
    private _List _object;
    
    public WeakList(int capacity = 8)
    {
        _capacity = _List.GetCapacity(capacity);
        _object = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(WeakList<T> obj)
    {
        return obj._object != null;
    }
    
    public void Dispose()
    {
        if (_object != null)
        {
            var list = _object;
            _object = null;
            _List.Return(list);
        }
    }
    
    public List<T> List
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (null == _object)
            {
                int safeCapacity = _List.GetCapacity(_capacity);
                _object = _List.Rent(safeCapacity);
            }
            return _object.list;
        }
    }
    
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_object != null)
            {
                int count = _object.list.Count;
                if (count == 0)
                {
                    var list = _object;
                    _object = null;
                    _List.Return(list);
                }
                
                return count;
            }
            
            return 0;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray()
    {
        if (_object != null)
        {
            return _object.list.ToArray();
        }
        return Array.Empty<T>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        if (_object != null)
        {
            var list = _object;
            _object = null;
            _List.Return(list);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        int newCount = 1;
        if (null != _object)
        {
            newCount = _object.list.Count + 1;
        }
        _object = MakeSureCapacity(_object, newCount);
        _object.list.Add(value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(T value)
    {
        if (_object != null)
        {
            if (_object.list.Remove(value))
            {
                int count = _object.list.Count;
                if (count == 0)
                {
                    var list = _object;
                    _object = null;
                    _List.Return( list);
                }
                
                return true;
            }
        }

        return false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool RemoveAt(int index)
    {
        if (_object != null)
        {
            int count = _object.list.Count;
            
            if (index < 0 || index >= count) 
                return false;
            
            _object.list.RemoveAt(index);
            
            count = _object.list.Count;
            if (count == 0)
            {
                var list = _object;
                _object = null;
                _List.Return( list);
            }
            
            return true;
        }
        
        return false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(T value)
    {
        if (_object != null)
        {
            return _object.list.Contains(value);
        }

        return false;
    }
    
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_object != null)
            {
                return _object.list[index];
            }
            throw new InvalidOperationException();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (_object != null)
            {
                _object.list[index] = value;
                return;
            }
            throw new InvalidOperationException();
        }
    }
    
    public int FindIndex(Predicate<T> match)
    {
        if (_object != null)
        {
            return _object.list.FindIndex(match);
        }
        return -1;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<T>.Enumerator GetEnumerator()
    {
        if (null == _object)
        {
            return _List.Default.list.GetEnumerator();
        }
        
        return _object.list.GetEnumerator();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort()
    {
        if (_object != null)
        {
            _object.list.Sort();
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort(Comparison<T> comparison)
    {
        if (_object != null)
        {
            _object.list.Sort(comparison);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort(IComparer<T> comparer)
    {
        if (_object != null)
        {
            _object.list.Sort(comparer);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort(int index, int count, IComparer<T> comparer)
    {
        if (_object != null)
        {
            _object.list.Sort(index, count, comparer);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reverse()
    {
        if (_object != null)
        {
            _object.list.Reverse();
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reverse(int index, int count)
    {
        if (_object != null)
        {
            _object.list.Reverse(index, count);
        }
    }
    
    /// <summary>
    /// Ensure the capacity of the list.
    /// </summary>
    /// <param name="size"></param>
    private static _List MakeSureCapacity(_List list, int count)
    {
        int capacity = _List.GetCapacity(count);
        
        if (list == null)
        {
            return _List.Rent(capacity);
        }
        
        if (list.capacity < capacity)
        {
            var oldList = list;
            
            list = _List.Rent(capacity);
            
            list.list.AddRange(oldList.list);
            
            _List.Return(oldList);
        }
        return list;
    }
    
    class _List
    {
        public static readonly _List Default = new _List(2);
        
        public readonly List<T> list;
        
        public readonly int capacity;
        
        protected _List(int capacity)
        {
            list = new List<T>(capacity);
            this.capacity = capacity <= 1024 ? capacity : int.MaxValue;
        }
        
        public static int GetCapacity(int capacity)
        {
            if (capacity <= 8) return 8;
            if (capacity <= 32) return 32;
            if (capacity <= 64) return 64;
            if (capacity <= 256) return 256;
            if (capacity <= 512) return 512;
            if (capacity <= 1024) return 1024;
            return 2048;
        }
        
        public static _List Rent(int capacity)
        {
            switch (capacity)
            {
                case 8:  return  PoolManager.Rent<List8>();
                case 32: return  PoolManager.Rent<List32>();
                case 64: return  PoolManager.Rent<List64>();
                case 256:return  PoolManager.Rent<List256>();
                case 512:return  PoolManager.Rent<List512>();
                case 1024:return  PoolManager.Rent<List1024>();
                default:
                {
                    DebugSystem.LogWarning(LogCategory.Framework ,$"Rent List with large capacity: {capacity}, use ListLimited instead.");
                    return  PoolManager.Rent<ListLimited>();
                }
            }
        }
        
        public static void Return(_List list)
        {
            switch (list.capacity)
            {
                case 8:  PoolManager.Return<List8>((List8)list); break;
                case 32: PoolManager.Return<List32>((List32)list); break;
                case 64: PoolManager.Return<List64>((List64)list); break;
                case 256:PoolManager.Return<List256>((List256)list); break;
                case 512:PoolManager.Return<List512>((List512)list); break;
                case 1024:PoolManager.Return<List1024>((List1024)list); break;
                default: PoolManager.Return<ListLimited>((ListLimited)list); break;
            }
        }
    }
    
    class List8 : _List, IPoolable
    {
        public List8() : base(8)
        {
        }

        public void Construct()
        {
        }

        public void Deconstruct()
        {
            list.Clear();
        }
    }
    
    class List32 : _List, IPoolable
    {
        public List32() : base(32)
        {
        }

        public void Construct()
        {
        }

        public void Deconstruct()
        {
            list.Clear();
        }
    }
    
    class List64 : _List, IPoolable
    {
        public List64() : base(64)
        {
        }

        public void Construct()
        {
        }

        public void Deconstruct()
        {
            list.Clear();
        }
    }
    
    class List256 : _List, IPoolable
    {
        public List256() : base(256)
        {
        }

        public void Construct()
        {
        }

        public void Deconstruct()
        {
            list.Clear();
        }
    }
    
    class List512 : _List, IPoolable
    {
        public List512() : base(512)
        {
        }

        public void Construct()
        {
        }

        public void Deconstruct()
        {
            list.Clear();
        }
    }
    
    class List1024 : _List, IPoolable
    {
        public List1024() : base(1024)
        {
        }

        public void Construct()
        {
        }

        public void Deconstruct()
        {
            list.Clear();
        }
    }
    
    
    class ListLimited : _List, IPoolable
    {
        public ListLimited() : base(2048)
        {
        }

        public void Construct()
        {
        }

        public void Deconstruct()
        {
            list.Clear();
        }
    }
}

