using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;

namespace Go.Windows
{
    [Export]
    public class VsServiceFactory
    {
        [Import]
        public SVsServiceProvider sVsServiceProvider { get; set; }

        [Import]
        public ITextDocumentFactoryService textDocumentFactoryService { get; set; }

        [Import]
        public IVsOutputWindow vsOutputWindow { get; set; }

        [ImportingConstructor]
        public VsServiceFactory(
            SVsServiceProvider sVsServiceProvider,
            ITextDocumentFactoryService textDocumentFactoryService,
            IVsOutputWindow vsOutputWindow)
        {
            this.sVsServiceProvider = sVsServiceProvider;
            this.textDocumentFactoryService = textDocumentFactoryService;
            this.vsOutputWindow = vsOutputWindow;
        }
    }
}
