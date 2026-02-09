///////////////////////////////////////////////////////////////////////////////////////////////////
// Author      : LiNan
// Description : Systems
// Department  : XDTown Client / Core / World-Actor
///////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XDTGame.Core;

/// <summary>
/// Systems is a container for all systems in the world.
///  - Systems will be initialized in the order they are added.
///  - Systems will be disposed in the reverse order they are added.
///  - Systems will be updated & late updated in the order they are added.
/// </summary>
public sealed class Systems : SystemGroup, IDisposable
{
    public readonly World world;
    private readonly Dictionary<Type, ISystem> _allSystems;
    private readonly Dictionary<Type, object> _injections;
    private bool _initialized;
    
    public T GetSystem<T>() where T : ISystem
    {
        if (_allSystems.TryGetValue(typeof(T), out var system))
        {
            return (T)system;
        }
        return default;
    }
    
    internal Systems(World world)
    {
        this.world = world;
        _allSystems = new Dictionary<Type, ISystem>();
        _injections = new Dictionary<Type, object>()
        {
            { typeof(World), world }
        };
        _initialized = false;
    }
    
    public void InitSystem(Type[] systemTypes)
    {
        if(_initialized)
            throw new Exception("Systems already initialized.");

        foreach (var type in systemTypes)
        {
            if (!_allSystems.ContainsKey(type))
            {
                ISystem system = (ISystem)Activator.CreateInstance(type);
                InitSystem(system);
            }
        }
    }

    public void InitSystem(ISystem system)
    {
        if(_initialized)
            throw new Exception("Systems already initialized.");

        Type type = system.GetType();
        if (!_allSystems.ContainsKey(type))
        {
            _allSystems.Add( type, system );
            Inject(system);
            
            // 反射获取所私有的filter， 且标记了Inject的字段
            foreach (var fieldInfo in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                         .Where(field => field.FieldType.IsSubclassOf(typeof(Filter)) && 
                                         !field.FieldType.IsAbstract && 
                                         field.GetCustomAttribute<InjectAttribute>() != null))
            {
                Filter filter = world.GetFilter(fieldInfo.FieldType);
                Inject(filter, fieldInfo.FieldType);
            }
            
            // Check if the system has SystemGroupAttribute
            var attribute = type.GetCustomAttribute<SystemGroupAttribute>();
            Type groupType = attribute?.GroupType;
            if (groupType != null && groupType.IsSubclassOf(typeof(SystemGroup)))
            {
                if (!_allSystems.TryGetValue(groupType, out ISystem group))
                {
                    group = (ISystem)Activator.CreateInstance(groupType);
                    InitSystem(group);
                }
                
                SystemGroup systemGroup = (SystemGroup)group;
                systemGroup.AddSystem(system);
            }
            else
            {
                AddSystem(system);
            }
        }
    }
    
    public void Inject(object obj, Type overrideType = null)
    {
        if(_initialized)
            throw new Exception("Systems already initialized.");
        
        Type type = overrideType ?? obj.GetType();
        _injections[type] = obj;
    }
    
    public void Init()
    {
        _initialized = true;
        
        // Inject sub system
        foreach (var system in _allSystems.Values)
        {
            Type type = system.GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (_injections.TryGetValue(fieldInfo.FieldType, out var value))
                {
                    fieldInfo.SetValue(system, value);
                }
            }
        }
        
        InternalInit();
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
        InternalDestroy();
    }
}

/// <summary>
/// SystemGroup is a group of systems.
///  - Systems will be initialized in the order they are added.
///  - Systems will be disposed in the reverse order they are added.
///  - Systems will be updated & late updated in the order they are added.
/// </summary>
public class SystemGroup : ISystem
{
    private readonly List<ISystem> _systems = new ();
    private IUpdateSystem[] _updateSystems;
    private ILateUpdateSystem[] _lateUpdateSystems;
    
    internal void AddSystem(ISystem system)
    {
        _systems.Add(system);
    }
    
    protected void InternalInit()
    {
        List<IUpdateSystem> updateSystems = new List<IUpdateSystem>();
        List<ILateUpdateSystem> lateUpdateSystems = new List<ILateUpdateSystem>();
        
        foreach (var system in _systems)
        {
            if(system is IUpdateSystem updateSystem)
            {
                updateSystems.Add(updateSystem);
            }
            
            if(system is ILateUpdateSystem lateUpdateSystem)
            {
                lateUpdateSystems.Add(lateUpdateSystem);
            }
        }
        
        _updateSystems = updateSystems.ToArray();
        _lateUpdateSystems = lateUpdateSystems.ToArray();
        
        foreach (var system in _systems)
        {
            if (system is SystemGroup systemGroup)
            {
                systemGroup.InternalInit();
            }
        }
        
        // pre init
        foreach (var system in _systems)
        {
            if (system is IPreInitSystem preInitSystem)
            {
                preInitSystem.PreInit();
            }
        }
        
        // init
        foreach (var system in _systems)
        {
            if (system is IInitSystem initSystem)
            {
                initSystem.Init();
            }
        }
    }
    
    internal void InternalUpdate()
    {
        if (null != _updateSystems)
        {
            foreach (var system in _updateSystems)
            {
                system.Update();
            }
        }
        
        foreach (var system in _systems)
        {
            if (system is SystemGroup systemGroup)
            {
                systemGroup.InternalUpdate();
            }
        }
    }
    
    internal void InternalLateUpdate()
    {
        if (null != _lateUpdateSystems)
        {
            foreach (var system in _lateUpdateSystems)
            {
                system.LateUpdate();
            }
        }
        
        foreach (var system in _systems)
        {
            if (system is SystemGroup systemGroup)
            {
                systemGroup.InternalLateUpdate();
            }
        }
    }
    
    protected void InternalDestroy()
    {
        // pre destroy
        foreach (var system in _systems)
        {
            if (system is IPreDestroySystem preDestroySystem)
            {
                preDestroySystem.OnPreDestroy();
            }
        }
        
        // destroy
        foreach (var system in _systems)
        {
            if (system is IDestroySystem destroySystem)
            {
                destroySystem.OnDestroy();
            }
        }
        
        foreach (var system in _systems)
        {
            if (system is SystemGroup systemGroup)
            {
                systemGroup.InternalDestroy();
            }
        }
    }
}

public interface ISystem
{
}

public interface IPreInitSystem : ISystem
{
    void PreInit();
}

public interface IInitSystem : ISystem
{
    void Init();
}

public interface IUpdateSystem : ISystem
{
    void Update();
}

public interface ILateUpdateSystem : ISystem
{
    void LateUpdate();
}

public interface IPreDestroySystem : ISystem
{
    void OnPreDestroy();
}

public interface IDestroySystem : ISystem
{
    void OnDestroy();
}
