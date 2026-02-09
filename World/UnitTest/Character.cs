#if DEBUG_SYSTEM
using UnityEngine;
using XDFramework.Core;

namespace XDTGame.Core.UnitTest;

public class CharacterActor : Actor, ICreate, IStart, IUpdate, ILateUpdate,
    IBeforeDestroy, IDestroy, IApplicationFocus
{
    public void OnCreate()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnCreate");
    }

    public void OnStart()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnStart");
    }

    public void Update(float deltaTime)
    {
    }

    public void LateUpdate(float deltaTime)
    {
    }
    
    public void OnBeforeDestroy()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnBeforeDestroy");
    }
    
    public void OnDestroy()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnDestroy");
    }

    public void OnApplicationFocus(bool focus)
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnApplicationFocus ( {focus} )");
    }
}

public class CharComponent : Component, ICreate, IStart, IUpdate, ILateUpdate,
    IBeforeDestroy, IDestroy, IApplicationFocus
{
    public void OnCreate()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnCreate");
    }

    public void OnStart()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnStart");
    }

    public void Update(float deltaTime)
    {
        if (Input.GetKey(KeyCode.U))
        {
            DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnUpdate");
        }
    }
    
    public void LateUpdate(float deltaTime)
    {
        if (Input.GetKey(KeyCode.L))
        {
            DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnLateUpdate");
        }
    }
    
    public void OnBeforeDestroy()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnBeforeDestroy");
    }
    
    public void OnDestroy()
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnDestroy");
    }

    public void OnApplicationFocus(bool focus)
    {
        DebugSystem.LogWarning(LogCategory.Framework, $"{GetType().Name}.OnApplicationFocus ( {focus} )");
    }
}

#endif