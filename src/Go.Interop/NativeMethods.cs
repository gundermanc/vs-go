namespace Go.Interop
{
    using System.Runtime.InteropServices;
    using Go.Interop.Text;

    internal sealed class NativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal unsafe delegate void WorkspaceUpdatedCallback(byte* fileName, int length, SnapshotBase versionId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal unsafe delegate void ProvideStringCallback(byte* fileName, int length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal delegate void ProvideTokenCallback(int pos, int end, int type);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateNewWorkspace();

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RegisterWorkspaceUpdateCallback(int workspaceId, WorkspaceUpdatedCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void QueueFileParse(int workspaceId, byte[] fileName, int count, InteropSnapshot snapshot, SnapshotBase versionId);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetErrors(int workspaceId, byte[] fileName, int count, ProvideStringCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetCompletions(int workspaceId, byte[] fileName, int count, ProvideStringCallback callback, int position);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void GetTokens(int workspaceId, byte[] fileName, int count, ProvideTokenCallback callback);
    }
}
