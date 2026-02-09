using System;

namespace XDTGame.Core;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class InjectAttribute : Attribute
{
}