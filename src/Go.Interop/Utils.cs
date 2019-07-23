namespace Go.Interop
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using Go.Interop.Text;

    public static class Utils
    {
        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void PrintSnapshot(GoSnapshot snapshot);
    }

    // Here be dragons...
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct GoSnapshot
    {
        private static List<ReadCallback> keepAlive = new List<ReadCallback>();

        public delegate int ReadCallback(
            // VS Mac runs on Mono runtime, which appears to have
            // trouble with the [Out] attribute.
#if WINDOWS
            [Out]
#endif
            byte* buffer,
            int offset,
            int count);

        public GoSnapshot(ReadCallback readCallback, int length)
        {
            this.readCallback = readCallback;
            this.length = length;
        }

        public readonly ReadCallback readCallback;

        // TODO: on 64 bit, this needs to marshal as the word size.
        public readonly int length;

        public static GoSnapshot FromSnapshot(SnapshotBase snapshotBase)
        {
            // TODO: can this memory alternatively be addressed with Memory<T>?
            unsafe int CopyChars(byte* buffer, int offset, int count)
            {
                var chars = stackalloc char[count];
                for (int i = 0; i < count && (i + offset) < snapshotBase.Length; i++)
                {
                    chars[i] = snapshotBase[i + offset];
                }

                // TODO: this might have issues with encoding conversions of multi-char characters
                // at array boundaries.
                return Encoding.UTF8.GetBytes(chars, count, buffer, count);
            }

            // HACK: keep the delegates rooted so they aren't GC-ed.
            // Eventually this should be managed as part of the snapshot life-cycle.
            keepAlive.Add(CopyChars);

            return new GoSnapshot(CopyChars, snapshotBase.Length);
        }
    }
}
