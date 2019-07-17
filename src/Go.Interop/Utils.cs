using System.Runtime.InteropServices;

namespace Go.Interop
{
    public static class Utils
    {
        [DllImport(GoLib.LibName)]
        public static extern int GetGoRoot();
    }
}
