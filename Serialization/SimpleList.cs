using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vena
{
    public class SimpleList<T>
    {
        private T[] array;
        private int capacity;
        private int size;

        public SimpleList(int capacity = 4)
        {
            array = new T[capacity];
            this.capacity = capacity;
            size = 0;
        }


        [Conditional("Debug")]
        void IndexCheck(int index)
        {
            if (index < 0 || index >= size)
            {
                throw new IndexOutOfRangeException();
            }
        }
        public ref T this[int index]
        {
            get
            {
                IndexCheck(index);
                return ref array[index];
            }
        }

        public int Count => size;
        public int Capacity => capacity;

        public ref T Add()
        {
            int index = size;
            size += 1;
            if (capacity < size)
            {
                int newCapacity = System.Math.Max(size, capacity * 2);
                capacity = newCapacity;
                Array.Resize(ref array, newCapacity);
            }

            return ref array[index];
        }
        
        public void Add(in T elem)
        {
            int index = size;
            size += 1;
            if (capacity < size)
            {
                int newCapacity = System.Math.Max(size, capacity * 2);
                capacity = newCapacity;
                Array.Resize(ref array, newCapacity);
            }
            array[index] = elem;
        }

        public void Resize(int newSize)
        {
            if( size > newSize )
            {
                Array.Clear( array, newSize, size - newSize);
                size = newSize;
            }
            else
            {
                for( int i = size; i < newSize; i++ )
                {
                    Add( default );
                }
            }
        }

        public System.ArraySegment<T> GetArraySegment()
        {
            return new ArraySegment<T>(array, 0, size);
        }

        public T[] ToArray()
        {
            var ret = new T[size];
            if (size > 0)
            {
                Array.Copy(array, ret, size);
            }

            return ret;
        }

        public void Clear()
        {
            if (size > 0)
            {
#if NETCOREAPP
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
                Array.Clear(array, 0, size);
            }
            size = 0;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator () {
            return new Enumerator (array, size);
        }

        public struct Enumerator {
            readonly T[] _array;
            readonly int _count;
            int _idx;

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            internal Enumerator (T[] array, int count) {
                _array = array;
                _count = count;
                _idx = -1;
            }

            public ref T Current {
                [MethodImpl (MethodImplOptions.AggressiveInlining)]
                get { return ref _array[_idx]; }
            }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            public void Dispose () { }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            public bool MoveNext () {
                return ++_idx < _count;
            }
        }
    }
}
