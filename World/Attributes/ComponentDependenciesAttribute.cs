///////////////////////////////////////////////////////////////////////////////////////////////////
// Author      : LiNan
// Description : Attribute For Entity Component
// Department  : XDTown Client / Gameplay-Entity
///////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace XDTLevelAndEntity.Core.World.Attributes;

/// <summary>
/// 依赖组件
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class ComponentDependenciesAttribute : Attribute
{
    /// <summary>
    /// 依赖组件
    /// </summary>
    public readonly Type[] Dependencies;
        
    public ComponentDependenciesAttribute(params Type[] dependencyComponentTypes)
    {
        this.Dependencies = dependencyComponentTypes;
    }
}