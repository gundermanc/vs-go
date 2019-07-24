namespace Go.Editor.Errors
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(GoContentType.Name)]
    [TagType(typeof(IErrorTag))]
    internal sealed class ErrorTaggerProvider : ITaggerProvider
    {
        private readonly GoWorkspace workspace;

        [ImportingConstructor]
        public ErrorTaggerProvider(GoWorkspace workspace)
        {
            this.workspace = workspace
                ?? throw new ArgumentNullException(nameof(workspace));
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new ErrorTagger(this.workspace, buffer) as ITagger<T>;
        }
    }
}
