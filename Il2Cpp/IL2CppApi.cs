using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using XDFramework.Core;

namespace MonoGame;

public enum Il2CppTypeEnum
{
    IL2CPP_TYPE_END = 0x00,       // End of List  
    IL2CPP_TYPE_VOID = 0x01,
    IL2CPP_TYPE_BOOLEAN = 0x02,
    IL2CPP_TYPE_CHAR = 0x03,
    IL2CPP_TYPE_I1 = 0x04,
    IL2CPP_TYPE_U1 = 0x05,
    IL2CPP_TYPE_I2 = 0x06,
    IL2CPP_TYPE_U2 = 0x07,
    IL2CPP_TYPE_I4 = 0x08,
    IL2CPP_TYPE_U4 = 0x09,
    IL2CPP_TYPE_I8 = 0x0a,
    IL2CPP_TYPE_U8 = 0x0b,
    IL2CPP_TYPE_R4 = 0x0c,
    IL2CPP_TYPE_R8 = 0x0d,
    IL2CPP_TYPE_STRING = 0x0e,
    IL2CPP_TYPE_PTR = 0x0f,       // arg: <type> token  
    IL2CPP_TYPE_BYREF = 0x10,     // arg: <type> token  
    IL2CPP_TYPE_VALUETYPE = 0x11, // arg: <type> token  
    IL2CPP_TYPE_CLASS = 0x12,     // arg: <type> token  
    IL2CPP_TYPE_VAR = 0x13,       // Generic parameter in a generic type definition, represented as number (compressed unsigned integer) number  
    IL2CPP_TYPE_ARRAY = 0x14,     // type, rank, boundsCount, bound1, loCount, lo1  
    IL2CPP_TYPE_GENERICINST = 0x15,     // <type> <type-arg-count> <type-1> \x{2026} <type-n>  
    IL2CPP_TYPE_TYPEDBYREF = 0x16,
    IL2CPP_TYPE_I = 0x18,
    IL2CPP_TYPE_U = 0x19,
    IL2CPP_TYPE_FNPTR = 0x1b,     // arg: full method signature  
    IL2CPP_TYPE_OBJECT = 0x1c,
    IL2CPP_TYPE_SZARRAY = 0x1d,   // 0-based one-dim-array  
    IL2CPP_TYPE_MVAR = 0x1e,      // Generic parameter in a generic method definition, represented as number (compressed unsigned integer)  
    IL2CPP_TYPE_CMOD_REQD = 0x1f, // arg: typedef or typeref token  
    IL2CPP_TYPE_CMOD_OPT = 0x20,  // optional arg: typedef or typref token  
    IL2CPP_TYPE_INTERNAL = 0x21,  // CLR internal type  

    IL2CPP_TYPE_MODIFIER = 0x40,  // Or with the following types  
    IL2CPP_TYPE_SENTINEL = 0x41,  // Sentinel for varargs method signature  
    IL2CPP_TYPE_PINNED = 0x45,    // Local var that points to pinned object  

    IL2CPP_TYPE_ENUM = 0x55       // an enumeration  
}


public class IL2CppApi
{
#if UNITY_IOS && !UNITY_EDITOR || UNITY_STANDALONE_WIN
    public const string LibIl2cpp = "__Internal";
#else
    public const string LibIl2cpp = "il2cpp";
#endif
    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_domain_get();

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_domain_assembly_open(IntPtr domain,string ns);
    
    
    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_domain_get_assemblies(IntPtr domain, out int size);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_assembly_get_image( IntPtr assembly);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_image_get_name(IntPtr image);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_get_corlib();

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_class_from_name(IntPtr image, string ns, string name);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_object_unbox(IntPtr il2cppObj);

    [DllImport(LibIl2cpp)]
    public unsafe static extern IntPtr il2cpp_value_box(IntPtr klass, void* data);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_string_chars(IntPtr il2cppObj);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_object_get_class(IntPtr il2cppObj);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_object_new(IntPtr klass);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_class_get_field_from_name( IntPtr klass, string fieldname);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_class_get_property_from_name(IntPtr klass, string propertyname);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_property_get_get_method(IntPtr property );

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_property_get_set_method(IntPtr property);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_class_get_method_from_name(IntPtr klass, string name, int argsCount);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_runtime_invoke_convert_args(IntPtr method, IntPtr obj, IntPtr* param, int paramCount, IntPtr* exc);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, IntPtr* param, IntPtr* exc);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_string_new_utf16(Char* text, int len);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_class_get_methods(IntPtr klass, void** iter);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_class_get_nested_types(IntPtr klass, void** iter);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_runtime_class_init(IntPtr klass);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_runtime_object_init(IntPtr il2cppobj);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_method_get_name(IntPtr method);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_type_get_name(IntPtr type);

    [DllImport(LibIl2cpp)]
    public static extern IntPtr il2cpp_thread_attach(IntPtr domain);

    [DllImport(LibIl2cpp)]
    public static extern void il2cpp_thread_detach(IntPtr thread);

    [DllImport(LibIl2cpp)]
    public static extern uint il2cpp_method_get_param_count(IntPtr method);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_method_get_param(IntPtr method, uint index);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern int il2cpp_type_get_type(IntPtr type);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern int il2cpp_field_get_offset(IntPtr method);
    
    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_class_from_type(IntPtr type);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_class_get_type(IntPtr klass);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_type_get_object(IntPtr type);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern void il2cpp_format_exception(IntPtr ex, StringBuilder message, int message_size);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern void il2cpp_field_get_value(IntPtr obj, IntPtr fieldinfo, void* value);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern void il2cpp_field_set_value(IntPtr obj, IntPtr fieldinfo, void* value);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern void il2cpp_field_set_value_object(IntPtr obj, IntPtr fieldinfo, IntPtr objectvalue);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_field_get_value_object(IntPtr fieldinfo, IntPtr obj);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern int il2cpp_class_value_size(IntPtr klass, IntPtr align);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_class_get_name(IntPtr klass);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_class_get_namespace(IntPtr klass);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_class_get_fields(IntPtr klass, void** iter);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_field_get_name(IntPtr fieldinfo);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr il2cpp_field_get_type(IntPtr fieldinfo);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_field_static_get_value(IntPtr fieldinfo, void* value );

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_field_static_set_value(IntPtr fieldinfo, void* value);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_array_new(IntPtr elementTypeInfo, int length);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern IntPtr il2cpp_array_class_get(IntPtr element_class, int rank);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public unsafe static extern int il2cpp_array_length(IntPtr il2cppArray);

    [DllImport(LibIl2cpp, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool il2cpp_class_is_assignable_from(IntPtr klass, IntPtr oklass);

    private static Dictionary<string, IntPtr> allil2cppImages = new Dictionary<string, IntPtr>();

    // @temp remove, for next version
    static IL2CppApi()
    {
        IntPtr domain = IL2CppApi.il2cpp_domain_get();

        int size = 0;
        IntPtr assemblies = IL2CppApi.il2cpp_domain_get_assemblies(domain, out size);

        //DebugSystem.LogError(LogCategory.System, $"assembiles count: {size}");

        for (int i = 0; i < size; ++i)
        {
            IntPtr assemblyPtr = Marshal.ReadIntPtr(assemblies, i * IntPtr.Size);
            IntPtr image = IL2CppApi.il2cpp_assembly_get_image(assemblyPtr);
            IntPtr imagename = IL2CppApi.il2cpp_image_get_name(image);
            string imagenamestring = Marshal.PtrToStringAnsi(imagename);
            allil2cppImages.Add(imagenamestring, image);
        }
    }


    public static IntPtr il2cpp_class_from_name(string image, string ns, string name)
    {
        if ( allil2cppImages.TryGetValue(image, out var imagePtr))
        {
            return IL2CppApi.il2cpp_class_from_name(imagePtr, ns, name);
        }
        return IntPtr.Zero;
    }


    public static void GetAllImages()
    {
        foreach( var imagename in  allil2cppImages.Keys )
        {
            DebugSystem.LogError(LogCategory.System, $"ImageName:{imagename}");
        }
    }
}
