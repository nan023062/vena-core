//****************************************************************************
// File: ConvertHelper.cs
// Author: Li Nan
// Date: 2024-01-07 12:00
// Version: 1.0
//****************************************************************************

using System.Runtime.InteropServices;

namespace Vena
{
    [StructLayout(LayoutKind.Explicit)]
    struct UIntFloat
    {
        [FieldOffset(0)] public float floatValue;

        [FieldOffset(0)] public uint intValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct UIntDouble
    {
        [FieldOffset(0)] public double doubleValue;

        [FieldOffset(0)] public ulong longValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct UIntDecimal
    {
        [FieldOffset(0)] public ulong longValue1;

        [FieldOffset(8)] public ulong longValue2;

        [FieldOffset(0)] public decimal decimalValue;
    }
}