using System;
using System.Runtime.CompilerServices;
using Vena.Math;

namespace Vena
{
    public abstract class Value
    {
        protected static byte _typeId;

        static Value()
        {
            // Register all value types here
            _ = Value<Bool>.TypeId;
            _ = Value<Int>.TypeId;
            _ = Value<Int2>.TypeId;
            _ = Value<Int3>.TypeId;
            _ = Value<UInt>.TypeId;
            _ = Value<Long>.TypeId;
            _ = Value<ULong>.TypeId;
            _ = Value<Float>.TypeId;
            _ = Value<Float2>.TypeId;
            _ = Value<Float3>.TypeId;
            _ = Value<Float4>.TypeId;
        }

        public abstract ValueType Type { get; }
    }

    public sealed class Value<T> : Value where T : IValue<T>
    {
        public static readonly ValueType TypeId;
        public static readonly T Default = default;

        static Value()
        {
            _typeId++;

            if (_typeId <= (byte)ValueType.Invalid || _typeId >= (byte)ValueType.OutofRange)
            {
                throw new Exception($"Value<{typeof(T).Name}> typeId out of range");
            }

            TypeId = (ValueType)_typeId;
        }

        public T value;

        public override ValueType Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TypeId;
        }
    }

    public enum ValueType : byte
    {
        Invalid = 0,

        Bool = 1,
        Int = 2,
        Int2 = 3,
        Int3 = 4,
        UInt = 5,
        Long = 6,
        ULong = 7,
        Float = 8,
        Float2 = 9,
        Float3 = 10,
        Float4 = 11,

        OutofRange
    }

    /// <summary>
    /// Value interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValue<T> : IEquatable<T>, IDisposable
    {
        void Serialize(ref ISerializer writer);

        void Deserialize(ref ISerializer reader);

        string ToCode();
    }

    public struct Bool : IValue<Bool>, IBoolean<Bool>
    {
        private bool _value;

        public string ToCode()
        {
            return _value ? "true" : "false";
        }

        public Bool(in bool value)
        {
            _value = value;
        }

        public bool Equals(Bool other)
        {
            return _value == other._value;
        }

        public static implicit operator bool(in Bool value)
        {
            return value._value;
        }

        public static implicit operator Bool(in bool value)
        {
            return new Bool(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteBoolean(_value);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value = reader.ReadBoolean();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Yes()
        {
            return _value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Not()
        {
            return !_value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool And(in Bool rhs)
        {
            return _value && rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Or(in Bool rhs)
        {
            return _value || rhs._value;
        }
    }

    public struct Int : IValue<Int>, ICompare<Int>, IArithmetic<Int>
    {
        private int _value;

        public string ToCode()
        {
            return _value.ToString();
        }

        public Int(in int value)
        {
            _value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Int other)
        {
            return _value == other._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in Int value)
        {
            return value._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Int(in int value)
        {
            return new Int(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteInt32(_value);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value = reader.ReadInt32();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Int rhs)
        {
            return _value == rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Int rhs)
        {
            return _value != rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Greater(in Int rhs)
        {
            return _value > rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool GreaterEqual(in Int rhs)
        {
            return _value >= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Less(in Int rhs)
        {
            return _value < rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool LessEqual(in Int rhs)
        {
            return _value <= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Abs()
        {
            return new Int(MathHelper.Abs(_value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Negate()
        {
            return new Int(-_value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Add(in Int rhs)
        {
            return new Int(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Subtract(in Int rhs)
        {
            return new Int(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Multiply(in Int rhs)
        {
            return new Int(_value * rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Divide(in Int rhs)
        {
            return new Int(_value / rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Mod(in Int rhs)
        {
            return new Int(_value % rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Min(in Int rhs)
        {
            return new Int(MathHelper.Min(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Max(in Int rhs)
        {
            return new Int(MathHelper.Max(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Mid(in Int rhs)
        {
            return new Int((_value + rhs._value) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int Clamp(in Int min, in Int max)
        {
            return new Int(MathHelper.Min(MathHelper.Max(_value, min._value), max._value));
        }
    }

    public struct Int2 : IValue<Int2>, IEquat<Int2>, IArithmetic<Int2>
    {
        private Vector2Int _value;

        public string ToCode()
        {
            return $"(new Int2({_value.X}, {_value.Y}))";
        }

        public Int2(int x = 0, int y = 0)
        {
            _value = new Vector2Int(x, y);
        }

        public Int2(in Vector2Int value)
        {
            _value = value;
        }

        public bool Equals(Int2 other)
        {
            return _value == other._value;
        }

        public static implicit operator Vector2Int(in Int2 value)
        {
            return value._value;
        }

        public static implicit operator Int2(in Vector2Int value)
        {
            return new Int2(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteInt32(_value.X);
            writer.WriteInt32(_value.Y);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value.X = reader.ReadInt32();
            _value.Y = reader.ReadInt32();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Int2 rhs)
        {
            return _value == rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Int2 rhs)
        {
            return _value != rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Abs()
        {
            return new Int2(new Vector2Int(MathHelper.Abs(_value.X), MathHelper.Abs(_value.Y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Negate()
        {
            return new Int2(new Vector2Int(-_value.X, -_value.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Add(in Int2 rhs)
        {
            return new Int2(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Subtract(in Int2 rhs)
        {
            return new Int2(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Multiply(in Int2 rhs)
        {
            return new Int2(_value * rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Divide(in Int2 rhs)
        {
            return new Int2(new Vector2Int(_value.X / rhs._value.X, _value.Y / rhs._value.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Mod(in Int2 rhs)
        {
            return new Int2(new Vector2Int(_value.X % rhs._value.X, _value.Y % rhs._value.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Min(in Int2 rhs)
        {
            return new Int2(new Vector2Int(MathHelper.Min(_value.X, rhs._value.X),
                MathHelper.Min(_value.Y, rhs._value.Y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Max(in Int2 rhs)
        {
            return new Int2(new Vector2Int(MathHelper.Max(_value.X, rhs._value.X),
                MathHelper.Max(_value.Y, rhs._value.Y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Mid(in Int2 rhs)
        {
            return new Int2(new Vector2Int((_value.X + rhs._value.X) / 2, (_value.Y + rhs._value.Y) / 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int2 Clamp(in Int2 min, in Int2 max)
        {
            var x = MathHelper.Min(MathHelper.Max(_value.X, min._value.X), max._value.X);
            var y = MathHelper.Min(MathHelper.Max(_value.Y, min._value.Y), max._value.Y);
            return new Int2(new Vector2Int(x, y));
        }
    }

    public struct Int3 : IValue<Int3>, IEquat<Int3>, IArithmetic<Int3>
    {
        private Vector3Int _value;

        public string ToCode()
        {
            return $"(new Int3({_value.X}, {_value.Y}, {_value.Z}))";
        }

        public Int3(int x = 0, int y = 0, int z = 0)
        {
            _value = new Vector3Int(x, y, z);
        }

        public Int3(in Vector3Int value)
        {
            _value = value;
        }

        public bool Equals(Int3 other)
        {
            return _value == other._value;
        }

        public static implicit operator Vector3Int(in Int3 value)
        {
            return value._value;
        }

        public static implicit operator Int3(in Vector3Int value)
        {
            return new Int3(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteInt32(_value.X);
            writer.WriteInt32(_value.Y);
            writer.WriteInt32(_value.Z);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value.X = reader.ReadInt32();
            _value.Y = reader.ReadInt32();
            _value.Z = reader.ReadInt32();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Int3 rhs)
        {
            return _value == rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Int3 rhs)
        {
            return _value != rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Abs()
        {
            return new Int3(new Vector3Int(MathHelper.Abs(_value.X), MathHelper.Abs(_value.Y),
                MathHelper.Abs(_value.Z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Negate()
        {
            return new Int3(new Vector3Int(-_value.X, -_value.Y, -_value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Add(in Int3 rhs)
        {
            return new Int3(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Subtract(in Int3 rhs)
        {
            return new Int3(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Multiply(in Int3 rhs)
        {
            return new Int3(_value * rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Divide(in Int3 rhs)
        {
            return new Int3(new Vector3Int(_value.X / rhs._value.X, _value.Y / rhs._value.Y, _value.Z / rhs._value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Mod(in Int3 rhs)
        {
            return new Int3(new Vector3Int(_value.X % rhs._value.X, _value.Y % rhs._value.Y, _value.Z % rhs._value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Min(in Int3 rhs)
        {
            return new Int3(new Vector3Int(MathHelper.Min(_value.X, rhs._value.X),
                MathHelper.Min(_value.Y, rhs._value.Y), MathHelper.Min(_value.Z, rhs._value.Z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Max(in Int3 rhs)
        {
            return new Int3(new Vector3Int(MathHelper.Max(_value.X, rhs._value.X),
                MathHelper.Max(_value.Y, rhs._value.Y), MathHelper.Max(_value.Z, rhs._value.Z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Mid(in Int3 rhs)
        {
            return new Int3(new Vector3Int((_value.X + rhs._value.X) / 2, (_value.Y + rhs._value.Y) / 2,
                (_value.Z + rhs._value.Z) / 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int3 Clamp(in Int3 min, in Int3 max)
        {
            var x = MathHelper.Min(MathHelper.Max(_value.X, min._value.X), max._value.X);
            var y = MathHelper.Min(MathHelper.Max(_value.Y, min._value.Y), max._value.Y);
            var z = MathHelper.Min(MathHelper.Max(_value.Z, min._value.Z), max._value.Z);
            return new Int3(new Vector3Int(x, y, z));
        }
    }

    public struct UInt : IValue<UInt>, ICompare<UInt>, IArithmetic<UInt>
    {
        private uint _value;

        public string ToCode()
        {
            return _value.ToString();
        }

        public UInt(in uint value)
        {
            _value = value;
        }

        public bool Equals(UInt other)
        {
            return _value == other._value;
        }

        public static implicit operator uint(in UInt value)
        {
            return value._value;
        }

        public static implicit operator UInt(in uint value)
        {
            return new UInt(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteUInt32(_value);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value = reader.ReadUInt32();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in UInt rhs)
        {
            return _value == rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in UInt rhs)
        {
            return _value != rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Greater(in UInt rhs)
        {
            return _value > rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool GreaterEqual(in UInt rhs)
        {
            return _value >= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Less(in UInt rhs)
        {
            return _value < rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool LessEqual(in UInt rhs)
        {
            return _value <= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Abs()
        {
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Negate()
        {
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Add(in UInt rhs)
        {
            return new UInt(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Subtract(in UInt rhs)
        {
            return new UInt(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Multiply(in UInt rhs)
        {
            return new UInt(_value * rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Divide(in UInt rhs)
        {
            return new UInt(_value / rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Mod(in UInt rhs)
        {
            return new UInt(_value % rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Min(in UInt rhs)
        {
            return new UInt(MathHelper.Min(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Max(in UInt rhs)
        {
            return new UInt(MathHelper.Max(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Mid(in UInt rhs)
        {
            return new UInt((_value + rhs._value) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt Clamp(in UInt min, in UInt max)
        {
            return new UInt(MathHelper.Min(MathHelper.Max(_value, min._value), max._value));
        }
    }

    public struct Long : IValue<Long>, ICompare<Long>, IArithmetic<Long>
    {
        private long _value;

        public string ToCode()
        {
            return _value.ToString();
        }

        public Long(in long value)
        {
            _value = value;
        }

        public bool Equals(Long other)
        {
            return _value == other._value;
        }

        public static implicit operator long(in Long value)
        {
            return value._value;
        }

        public static implicit operator Long(in long value)
        {
            return new Long(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteInt64(_value);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value = reader.ReadInt64();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Long rhs)
        {
            return _value == rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Long rhs)
        {
            return _value != rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Greater(in Long rhs)
        {
            return _value > rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool GreaterEqual(in Long rhs)
        {
            return _value >= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Less(in Long rhs)
        {
            return _value < rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool LessEqual(in Long rhs)
        {
            return _value <= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Abs()
        {
            return new Long(MathHelper.Abs(_value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Negate()
        {
            return new Long(-_value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Add(in Long rhs)
        {
            return new Long(_value + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Subtract(in Long rhs)
        {
            return new Long(_value - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Multiply(in Long rhs)
        {
            return new Long(_value * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Divide(in Long rhs)
        {
            return new Long(_value / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Mod(in Long rhs)
        {
            return new Long(_value % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Min(in Long rhs)
        {
            return new Long(MathHelper.Min(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Max(in Long rhs)
        {
            return new Long(MathHelper.Max(_value, rhs));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Mid(in Long rhs)
        {
            return new Long((_value + rhs) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Long Clamp(in Long min, in Long max)
        {
            return new Long(MathHelper.Min(MathHelper.Max(_value, min), max));
        }
    }

    public struct ULong : IValue<ULong>, ICompare<ULong>, IArithmetic<ULong>
    {
        private ulong _value;

        public string ToCode()
        {
            return _value.ToString();
        }

        public ULong(in ulong value)
        {
            _value = value;
        }

        public bool Equals(ULong other)
        {
            return _value == other._value;
        }

        public static implicit operator ulong(in ULong value)
        {
            return value._value;
        }

        public static implicit operator ULong(in ulong value)
        {
            return new ULong(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteUInt64(_value);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value = reader.ReadUInt64();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in ULong rhs)
        {
            return _value == rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in ULong rhs)
        {
            return _value != rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Greater(in ULong rhs)
        {
            return _value > rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool GreaterEqual(in ULong rhs)
        {
            return _value >= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Less(in ULong rhs)
        {
            return _value < rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool LessEqual(in ULong rhs)
        {
            return _value <= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Abs()
        {
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Negate()
        {
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Add(in ULong rhs)
        {
            return new ULong(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Subtract(in ULong rhs)
        {
            return new ULong(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Multiply(in ULong rhs)
        {
            return new ULong(_value * rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Divide(in ULong rhs)
        {
            return new ULong(_value / rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Mod(in ULong rhs)
        {
            return new ULong(_value % rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Min(in ULong rhs)
        {
            return new ULong(MathHelper.Min(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Max(in ULong rhs)
        {
            return new ULong(MathHelper.Max(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Mid(in ULong rhs)
        {
            return new ULong((_value + rhs._value) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ULong Clamp(in ULong min, in ULong max)
        {
            return new ULong(MathHelper.Min(MathHelper.Max(_value, min._value), max._value));
        }
    }

    public struct Float : IValue<Float>, ICompare<Float>, IArithmetic<Float>
    {
        public const float Epsilon = 0.0005f;

        private float _value;

        public string ToCode()
        {
            return $"{_value}f";
        }

        public Float(in float value)
        {
            _value = value;
        }

        public bool Equals(Float other)
        {
            return MathHelper.Abs(_value - other._value) < Epsilon;
        }

        public static implicit operator float(in Float value)
        {
            return value._value;
        }

        public static implicit operator Float(in float value)
        {
            return new Float(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteSingle(_value);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value = reader.ReadSingle();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Float rhs)
        {
            return MathHelper.Abs(_value - rhs._value) < Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Float rhs)
        {
            return MathHelper.Abs(_value - rhs._value) >= Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Greater(in Float rhs)
        {
            return _value > rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool GreaterEqual(in Float rhs)
        {
            return _value >= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Less(in Float rhs)
        {
            return _value < rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool LessEqual(in Float rhs)
        {
            return _value <= rhs._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Abs()
        {
            return new Float(MathHelper.Abs(_value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Negate()
        {
            return new Float(-_value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Add(in Float rhs)
        {
            return new Float(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Subtract(in Float rhs)
        {
            return new Float(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Multiply(in Float rhs)
        {
            return new Float(_value * rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Divide(in Float rhs)
        {
            return new Float(_value / rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Mod(in Float rhs)
        {
            return new Float(_value % rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Min(in Float rhs)
        {
            return new Float(MathHelper.Min(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Max(in Float rhs)
        {
            return new Float(MathHelper.Max(_value, rhs._value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Mid(in Float rhs)
        {
            return new Float((_value + rhs._value) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float Clamp(in Float min, in Float max)
        {
            return new Float(MathHelper.Min(MathHelper.Max(_value, min._value), max._value));
        }
    }

    public struct Float2 : IValue<Float2>, IEquat<Float2>, IArithmetic<Float2>
    {
        private Vector2 _value;

        public string ToCode()
        {
            return $"(new Float2({_value.X}f, {_value.Y}f))";
        }

        public Float2(float x = 0, float y = 0)
        {
            _value = new Vector2(x, y);
        }

        public Float2(in Vector2 value)
        {
            _value = value;
        }

        public bool Equals(Float2 other)
        {
            return _value == other._value;
        }

        public static implicit operator Vector2(in Float2 value)
        {
            return value._value;
        }

        public static implicit operator Float2(in Vector2 value)
        {
            return new Float2(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteSingle(_value.X);
            writer.WriteSingle(_value.Y);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value.X = reader.ReadSingle();
            _value.Y = reader.ReadSingle();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Float2 rhs)
        {
            return MathHelper.Abs(_value.X - rhs._value.X) < Float.Epsilon &&
                   MathHelper.Abs(_value.Y - rhs._value.Y) < Float.Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Float2 rhs)
        {
            return MathHelper.Abs(_value.X - rhs._value.X) >= Float.Epsilon ||
                   MathHelper.Abs(_value.Y - rhs._value.Y) >= Float.Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Abs()
        {
            return new Float2(new Vector2(MathHelper.Abs(_value.X), MathHelper.Abs(_value.Y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Negate()
        {
            return new Float2(new Vector2(-_value.X, -_value.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Add(in Float2 rhs)
        {
            return new Float2(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Subtract(in Float2 rhs)
        {
            return new Float2(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Multiply(in Float2 rhs)
        {
            return new Float2(_value * rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Divide(in Float2 rhs)
        {
            return new Float2(new Vector2(_value.X / rhs._value.X, _value.Y / rhs._value.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Mod(in Float2 rhs)
        {
            return new Float2(new Vector2(_value.X % rhs._value.X, _value.Y % rhs._value.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Min(in Float2 rhs)
        {
            return new Float2(new Vector2(MathHelper.Min(_value.X, rhs._value.X),
                MathHelper.Min(_value.Y, rhs._value.Y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Max(in Float2 rhs)
        {
            return new Float2(new Vector2(MathHelper.Max(_value.X, rhs._value.X),
                MathHelper.Max(_value.Y, rhs._value.Y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Mid(in Float2 rhs)
        {
            return new Float2(new Vector2((_value.X + rhs._value.X) / 2, (_value.Y + rhs._value.Y) / 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float2 Clamp(in Float2 min, in Float2 max)
        {
            var x = MathHelper.Min(MathHelper.Max(_value.X, min._value.X), max._value.X);
            var y = MathHelper.Min(MathHelper.Max(_value.Y, min._value.Y), max._value.Y);
            return new Float2(new Vector2(x, y));
        }
    }

    public struct Float3 : IValue<Float3>, IEquat<Float3>, IArithmetic<Float3>
    {
        private Vector3 _value;

        public string ToCode()
        {
            return $"(new Float3({_value.X}f, {_value.Y}f, {_value.Z}f))";
        }

        public Float3(float x = 0, float y = 0, float z = 0)
        {
            _value = new Vector3(x, y, z);
        }

        public Float3(in Vector3 value)
        {
            _value = value;
        }

        public bool Equals(Float3 other)
        {
            return _value == other._value;
        }

        public static implicit operator Vector3(in Float3 value)
        {
            return value._value;
        }

        public static implicit operator Float3(in Vector3 value)
        {
            return new Float3(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteSingle(_value.X);
            writer.WriteSingle(_value.Y);
            writer.WriteSingle(_value.Z);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value.X = reader.ReadSingle();
            _value.Y = reader.ReadSingle();
            _value.Z = reader.ReadSingle();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Float3 rhs)
        {
            return MathHelper.Abs(_value.X - rhs._value.X) < Float.Epsilon &&
                   MathHelper.Abs(_value.Y - rhs._value.Y) < Float.Epsilon &&
                   MathHelper.Abs(_value.Z - rhs._value.Z) < Float.Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Float3 rhs)
        {
            return MathHelper.Abs(_value.X - rhs._value.X) >= Float.Epsilon ||
                   MathHelper.Abs(_value.Y - rhs._value.Y) >= Float.Epsilon ||
                   MathHelper.Abs(_value.Z - rhs._value.Z) >= Float.Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Abs()
        {
            return new Float3(new Vector3(MathHelper.Abs(_value.X), MathHelper.Abs(_value.Y),
                MathHelper.Abs(_value.Z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Negate()
        {
            return new Float3(new Vector3(-_value.X, -_value.Y, -_value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Add(in Float3 rhs)
        {
            return new Float3(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Subtract(in Float3 rhs)
        {
            return new Float3(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Multiply(in Float3 rhs)
        {
            return new Float3(new Vector3(_value.X * rhs._value.X, _value.Y * rhs._value.Y, _value.Z * rhs._value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Divide(in Float3 rhs)
        {
            return new Float3(new Vector3(_value.X / rhs._value.X, _value.Y / rhs._value.Y, _value.Z / rhs._value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Mod(in Float3 rhs)
        {
            return new Float3(new Vector3(_value.X % rhs._value.X, _value.Y % rhs._value.Y, _value.Z % rhs._value.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Min(in Float3 rhs)
        {
            return new Float3(new Vector3(MathHelper.Min(_value.X, rhs._value.X),
                MathHelper.Min(_value.Y, rhs._value.Y), MathHelper.Min(_value.Z, rhs._value.Z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Max(in Float3 rhs)
        {
            return new Float3(new Vector3(MathHelper.Max(_value.X, rhs._value.X),
                MathHelper.Max(_value.Y, rhs._value.Y), MathHelper.Max(_value.Z, rhs._value.Z)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Mid(in Float3 rhs)
        {
            return new Float3(new Vector3((_value.X + rhs._value.X) / 2, (_value.Y + rhs._value.Y) / 2,
                (_value.Z + rhs._value.Z) / 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float3 Clamp(in Float3 min, in Float3 max)
        {
            var x = MathHelper.Min(MathHelper.Max(_value.X, min._value.X), max._value.X);
            var y = MathHelper.Min(MathHelper.Max(_value.Y, min._value.Y), max._value.Y);
            var z = MathHelper.Min(MathHelper.Max(_value.Z, min._value.Z), max._value.Z);
            return new Float3(new Vector3(x, y, z));
        }
    }

    public struct Float4 : IValue<Float4>, IEquat<Float4>, IArithmetic<Float4>
    {
        private Vector4 _value;

        public string ToCode()
        {
            return $"(new Float4({_value.X}f, {_value.Y}f, {_value.Z}f, {_value.W}f))";
        }

        public Float4(float x = 0, float y = 0, float z = 0, float w = 0)
        {
            _value = new Vector4(x, y, z, w);
        }

        public Float4(in Vector4 value)
        {
            _value = value;
        }

        public bool Equals(Float4 other)
        {
            return _value == other._value;
        }

        public static implicit operator Vector4(in Float4 value)
        {
            return value._value;
        }

        public static implicit operator Float4(in Vector4 value)
        {
            return new Float4(value);
        }

        public void Serialize(ref ISerializer writer)
        {
            writer.WriteSingle(_value.X);
            writer.WriteSingle(_value.Y);
            writer.WriteSingle(_value.Z);
            writer.WriteSingle(_value.W);
        }

        public void Deserialize(ref ISerializer reader)
        {
            _value.X = reader.ReadSingle();
            _value.Y = reader.ReadSingle();
            _value.Z = reader.ReadSingle();
            _value.W = reader.ReadSingle();
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool Equal(in Float4 rhs)
        {
            return MathHelper.Abs(_value.X - rhs._value.X) < Float.Epsilon &&
                   MathHelper.Abs(_value.Y - rhs._value.Y) < Float.Epsilon &&
                   MathHelper.Abs(_value.Z - rhs._value.Z) < Float.Epsilon &&
                   MathHelper.Abs(_value.W - rhs._value.W) < Float.Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bool NotEqual(in Float4 rhs)
        {
            return MathHelper.Abs(_value.X - rhs._value.X) >= Float.Epsilon ||
                   MathHelper.Abs(_value.Y - rhs._value.Y) >= Float.Epsilon ||
                   MathHelper.Abs(_value.Z - rhs._value.Z) >= Float.Epsilon ||
                   MathHelper.Abs(_value.W - rhs._value.W) >= Float.Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Abs()
        {
            return new Float4(new Vector4(MathHelper.Abs(_value.X), MathHelper.Abs(_value.Y),
                MathHelper.Abs(_value.Z), MathHelper.Abs(_value.W)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Negate()
        {
            return new Float4(new Vector4(-_value.X, -_value.Y, -_value.Z, -_value.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Add(in Float4 rhs)
        {
            return new Float4(_value + rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Subtract(in Float4 rhs)
        {
            return new Float4(_value - rhs._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Multiply(in Float4 rhs)
        {
            return new Float4(new Vector4(_value.X * rhs._value.X, _value.Y * rhs._value.Y, _value.Z * rhs._value.Z,
                _value.W * rhs._value.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Divide(in Float4 rhs)
        {
            return new Float4(new Vector4(_value.X / rhs._value.X, _value.Y / rhs._value.Y, _value.Z / rhs._value.Z,
                _value.W / rhs._value.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Mod(in Float4 rhs)
        {
            return new Float4(new Vector4(_value.X % rhs._value.X, _value.Y % rhs._value.Y, _value.Z % rhs._value.Z,
                _value.W % rhs._value.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Min(in Float4 rhs)
        {
            return new Float4(new Vector4(MathHelper.Min(_value.X, rhs._value.X),
                MathHelper.Min(_value.Y, rhs._value.Y), MathHelper.Min(_value.Z, rhs._value.Z),
                MathHelper.Min(_value.W, rhs._value.W)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Max(in Float4 rhs)
        {
            return new Float4(new Vector4(MathHelper.Max(_value.X, rhs._value.X),
                MathHelper.Max(_value.Y, rhs._value.Y), MathHelper.Max(_value.Z, rhs._value.Z),
                MathHelper.Max(_value.W, rhs._value.W)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Mid(in Float4 rhs)
        {
            return new Float4(new Vector4((_value.X + rhs._value.X) / 2, (_value.Y + rhs._value.Y) / 2,
                (_value.Z + rhs._value.Z) / 2, (_value.W + rhs._value.W) / 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Float4 Clamp(in Float4 min, in Float4 max)
        {
            var x = MathHelper.Min(MathHelper.Max(_value.X, min._value.X), max._value.X);
            var y = MathHelper.Min(MathHelper.Max(_value.Y, min._value.Y), max._value.Y);
            var z = MathHelper.Min(MathHelper.Max(_value.Z, min._value.Z), max._value.Z);
            var w = MathHelper.Min(MathHelper.Max(_value.W, min._value.W), max._value.W);
            return new Float4(new Vector4(x, y, z, w));
        }
    }
}