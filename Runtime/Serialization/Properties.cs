using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vena
{
    public interface IPropertyOwner
    {
        int propertyCount { get; }

        IProperty CreateProperty(int propertyId);
    }

    /// <summary>
    /// 属性集合
    /// </summary>
    public sealed class Properties : IBlackboard
    {
        const long HEAD_FLAG = 0x5A5A5A5A;

        const long TAIL_FLAG = 0xA5A5A5A5;

        private readonly IProperty[] _properties;

        public readonly int propertyCount;

        public ulong dirtyMask;

        public readonly IPropertyOwner owner;

        public Properties(IPropertyOwner owner)
        {
            this.owner = owner;

            propertyCount = owner.propertyCount;

            _properties = new IProperty[propertyCount];

            for (var i = 0; i < propertyCount; i++)
            {
                _properties[i] = owner.CreateProperty(i);
            }

            dirtyMask = 0UL;
        }

        public void Initialize(ref ISerializer reader)
        {
            dirtyMask = 0UL;

            for (var i = 0; i < _properties.Length; i++)
            {
                ref var property = ref _properties[i];

                property.Initialize(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<TValue>(in Variable variable, in TValue value) where TValue : struct, IValue<TValue>
        {
            bool changed = ((Property<TValue>)_properties[variable.propertyId]).Set(variable.index, value);
            if (changed)
            {
                dirtyMask |= 1UL << variable.propertyId;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly TValue Get<TValue>(in Variable variable) where TValue : struct, IValue<TValue>
        {
            return ref ((Property<TValue>)_properties[variable.propertyId]).Get(variable.index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Modify(in Variable variable, IExpression expression)
        {
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IProperty GetProperty(int propetyIndex)
        {
            return _properties[propetyIndex];
        }

        public void CopyFrom(Properties other, in Variable variable)
        {
            ref var srcProperty = ref other._properties[variable.propertyId];

            ref var dstProperty = ref _properties[variable.propertyId];

            dstProperty.CopyFrom(srcProperty, variable.index);

            // force update index dirty mask
            dstProperty.dirtyMask |= 1UL << variable.index;

            // force update property dirty mask
            dirtyMask |= 1UL << variable.propertyId;
        }

        public IProperty Clone(int propertyId)
        {
            return _properties[propertyId].Clone();
        }

        public void DeserializeBuffer(ref ISerializer reader)
        {
            // read head flag
            long headFlag = reader.ReadInt64();

            if (headFlag != HEAD_FLAG)
            {
                throw new Exception("DeserializeBuffer head flag error");
            }

            // read dirty property count
            int dirtyCount = reader.ReadInt32();

            for (int i = 0; i < dirtyCount; i++)
            {
                int propertyId = reader.ReadInt32();

                ref var array = ref _properties[propertyId];

                ulong mask = reader.ReadUInt64();

                for (var index = 0; index < array.count; index++)
                {
                    if ((mask & (1UL << index)) != 0UL)
                    {
                        array.Deserialize(ref reader, index);
                    }
                }
            }

            // read tail flag
            long tailFlag = reader.ReadInt64();

            if (tailFlag != TAIL_FLAG)
            {
                throw new Exception("DeserializeBuffer tail flag error");
            }
        }

        public void SerializeBuffer(ref ISerializer writer)
        {
            // write head flag
            writer.WriteInt64(HEAD_FLAG);

            int dirtyCount = 0;

            for (var propertyId = 0; propertyId < _properties.Length; propertyId++)
            {
                ref var array = ref _properties[propertyId];

                ulong dirtyMask = array.dirtyMask;

                if (dirtyMask != 0ul) dirtyCount++;
            }

            // write dirty property count
            writer.WriteInt32(dirtyCount);

            if (dirtyCount > 0)
            {
                for (var propertyId = 0; propertyId < _properties.Length; propertyId++)
                {
                    ref var array = ref _properties[propertyId];

                    ulong dirtyMask = array.dirtyMask;

                    if (dirtyMask == 0ul) continue;

                    writer.WriteInt32(propertyId);

                    writer.WriteUInt64(dirtyMask);

                    for (var index = 0; index < array.count; index++)
                    {
                        if ((dirtyMask & (1uL << index)) != 0UL)
                        {
                            array.Serialize(ref writer, index);
                        }
                    }

                    array.dirtyMask = 0UL;
                }
            }

            // write tail flag
            writer.WriteInt64(TAIL_FLAG);
        }

        public void ResetDirtyMask()
        {
            dirtyMask = 0UL;

            for (var i = 0; i < _properties.Length; i++)
            {
                _properties[i].dirtyMask = 0UL;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Variable : IEquatable<Variable>
    {
        [FieldOffset(0)] public readonly short propertyId;

        [FieldOffset(2)] public readonly short index;

        [FieldOffset(0)] public readonly int value;

        public Variable(int value)
        {
            propertyId = 0;

            index = 0;

            this.value = value;
        }

        public Variable(int propertyId, int index)
        {
            value = 0;

            this.propertyId = (short)propertyId;

            this.index = (short)index;
        }

        public bool Equals(Variable other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is Variable other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}