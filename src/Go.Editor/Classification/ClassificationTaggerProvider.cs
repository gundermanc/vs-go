namespace Go.Editor.Classification
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(GoContentType.Name)]
    [TagType(typeof(IClassificationTag))]
    internal sealed class ClassificationTaggerProvider : ITaggerProvider
    {
        private readonly IClassificationTypeRegistryService classificationRegistryService;

        [ImportingConstructor]
        public ClassificationTaggerProvider(IClassificationTypeRegistryService classificationRegistryService)
        {
            this.classificationRegistryService = classificationRegistryService
                ?? throw new ArgumentNullException(nameof(classificationRegistryService));
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer textBuffer) where T : ITag
        {
            return new ClassificationTagger(classificationRegistryService, textBuffer) as ITagger<T>;
        }
    }
}
