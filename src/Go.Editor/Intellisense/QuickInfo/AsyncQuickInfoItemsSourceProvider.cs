namespace Go.Editor.Intellisense.QuickInfo
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Utilities;

    // TODO: either enable for Mac or remove.
    //[Export(typeof(IAsyncQuickInfoSourceProvider))]
    [ContentType(GoContentType.Name)]
    internal sealed class AsyncQuickInfoItemsSourceProvider : IAsyncQuickInfoSourceProvider
    {
        private readonly ITextDocumentFactoryService textDocumentFactoryService;

        [ImportingConstructor]
        public AsyncQuickInfoItemsSourceProvider(ITextDocumentFactoryService textDocumentFactoryService)
        {
            this.textDocumentFactoryService = textDocumentFactoryService
                ?? throw new ArgumentNullException(nameof(textDocumentFactoryService));
        }

        public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) => new AsyncQuickInfoSource(this.textDocumentFactoryService, textBuffer);
    }
}
