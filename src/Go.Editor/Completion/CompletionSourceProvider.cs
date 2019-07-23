namespace Go.Editor.Completion
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType(GoContentType.Name)]
    [Name("Go Completion")]
    internal sealed class CompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        private readonly ITextStructureNavigatorSelectorService textStructureNavigatorSelectorService;
        private readonly GoWorkspace workspace = null;

        [ImportingConstructor]
        public CompletionSourceProvider(
            ITextStructureNavigatorSelectorService textStructureNavigatorSelectorService,
            GoWorkspace workspace)
        {
            this.textStructureNavigatorSelectorService = textStructureNavigatorSelectorService ??
                throw new ArgumentNullException(nameof(textStructureNavigatorSelectorService));
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        }

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
            => textView.Properties.GetOrCreateSingletonProperty(
                typeof(CompletionSourceProvider),
                () => this.CreateAndConfigureCompletionSource(textView));

        private IAsyncCompletionSource CreateAndConfigureCompletionSource(ITextView textView)
        {
            // TODO: create singleton property.
            var source = new CompletionSource(this.textStructureNavigatorSelectorService, this.workspace);

            return source;
        }
    }
}
