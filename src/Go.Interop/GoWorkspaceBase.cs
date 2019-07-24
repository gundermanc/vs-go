﻿namespace Go.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    public abstract class GoWorkspaceBase
    {
        private readonly int workspaceId;

        // Ensure callback is not GC-ed after passing to GO.
        private readonly ProvideStringCallback workspaceUpdatedDelegate;

        public event EventHandler<string> WorkspaceFileUpdated;

        protected unsafe GoWorkspaceBase()
        {
            this.workspaceId = CreateNewWorkspace();
            workspaceUpdatedDelegate = this.WorkspaceUpdated;
            RegisterWorkspaceUpdateCallback(this.workspaceId, this.workspaceUpdatedDelegate);
        }

        public void QueueFileParse(string fileName, GoSnapshot snapshot)
        {
            var utf8Name = Encoding.UTF8.GetBytes(fileName);

            QueueFileParse(this.workspaceId, utf8Name, utf8Name.Length, snapshot);
        }

        public unsafe IList<string> GetErrors()
        {
            var errors = new List<string>();

            void ErrorCallback(byte* errorText, int length)
            {
                errors.Add(Encoding.UTF8.GetString(errorText, length));
            }

            GetWorkspaceErrors(this.workspaceId, ErrorCallback);
            return errors;
        }

        public unsafe IList<string> GetWorkspaceCompletions()
        {
            var completions = new List<string>();

            void CompletionsCallback(byte* errorText, int length)
            {
                completions.Add(Encoding.UTF8.GetString(errorText, length));
            }

            GetWorkspaceCompletions(this.workspaceId, CompletionsCallback);
            return completions;
        }

        public IList<TypedToken> GetFileTokens()
        {
            var tokens = new List<TypedToken>();

            void TokensCallback(int pos, int end, int type)
            {
                tokens.Add(new TypedToken(pos, end, (TokenType)type));
            }

            // TODO: file name.
            var fileNameBytes = Encoding.UTF8.GetBytes("foo.go");

            GetTokens(this.workspaceId, fileNameBytes, fileNameBytes.Length, TokensCallback);
            return tokens;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private unsafe delegate void ProvideStringCallback(byte* fileName, int length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private delegate void ProvideTokenCallback(int pos, int end, int type);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int CreateNewWorkspace();

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterWorkspaceUpdateCallback(int workspaceId, ProvideStringCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void QueueFileParse(int workspaceId, byte[] fileName, int count, GoSnapshot snapshot);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetWorkspaceErrors(int workspaceId, ProvideStringCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetWorkspaceCompletions(int workspaceId, ProvideStringCallback callback);

        [DllImport(GoLib.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetTokens(int workspaceId, byte[] fileName, int count, ProvideTokenCallback callback);

        private unsafe void WorkspaceUpdated(byte* fileName, int length)
        {
            this.WorkspaceFileUpdated?.Invoke(this, Encoding.UTF8.GetString(fileName, length));
        }
    }
}
