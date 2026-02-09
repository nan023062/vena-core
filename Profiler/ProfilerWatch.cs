//****************************************************************************
// File: ProfilerWatch.cs
// Author: Li Nan
// Date: 2023-01-05 12:00
// Version: 1.0
//****************************************************************************

using System;

namespace XDTGame.Core
{
    public readonly struct ProfilerWatch : IDisposable
    {
        public ProfilerWatch(in string message)
        {
#if DEBUG_SYSTEM && UNITY_EDITOR
            UnityEngine.Profiling.Profiler.BeginSample(message);
#endif
        }
        
        public void Dispose()
        {
#if DEBUG_SYSTEM  && UNITY_EDITOR
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }
    }
}