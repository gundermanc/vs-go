namespace Go.Editor.Intellisense.QuickInfo
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [ContentType(GoContentType.Name)]
    internal sealed class QuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        private readonly GoWorkspace workspace;

        [ImportingConstructor]
        internal QuickInfoSourceProvider(GoWorkspace workspace)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            var document = this.workspace.GetOrCreateDocument(textBuffer);

            return new QuickInfoSource(document);
        }
    }
}
