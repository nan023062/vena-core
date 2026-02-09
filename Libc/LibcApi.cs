using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame;

public class LibCApi
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private const string cexport = "msvcrt";
#else
    private const string cexport = "c";
#endif

    [DllImport(cexport, CharSet = CharSet.Ansi)]
    public static extern IntPtr fopen(string filename, string mode);

    [DllImport(cexport, CharSet = CharSet.Ansi)]
    public static extern int fread(byte[] buffer, int size, int count, IntPtr stream);

    [DllImport(cexport, CharSet = CharSet.Ansi)]
    public static extern int fclose(IntPtr stream);
}
