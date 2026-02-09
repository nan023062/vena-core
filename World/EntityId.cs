using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XDTGame.World;

/// <summary>
/// entity id
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct EntityId : IEquatable<EntityId>
{
    public static readonly EntityId Invalid = new EntityId(0);
    
    public readonly uint value;
        
    public EntityId(uint value)
    {
        this.value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(EntityId lhs, EntityId rhs)
    {
        return lhs.value == rhs.value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(EntityId lhs, EntityId rhs)
    {
        return lhs.value != rhs.value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(EntityId lhs, EntityId rhs)
    {
        return lhs.value < rhs.value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(EntityId lhs, EntityId rhs)
    {
        return lhs.value > rhs.value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(EntityId lhs, EntityId rhs)
    {
        return lhs.value <= rhs.value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(EntityId lhs, EntityId rhs)
    {
        return lhs.value >= rhs.value;
    }
        
    public bool Equals(EntityId other)
    {
        return value == other.value;
    }

    public override bool Equals(object obj)
    {
        return obj is EntityId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override string ToString()
    {
        return value.ToString();
    }
}
