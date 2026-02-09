using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace XDTGame.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DescriptionAttribute : Attribute
{
    public readonly int Id;
    public readonly string Content;
    
    public DescriptionAttribute(int contentId, string content = default)
    {
        Id = contentId;
        Content = content;
    }
}

public static class DescriptionAttributeExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDescription(this System.Object obj)
    {
        Type type = obj.GetType();
        var attr = type.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Content ?? type.Name;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetDescriptionId(this System.Object obj)
    {
        Type type = obj.GetType();
        var attr = type.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Id ?? 0;
    }
}