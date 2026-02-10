using System;
using System.Runtime.CompilerServices;

namespace Vena
{
    /// <summary>
    /// value set 
    /// </summary>
    public interface IProperty
    {
        ValueType valueType { get; }

        int count { get; }

        ulong dirtyMask { get; set; }

        void Initialize(ref ISerializer reader);

        void Modify(int index, IExpression expression, IBlackboard blackboard);

        void Serialize(ref ISerializer writer, int index);

        void Deserialize(ref ISerializer reader, int index);

        void CopyFrom(IProperty from, int index);

        IProperty Clone();
    }

    public abstract class Property<TValue> : IProperty where TValue : struct, IValue<TValue>
    {
        public static readonly TValue DefaultValue = default;

        public ValueType valueType => Value<TValue>.TypeId;

        public abstract ulong dirtyMask { get; set; }

        public abstract int count { get; }

        public abstract ref readonly TValue Get(int id);

        public abstract bool Set(int id, in TValue value);

        public abstract void Initialize(ref ISerializer reader);

        public void Modify(int index, IExpression expression, IBlackboard blackboard)
        {
            Set(index, ((IExpression<TValue>)expression).Evaluate(ref blackboard));
        }

        public abstract void Serialize(ref ISerializer writer, int index);

        public abstract void Deserialize(ref ISerializer reader, int index);

        public abstract void CopyFrom(IProperty from, int index);

        public abstract IProperty Clone();
    }

    /// <summary>
    /// 同步属性集
    /// </summary>
    /// <typeparam name="TIndex"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Property<TIndex, TValue> : Property<TValue>
        where TIndex : struct, Enum
        where TValue : struct, IValue<TValue>
    {
        public static readonly int Length;

        static Property()
        {
            Length = Enum.GetValues(typeof(TIndex)).Length;

            if (Length > 64)
            {
                throw new Exception($"{typeof(TIndex)} element count can't be greater than 64");
            }
        }

        private ulong _dirtyMask;

        private readonly TValue[] _values = new TValue[Length];

        public override int count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Length;
        }

        public override ulong dirtyMask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _dirtyMask;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _dirtyMask = value;
        }

        public Property()
        {
            for (var i = 0; i < Length; i++)
            {
                _values[i] = DefaultValue;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Initialize(ref ISerializer reader)
        {
            for (var i = 0; i < _values.Length; i++)
            {
                bool hasValue = reader.ReadBoolean();
                if (hasValue)
                {
                    ref var value = ref _values[i];
                    value.Deserialize(ref reader);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Serialize(ref ISerializer writer, int index)
        {
            ref var value = ref _values[index];

            value.Serialize(ref writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Deserialize(ref ISerializer reader, int index)
        {
            ref var value = ref _values[index];

            value.Deserialize(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ref readonly TValue Get(int id)
        {
            return ref _values[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Set(int id, in TValue value)
        {
            ref var oldValue = ref _values[id];

            if (!oldValue.Equals(value))
            {
                oldValue = value;

                _dirtyMask |= 1uL << id;

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyFrom(IProperty from, int index)
        {
            if (from is Property<TIndex, TValue> property)
            {
                _values[index] = property._values[index];
                return;
            }

            throw new Exception("CopyFrom error");
        }

        public override IProperty Clone()
        {
            var property = new Property<TIndex, TValue>();

            for (var i = 0; i < Length; i++)
            {
                property._values[i] = _values[i];
            }

            return property;
        }
    }

    public sealed class FlagProperty<TIndex> : Property<Bool> where TIndex : struct, Enum
    {
        public static readonly int Length;
        private static Bool _default = false;

        static FlagProperty()
        {
            Length = Enum.GetValues(typeof(TIndex)).Length;

            if (Length > 64)
            {
                throw new Exception($"{typeof(TIndex)} element count can't be greater than 64");
            }
        }

        private ulong _bit;

        public override int count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Length;
        }

        public override ulong dirtyMask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ref readonly Bool Get(int index)
        {
            _default = (_bit & (1uL << index)) != 0uL;
            return ref _default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Set(int index, in Bool value)
        {
            bool oldValue = (_bit & (1uL << index)) != 0uL;

            if (oldValue != value)
            {
                if (value)
                {
                    _bit |= 1uL << index;
                }
                else
                {
                    _bit &= ~(1uL << index);
                }

                dirtyMask |= 1uL << index;

                return true;
            }

            return false;
        }

        public override void Initialize(ref ISerializer reader)
        {
            _bit = reader.ReadUInt64();

            dirtyMask = 0xFFFFFFFFFFFFFFFF;
        }

        public override void Serialize(ref ISerializer writer, int index)
        {
            if (index == 0)
            {
                writer.WriteUInt64(_bit);
            }
        }

        public override void Deserialize(ref ISerializer reader, int index)
        {
            if (index == 0)
            {
                _bit = reader.ReadUInt64();
            }
        }

        public override void CopyFrom(IProperty from, int index)
        {
            if (from is FlagProperty<TIndex> property)
            {
                bool value = property.Get(index);
                Set(index, value);
                return;
            }

            throw new Exception("CopyFrom error");
        }

        public override IProperty Clone()
        {
            var property = new FlagProperty<TIndex>();

            property._bit = _bit;

            return property;
        }
    }
}