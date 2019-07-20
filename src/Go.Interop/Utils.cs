namespace Go.Interop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class Utils
    {
        [DllImport(GoLib.LibName)]
        public static extern int GetGoRoot();

        [DllImport(GoLib.LibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateSnapshot(byte[] inBytes, byte[] outBytes, uint byteLen);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GoString
    {
        public GoString(string str)
        {
            this.str = str;
            this.n = str.Length;
        }

        [MarshalAs(UnmanagedType.LPStr)]
        public string str;

        // TODO: on 64 bit, this needs to marshal as the word size.
        public int n;
    }
}
