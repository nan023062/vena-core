using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace XDTGame.Core;

public struct WeakArray<T> : IDisposable
{
    private readonly int _length;

    private Array _array;

    public WeakArray(int length)
    {
        _length = Array.GetCapacity(length);
        _array = null;
    }
    
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => null != _array ? _length : 0;
    }

    public void Clear()
    {
        if (null != _array)
        {
            var array = _array;

            _array = default;

            Array.Return(array);
        }
    }

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if(index < 0 || index >= _length)
            {
                throw new IndexOutOfRangeException();
            }
            if (_array != null)
            {
                return _array.array[index];
            }
            return default;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if(index < 0 || index >= _length)
            {
                throw new IndexOutOfRangeException();
            }
            
            _array ??= Array.Rent(_length);
            
            _array.array[index] = value;
        }
    }

    public T[] ToArray()
    {
        if (_array != null)
        {
            T[] toArray = new T[_array.array.Length];
            System.Array.Copy(_array.array, toArray, _array.array.Length);
            return toArray;
        }
        return System.Array.Empty<T>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(List<T> destination)
    {
        CopyTo(0, destination, Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(int arrayIndex, List<T> destination, int length)
    {
        if (_array != null)
        {
            length = Math.Min(arrayIndex + length, _array.array.Length);

            for (int i = arrayIndex; i < length; i++)
            {
                destination.Add(_array.array[i]);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(in WeakArray<T> source, ref WeakArray<T> destination, int length)
    {
        Copy(source, 0, ref destination, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Copy(in WeakArray<T> source, int sourceIndex, ref WeakArray<T> destination, int destinationIndex, int length)
    {
        if (source._array != null)
        {
            if (destination._array != null)
            {
                var sourceArray = source._array;
                var destinationArray = destination._array;
                System.Array.Copy(sourceArray.array, sourceIndex, destinationArray.array, destinationIndex, length);
                return;
            }

            throw new IndexOutOfRangeException($"destination array is null");
        }
        
        throw new IndexOutOfRangeException($"source array is null");
    }

    public void Dispose()
    {
        if (null != _array)
        {
            var array = _array;

            _array = default;

            Array.Return(array);
        }
    }
    
    public Enumerator GetEnumerator()
    {
        if (null != _array)
        {
            return new Enumerator(_array, 0, _array.array.Length);
        }
        return new Enumerator(Array.Empty, 0, 0);
    }

    public struct Enumerator : IEnumerator<T>
    {
        private readonly Array array;
        private readonly int endIndex;
        private readonly int startIndex;
        private int index;
        private bool _complete;

        internal Enumerator(Array array, int index, int count)
        {
            this.array = array;

            if (array._lock)
            {
                throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
            }

            this.array._lock = true;
            startIndex = index - 1;
            endIndex = index + count;
            this.index = startIndex;
            _complete = false;
        }

        public bool MoveNext()
        {
            if (_complete)
            {
                index = endIndex;
                return false;
            }

            ++index;
            _complete = index >= endIndex;
            return !_complete;
        }

        public void Reset()
        {
            index = startIndex;
            _complete = false;
        }

        object IEnumerator.Current => ((IEnumerator<T>)this).Current;

        T IEnumerator<T>.Current
        {
            get
            {
                if (index < startIndex)
                    throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
                if (_complete)
                    throw new InvalidOperationException("InvalidOperation_EnumEnded");
                return array.array[index];
            }
        }

        public void Dispose()
        {
            // TODO release managed resources here
            array._lock = false;
        }
    }
    
    internal class Array 
    {
        public static readonly Array Empty = new Array(new T[0]);
        public readonly T[] array;
        public readonly int capacity;
        public bool _lock;
        
        protected Array(T[] array)
        {
            this.array = array;
            capacity = array.Length;
            _lock = false;
        }
        
        public static int GetCapacity(int capacity)
        {
            if(capacity <= 8) return 8;
            if(capacity <= 32) return 32;
            if(capacity <= 128) return 128;
            if(capacity <= 512) return 512;
            if(capacity <= 1024) return 1024;
            throw new ArgumentOutOfRangeException($"capacity = {capacity} is invalid");
        }
        
        public static Array Rent(int capacity)
        {
            switch (capacity)
            {
                case 8:  return  PoolManager.Rent<Array8>();
                case 32: return  PoolManager.Rent<Array32>();
                case 128: return  PoolManager.Rent<Array128>();
                case 512: return  PoolManager.Rent<Array512>();
                case 1024: return  PoolManager.Rent<Array1024>();
                default:
                    throw new ArgumentOutOfRangeException($"capacity = {capacity} is invalid");
            }
        }
    
        public static void Return(Array array)
        {
            switch (array.capacity)
            {
                case 8: PoolManager.Return((Array8)array); break;
                case 32: PoolManager.Return((Array32)array); break;
                case 128: PoolManager.Return((Array128)array); break;
                case 512: PoolManager.Return((Array512)array); break;
                case 1024: PoolManager.Return((Array1024)array); break;
                default:
                    throw new ArgumentOutOfRangeException($"capacity = {array.capacity} is invalid");
            }
        }
    }
    
    class Array8 : Array,IPoolable
    {
        public Array8 () : base(new T[8])
        {
        }
        
        public void Construct()
        {
            _lock = false;
        }

        public void Deconstruct()
        {
            _lock = false;
            // clear array
            System.Array.Clear(array, 0, array.Length);
        }
    }
    
    class Array32 : Array,IPoolable
    {
        public Array32 () : base(new T[32])
        {
        }

        public void Construct()
        {
            _lock = false;
        }

        public void Deconstruct()
        {
            _lock = false;
            // clear array
            System.Array.Clear(array, 0, array.Length);
        }
    }
    
    class Array128 : Array,IPoolable
    {
        public Array128 () : base(new T[128])
        {
        }

        public void Construct()
        {
            _lock = false;
        }

        public void Deconstruct()
        {
            _lock = false;
            // clear array
            System.Array.Clear(array, 0, array.Length);
        }
    }
    
    class Array512 : Array,IPoolable
    {
        public Array512 () : base(new T[512])
        {
        }

        public void Construct()
        {
            _lock = false;
        }

        public void Deconstruct()
        {
            _lock = false;
            // clear array
            System.Array.Clear(array, 0, array.Length);
        }
    }
    
    class Array1024 : Array,IPoolable
    {
        public Array1024 () : base(new T[1024])
        {
        }

        public void Construct()
        {
            _lock = false;
        }

        public void Deconstruct()
        {
            _lock = false;
            // clear array
            System.Array.Clear(array, 0, array.Length);
        }
    }
}
