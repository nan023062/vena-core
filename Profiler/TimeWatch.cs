//****************************************************************************
// File: TimeWatch.cs
// Author: Li Nan
// Date: 2023-01-05 12:00
// Version: 1.0
//****************************************************************************

using System;
using System.Diagnostics;
using XDFramework.Core;

namespace XDTGame.Core
{
    public readonly struct TimeWatch : IDisposable
    {
        private readonly string _message;

        private readonly long _timestamp;
        
        public TimeWatch(string message)
        {
            _message = message;
            _timestamp = Stopwatch.GetTimestamp();
        }
        
        public void Dispose()
        {
#if DEBUG_SYSTEM
            float ms = (Stopwatch.GetTimestamp() - _timestamp) / (float)Stopwatch.Frequency * 1000;
            DebugSystem.LogWarning( LogCategory.Framework,$"[TimeWatch]{_message} : cost {ms} ms!");
#endif
        }
    }
}