namespace Go.Editor.Intellisense.QuickInfo
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [ContentType(GoContentType.Name)]
    internal sealed class QuickInfoItemSourceProvider : IAsyncQuickInfoSourceProvider
    {
        private readonly GoWorkspace workspace;

        [ImportingConstructor]
        public QuickInfoItemSourceProvider(GoWorkspace workspace)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new QuickInfoItemSource(this.workspace, textBuffer);
        }
    }
}
