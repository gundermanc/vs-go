namespace Go.Editor.Errors
{
    using System;
    using System.ComponentModel.Composition;
    using System.Runtime.InteropServices;
    using System.Text;
    using Go.Editor.Workspace;
    using Go.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Threading;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(GoContentType.Name)]
    [TagType(typeof(IErrorTag))]
    internal sealed class ErrorTaggerProvider : ITaggerProvider
    {
        private readonly JoinableTaskContext joinableTaskContext;
        private readonly WorkspaceService workspace;

        [ImportingConstructor]
        public ErrorTaggerProvider(
            JoinableTaskContext joinableTaskContext,
            WorkspaceService workspace)
        {
            this.joinableTaskContext = joinableTaskContext ?? throw new ArgumentNullException(nameof(joinableTaskContext));
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer textBuffer) where T : ITag
        {
            var document = this.workspace.GetOrCreateDocument(textBuffer);

            var inBytes = Encoding.UTF8.GetBytes("Yo, whazzup?");
            var outBytes = new byte[100];
            Utils.CreateSnapshot(inBytes, outBytes, (uint)outBytes.Length);

            return new ErrorTagger(this.joinableTaskContext, document) as ITagger<T>;
        }
    }
}
