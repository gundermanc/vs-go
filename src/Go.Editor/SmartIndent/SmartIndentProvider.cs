namespace Go.Editor.SmartIndent
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ISmartIndentProvider))]
    [ContentType(GoContentType.Name)]
    internal sealed class SmartIndentProvider : ISmartIndentProvider
    {
        private readonly GoWorkspace workspace;

        [ImportingConstructor]
        public SmartIndentProvider(GoWorkspace workspace)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        }

        public ISmartIndent CreateSmartIndent(ITextView textView)
        {
            var document = this.workspace.GetOrCreateDocument(textView.TextBuffer);
            return new SmartIndent(document, textView.Options);
        }
    }
}
