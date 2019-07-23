#region Copyright

// -------------------------------------------------------------------------------
// <copyright file="BuildCommandTarget.cs" company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------------

#endregion

namespace Go.Windows
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// NOTE: This is shelved now since Go doesn't have a "solution".
    /// </summary>
    [Export]
    internal sealed class BuildCommandTarget : IOleCommandTarget
    {
        [Import]
        public IVsOutputWindow VsOutputWindow { get; set; }

        // When command is "Build Solution", intercept it.
        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == VSConstants.CMDSETID.StandardCommandSet97_guid)
            {
                switch (prgCmds[0].cmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.BuildSln:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED | (uint)OLECMDF.OLECMDF_ENABLED;
                        return VSConstants.S_OK;
                }
            }

            return 0;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == VSConstants.CMDSETID.StandardCommandSet97_guid)
            {
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.BuildSln:
                        // Do something here
                        if (this.VsOutputWindow != null)
                        {
                            // Do something
                        }

                        return VSConstants.S_OK;
                }
            }

            return 0;
        }
    }
}
