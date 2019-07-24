namespace Go.Editor
{
    using System.ComponentModel.Composition;
    using Go.Interop;

    [Export(typeof(GoWorkspace))]
    internal class GoWorkspace : GoWorkspaceBase
    {
    }
}
