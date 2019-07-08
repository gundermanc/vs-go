namespace Go.Editor.SmartIndent
{
    using System;
    using System.ComponentModel.Composition;
    using Go.Editor.Workspace;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ISmartIndentProvider))]
    [ContentType(GoContentType.Name)]
    internal sealed class SmartIndentProvider : ISmartIndentProvider
    {
        private readonly WorkspaceService workspaceService;

        [ImportingConstructor]
        public SmartIndentProvider(WorkspaceService workspaceService)
        {
            this.workspaceService = workspaceService
                ?? throw new ArgumentNullException(nameof(workspaceService));
        }

        public ISmartIndent CreateSmartIndent(ITextView textView)
        {
            var document = this.workspaceService.GetOrCreateDocument(textView.TextBuffer);

            // TODO: respond to tab size change event.
            var tabSize = textView.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);

            return new SmartIndent(document, tabSize);
        }
    }
}
