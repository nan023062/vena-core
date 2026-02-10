using System;
using System.Runtime.InteropServices;

namespace Vena.BehaviourTree
{
    public enum ValueType : byte
    {
        Bool,
        Int,
        Float,
    }
    
    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public struct Var
    {
        [FieldOffset(0)] public int intValue;
        [FieldOffset(0)] public bool boolValue;
        [FieldOffset(0)] public float floatValue;
        
        public static implicit operator Var(int value)
        {
            return new Var{intValue = value};
        }

        public static implicit operator Var(bool value)
        {
            return new Var{boolValue = value};
        }

        public static implicit operator Var(float value)
        {
            return new Var{floatValue = value};
        }

        public static implicit operator bool(Var value)
        {
            return value.boolValue;
        }

        public static implicit operator int(Var value)
        {
            return value.intValue;
        }

        public static implicit operator float(Var value)
        {
            return value.floatValue;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public readonly struct Value : IEquatable<Value>
    {
        [FieldOffset(0)] public readonly ValueType valueType;
        [FieldOffset(4)] public readonly Var var;
        
        public Value(bool value)
        {
            valueType = ValueType.Bool;
            var = value;
        }

        public Value(int value)
        {
            valueType = ValueType.Int;
            var = value;
        }

        public Value(float value)
        {
            valueType = ValueType.Float;
            var = value;
        }
        
        public override string ToString()
        {
            switch (valueType)
            {
                case ValueType.Bool:
                    return $"<{(byte)valueType}-{var.boolValue}>";
                case ValueType.Int:
                    return $"<{(byte)valueType}-{var.intValue}>";
                case ValueType.Float:
                    return $"<{(byte)valueType}-{var.floatValue}>";
            }

            throw new Exception("Unsupport value type");
        }

        public static implicit operator Value(int value)
        {
            return new Value(value);
        }

        public static implicit operator Value(bool value)
        {
            return new Value(value);
        }

        public static implicit operator Value(float value)
        {
            return new Value(value);
        }

        public static implicit operator bool(Value value)
        {
            if (value.valueType != ValueType.Bool)
                throw new InvalidCastException("Value type is not bool");
            return value.var.boolValue;
        }

        public static implicit operator int(Value value)
        {
            if (value.valueType != ValueType.Int)
                throw new InvalidCastException("Value type is not int");
            return value.var.intValue;
        }

        public static implicit operator float(Value value)
        {
            if (value.valueType != ValueType.Float)
                throw new InvalidCastException("Value type is not float");
            return value.var.floatValue;
        }

        public static Value operator +(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue + b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue + b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '+'");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue + b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue + b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '+'");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '+'");
        }

        public static Value operator -(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue - b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue - b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '-'");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue - b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue - b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '-'");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '-'");
        }

        public static Value operator *(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue * b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue * b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '*'");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue * b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue * b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '*'");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '*'");
        }

        public static Value operator /(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue / b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue / b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '/'");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue / b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue / b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '/'");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '/'");
        }

        public static bool operator >(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue > b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue > b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '>'");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue > b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue > b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '>'");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '>'");
        }

        public static bool operator <(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue < b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue < b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '<'");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue < b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue < b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '<'");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '<'");
        }

        public static bool operator >=(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue >= b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue >= b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '>='");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue >= b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue >= b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '>='");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '>='");
        }

        public static bool operator <=(Value a, Value b)
        {
            switch (a.valueType)
            {
                case ValueType.Int:
                {
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.intValue <= b.var.intValue;
                        case ValueType.Float:
                            return a.var.intValue <= b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '<='");
                }
                 
                case ValueType.Float:
                    switch (b.valueType)
                    {
                        case ValueType.Int:
                            return a.var.floatValue <= b.var.intValue;
                        case ValueType.Float:
                            return a.var.floatValue <= b.var.floatValue;
                    }
                    throw new InvalidCastException($"b value. {b.valueType} is not support op '<='");
            }
            
            throw new InvalidCastException($"a value. {a.valueType} is not support op '<='");
        }
        
        public static bool operator ==(Value a, Value b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Value a, Value b)
        {
            return !a.Equals(b);
        }
        
        public Value Pow(in Value pow)
        {
            switch (pow.valueType)
            {
                case ValueType.Int:
                {
                    int intPow = pow;
                    switch (valueType)
                    {
                        case ValueType.Int:
                            return UnityEngine.Mathf.Pow(var.intValue, intPow);
                        case ValueType.Float:
                            return UnityEngine.Mathf.Pow(var.floatValue, intPow);
                    }
                    throw new InvalidCastException($"value. {valueType} is not support pow");
                }

                case ValueType.Float:
                {
                    float fPow = pow;
                    switch (valueType)
                    {
                        case ValueType.Int:
                            return UnityEngine.Mathf.Pow(var.intValue, fPow);
                        case ValueType.Float:
                            return UnityEngine.Mathf.Pow(var.floatValue, fPow);
                    }
                    throw new InvalidCastException($"value. {valueType} is not support pow");
                }
            }

            throw new InvalidCastException($"pow. {pow.valueType} is not support pow");
        }
        
        public bool Equals(Value other)
        {
            if (valueType == other.valueType)
            {
                switch (valueType)
                {
                    case ValueType.Bool:
                        return var.boolValue == other.var.boolValue;
                    case ValueType.Int:
                        return var.intValue == other.var.intValue;
                    case ValueType.Float:
                        return System.Math.Abs(var.floatValue - other.var.floatValue) < 0.00001f;
                }
            }
            
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Value other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)valueType;
                return (hashCode * 397) ^ var.intValue.GetHashCode();
            }
        }
    }
}