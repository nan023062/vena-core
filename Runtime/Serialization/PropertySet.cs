using System;
using System.Collections.Generic;

namespace Vena
{
    /// <summary>
    /// 属性容器
    /// </summary>
    public struct PropertySet : IDisposable
    {
        private static readonly Dictionary<ValueType, Stack<Value>> _valuePool = new();

        private static readonly Stack<Dictionary<Variable, Value>> _mapPool = new();

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public static void Free()
        {
            _valuePool.Clear();

            _mapPool.Clear();
        }

        private Dictionary<Variable, Value> _set;

        public int Count => _set.Count;

        public PropertySet(int capacity = 0)
        {
            _set = _mapPool.Count > 0 ? _mapPool.Pop() : new Dictionary<Variable, Value>();
        }

        public readonly void Set<T>(in Variable variable, in T value) where T : struct, IValue<T>
        {
            if (_set.TryGetValue(variable, out var hasValue))
            {
                ((Value<T>)hasValue).value = value;
            }
            else
            {
                Value<T> valueRef;

                if (_valuePool.TryGetValue(Value<T>.TypeId, out var stack) && stack.Count > 0)
                {
                    valueRef = (Value<T>)stack.Pop();
                }
                else
                {
                    valueRef = new Value<T>();
                }

                valueRef.value = value;

                _set.Add(variable, valueRef);
            }
        }

        public readonly bool Has(in Variable variable)
        {
            return _set.ContainsKey(variable);
        }

        public readonly ref readonly T Get<T>(in Variable variable) where T : struct, IValue<T>
        {
            if (_set.TryGetValue(variable, out var value))
            {
                return ref ((Value<T>)value).value;
            }

            return ref Value<T>.Default;
        }

        public void Dispose()
        {
            // can't dispose twice
            if (null == _set)
                return;

            var dictionary = _set;

            _set = default;

            if (dictionary.Count > 0)
            {
                foreach (var value in dictionary.Values)
                {
                    ValueType type = value.Type;

                    if (!_valuePool.TryGetValue(type, out var stack))
                    {
                        stack = new Stack<Value>();

                        _valuePool.Add(type, stack);
                    }

                    stack.Push(value);
                }

                dictionary.Clear();
            }

            // return dictionary to pool
            _mapPool.Push(dictionary);
        }

        public Dictionary<Variable, Value>.Enumerator GetEnumerator()
        {
            return _set.GetEnumerator();
        }
    }
}