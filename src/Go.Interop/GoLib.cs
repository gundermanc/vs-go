﻿namespace Go.Interop
{
    internal static class GoLib
    {
#if WINDOWS
        public const string LibName = "golib.dll";
#else
        public const string LibName = "golib.so";
#endif
    }
}
