namespace Go.Interop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Go.CodeAnalysis.Text;

    public static class Utils
    {
        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void PrintSnapshot(GoSnapshot snapshot);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GoSnapshot
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int ReadCallback([MarshalAs(UnmanagedType.LPArray, SizeConst = 10)][Out]byte[] buffer, int offset, int count);

        public GoSnapshot(ReadCallback readCallback, int length)
        {
            this.readCallback = readCallback ?? throw new ArgumentNullException(nameof(readCallback));
            this.length = length;
        }

        public readonly ReadCallback readCallback;

        // TODO: on 64 bit, this needs to marshal as the word size.
        public readonly int length;

        public static GoSnapshot FromSnapshot(SnapshotBase snapshotBase)
        {
            // TODO: this is all terribly inefficient.

            int CopyChars(byte[] buffer, int offset, int count)
            {
                var chars = new char[count];
                for (int i = 0; i < count && (i+offset) < snapshotBase.Length; i++)
                {
                    chars[i] = snapshotBase[i + offset];
                }

                // TODO: this might have issues with encoding conversions of multi-char characters
                // at array boundaries.

                return Encoding.UTF8.GetBytes(chars, 0, chars.Length, buffer, 0);
            }

            return new GoSnapshot(CopyChars, snapshotBase.Length);
        }
    }
}
