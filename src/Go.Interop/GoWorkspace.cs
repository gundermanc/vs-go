namespace Go.Interop
{
    using System.Runtime.InteropServices;

    public static class GoWorkspace
    {
        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateNewWorkspace();

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseWorkspace(int workspaceId);
    }
}
