namespace Go.Interop
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class NativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal unsafe delegate void WorkspaceUpdatedCallback(byte* fileName, int length, IntPtr versionId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal unsafe delegate void ProvideStringCallback(byte* fileName, int length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate void ProvideTokenCallback(int pos, int end, int type);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateNewWorkspace();

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterWorkspaceUpdateCallback(int workspaceId, WorkspaceUpdatedCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void QueueFileParse(int workspaceId, byte[] fileName, int count, InteropSnapshot snapshot, IntPtr versionId);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetErrors(int workspaceId, byte[] fileName, int count, ProvideStringCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetQuickInfo(int workspaceId, byte[] fileName, int count, int offset, ProvideStringCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetCompletions(int workspaceId, byte[] fileName, int count, ProvideStringCallback callback, int position);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetTokens(int workspaceId, byte[] fileName, int count, ProvideTokenCallback callback);
    }
}
