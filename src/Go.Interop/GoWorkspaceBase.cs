namespace Go.Interop
{
    using System.Runtime.InteropServices;
    using System.Text;

    public abstract class GoWorkspaceBase
    {
        private readonly int workspaceId;

        protected unsafe GoWorkspaceBase()
        {
            this.workspaceId = CreateNewWorkspace();
            RegisterWorkspaceUpdateCallback(this.workspaceId, this.WorkspaceUpdated);
        }

        public void QueueFileParse(string fileName, GoSnapshot snapshot)
        {
            var utf8Name = Encoding.UTF8.GetBytes(fileName);

            QueueFileParse(this.workspaceId, utf8Name, utf8Name.Length, snapshot);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private unsafe delegate void WorkspaceUpdateCallback(byte* fileName, int count);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int CreateNewWorkspace();

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterWorkspaceUpdateCallback(int workspaceId, WorkspaceUpdateCallback workspaceUpdateCallback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void QueueFileParse(int workspaceId, byte[] fileName, int count, GoSnapshot snapshot);

        private unsafe void WorkspaceUpdated(byte* fileName, int count)
        {

        }
    }
}
