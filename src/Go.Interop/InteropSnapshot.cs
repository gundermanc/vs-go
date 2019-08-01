namespace Go.Interop
{
    using System.Runtime.InteropServices;
    using System.Text;
    using Go.Interop.Text;

    // Here be dragons...
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal unsafe struct InteropSnapshot
    {
#if WINDOWS
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
#endif
        public delegate int ReadCallback(
            // VS Mac runs on Mono runtime, which appears to have
            // trouble with the [Out] attribute.
#if WINDOWS
            [Out]
#endif
            byte* buffer,
            int offset,
            int count);

        public InteropSnapshot(ReadCallback readCallback, int length)
        {
            this.readCallback = readCallback;
            this.length = length;
        }

        public readonly ReadCallback readCallback;

        // TODO: on 64 bit, this needs to marshal as the word size.
        public readonly int length;

        public static InteropSnapshot FromSnapshot(SnapshotBase snapshotBase)
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

            ReadCallback callback = CopyChars;

            // Ensure that the callback can't be GC-ed.
            // TODO: free?
            GCHandle.Alloc(callback);

            return new InteropSnapshot(callback, snapshotBase.Length);
        }
    }
}
