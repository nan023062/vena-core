using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace XDTGame.Core;

/// <summary>
/// object in the world, such as actor, item, etc.
/// for define UniqueId
/// </summary>
public abstract class WorldObject
{
    private static uint _guidAlloc;
    
    private readonly uint _guid;
    
    internal int _version;
    
    /// <summary>
    /// UniqueId
    /// </summary>
    /// <exception cref="InvalidOperationException">if the object is destroyed</exception>
    public uint Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _version > 0 ? _guid : throw new InvalidOperationException("WorldObject has been destroyed");
    }
    
    /// <summary>
    /// if the object is destroyed , return false
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(WorldObject obj) => obj is { _version: > 0 };
    
    protected internal WorldObject()
    {
        Checker.ThrowIsUnSafe(CallFunc.Create);
        
        _guid = ++_guidAlloc;

        _version = 0;
        
        // if _version is overflow, reset to 0
        if( _guidAlloc >= uint.MaxValue)
        {
            _guidAlloc = 0;
        }
    }

    public override string ToString()
    {
        return $"worldObject:{_guid}";
    }
    
    public abstract void CopyTo(WorldObject target);
    
    private static List<FieldInfo> GetAllInstanceFields(Type type)
    {
        var fields = new List<FieldInfo>();
        
        while (type != null && type != typeof(WorldObject))
        {
            FieldInfo[] infos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (var fieldInfo in infos)
            {
                // 且未定义IgnoreCloneAttribute
                if( fieldInfo.GetCustomAttribute<SerializeField>() == null)
                {
                    fields.Add(fieldInfo);
                }
            }
            
            type = type.BaseType;
        }
        return fields;
    }

    
    internal enum CallFunc : byte
    {
        Default,
        Create,
        Destroy,
    }
    
    private static CallFunc _callFunc;
    
    internal static readonly object __lock__ = new object();
    
    internal readonly struct Checker : IDisposable
    {
        public Checker(CallFunc type)
        {
            if (_callFunc != CallFunc.Default)
            {
                throw new Exception($"WorldObject.Checker is locked by other {_callFunc}");
            }
            _callFunc = type;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIsUnSafe(CallFunc type)
        {
            if (_callFunc != type)
            {
                throw new InvalidOperationException("WorldObject can only be created or destroyed by World");
            }
        }
        
        public void Dispose()
        {
            // TODO release managed resources here
            _callFunc = CallFunc.Default;
        }
    }
}


/// <summary>
/// Bind WorldObject type to WorldObjectSource type
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ObjectSerializableAttribute : Attribute
{
    public readonly Type objectType;

    public ObjectSerializableAttribute(Type objectType)
    {
        this.objectType = objectType;
    }
}

public interface IObjectSource
{
    void Serialize(WorldObject worldObject);
    
    void Deserialize(WorldObject worldObject);
}

/// <summary>
/// ActorSource is an abstract class that represents a source of Actor.
/// </summary>
public abstract class ActorSource : IObjectSource
{
    public Actor Instatiate(World world)
    {
        ObjectSerializableAttribute attribute = GetType().GetCustomAttribute<ObjectSerializableAttribute>();

        if (attribute == null)
        {
            throw new InvalidOperationException($"ObjectBindingAttribute is not defined for {GetType().Name}");
        }

        Type objectType = attribute.objectType;

        if (objectType.IsSubclassOf(typeof(Actor)))
        {
            Actor actor = world.CreateActor(attribute.objectType);
            
            DeserializeInternal( actor);
            
            return actor;
        }
        
        throw new InvalidOperationException($"Unsupported Actor type: {objectType.Name}");
    }
    
    public T Instatiate<T>(World world) where T : Actor
    {
        var worldObject = Instatiate(world);
        
        if (worldObject is T typedObj)
        {
            return typedObj;
        }
        throw new InvalidOperationException($"Instatiate failed, expected type {typeof(T)}, but got {worldObject.GetType()}");
    }
    
    protected abstract void SerializeInternal(Actor actor);
    
    protected abstract void DeserializeInternal(Actor actor);
    
    public void Serialize(WorldObject worldObject)
    {
        if( worldObject is Actor actor)
        {
            SerializeInternal(actor);
            return;
        }
        
        throw new InvalidOperationException($"SerializeInternal failed, expected type Actor, but got {worldObject.GetType()}");
    } 

    public void Deserialize(WorldObject worldObject)
    {
        if( worldObject is Actor actor)
        {
            DeserializeInternal(actor);
            return;
        }
        
        throw new InvalidOperationException($"DeserializeInternal failed, expected type Actor, but got {worldObject.GetType()}");
    }
}

/// <summary>
/// ComponentSource is an abstract class that represents a source of Component.
/// </summary>
public abstract class ComponentSource : IObjectSource
{
    public Component Instatiate(Actor actor)
    {
        ObjectSerializableAttribute attribute = GetType().GetCustomAttribute<ObjectSerializableAttribute>();

        if (attribute == null)
        {
            throw new InvalidOperationException($"ObjectBindingAttribute is not defined for {GetType().Name}");
        }

        Type objectType = attribute.objectType;

        if (objectType.IsSubclassOf(typeof(Component)))
        {
            Component component = actor.AddComponent(attribute.objectType);
            
            DeserializeInternal( component);
            
            return component;
        }

        throw new InvalidOperationException($"Unsupported Component type: {objectType.Name}");
    }
    
    public T Instatiate<T>(Actor actor) where T : Component
    {
        var component = Instatiate(actor);
        
        if (component is T typedObj)
        {
            return typedObj;
        }
        throw new InvalidOperationException($"CreateInstance failed, expected type {typeof(T)}, but got {component.GetType()}");
    }
    
    protected abstract void SerializeInternal(Component component);
    
    protected abstract void DeserializeInternal(Component component);
    
    public void Serialize(WorldObject worldObject)
    {
        if (worldObject is Component component)
        {
            SerializeInternal(component);
            return;
        }
        throw new InvalidOperationException($"SerializeInternal failed, expected type Actor, but got {worldObject.GetType()}");
    } 

    public void Deserialize(WorldObject worldObject)
    {
        if (worldObject is Component component)
        {
            DeserializeInternal(component);
            return;
        }
        throw new InvalidOperationException($"DeserializeInternal failed, expected type Actor, but got {worldObject.GetType()}");
    }
}


public static class WorldObjectExtensions
{
    /// <summary>
    /// Serialize Actor to ActorSource
    /// </summary>
    /// <param name="actor"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T SerializeToSource<T>(this Actor actor) where T : ActorSource, new()
    {
        ObjectSerializableAttribute attribute = typeof(T).GetCustomAttribute<ObjectSerializableAttribute>();
        if (attribute == null)
        {
            throw new InvalidOperationException($"ObjectSerializableAttribute is not defined for {typeof(T).Name}");
        }

        Type sourceType = attribute.objectType;
        if (sourceType == actor.GetType())
        {
             T source = new T();
             source.Serialize(actor);
             return  source;
        }
        throw new InvalidOperationException($"Unsupported Actor type: {sourceType.Name}");
    }
    
    /// <summary>
    /// Serialize Component to ComponentSource
    /// </summary>
    /// <param name="component"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T SerializeToSource<T>(this Component component) where T : ComponentSource, new()
    {
        ObjectSerializableAttribute attribute = typeof(T).GetCustomAttribute<ObjectSerializableAttribute>();
        if (attribute == null)
        {
            throw new InvalidOperationException($"ObjectSerializableAttribute is not defined for {typeof(T).Name}");
        }

        Type sourceType = attribute.objectType;
        if (sourceType == component.GetType())
        {
            T source = new T();
            source.Serialize(component);
            return  source;
        }
        throw new InvalidOperationException($"Unsupported Component type: {sourceType.Name}");
    }
}