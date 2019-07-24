namespace Go.Editor.Classification
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IClassificationTag))]
    [ContentType(GoContentType.Name)]
    internal sealed class ClassificationTaggerProvider : ITaggerProvider
    {
        private readonly GoWorkspace workspace;
        private readonly IClassificationTypeRegistryService classificationTypeRegistry;

        [ImportingConstructor]
        public ClassificationTaggerProvider(
            GoWorkspace workspace,
            IClassificationTypeRegistryService classificationTypeRegistry)
        {
            this.workspace = workspace
                ?? throw new ArgumentNullException(nameof(workspace));
            this.classificationTypeRegistry = classificationTypeRegistry
                ?? throw new ArgumentNullException(nameof(classificationTypeRegistry));
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new ClassificationTagger(this.workspace, this.classificationTypeRegistry, buffer) as ITagger<T>;
        }
    }
}
