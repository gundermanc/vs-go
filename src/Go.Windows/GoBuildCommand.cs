using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Text;
using EnvDTE;

namespace Go.Windows
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GoBuildCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("13629496-9f13-45fd-96d9-b5ba4ac9c086");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoBuildCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GoBuildCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            MenuCommand menuItem = new MenuCommand(this.BuildGoFile, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        private void BuildGoFile(object sender, EventArgs e)
        {
            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                "Run go build",
                "Go Build",
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            // Try to get document path
            var dte = (ServiceProvider.GetServiceAsync(typeof(DTE))).Result as DTE;
            var dteServiceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
            var documentService = dteServiceProvider.GetService(typeof(Document)) as Document;
            if (documentService != null)
            {
                var documentPath = documentService.Path;
                Console.WriteLine(documentPath);
            }

            // Try to output something
            var outputWindowPane = ServiceProvider.GetServiceAsync(typeof(IVsOutputWindowPane)).Result as IVsOutputWindowPane;
            if (outputWindowPane != null)
            {
                outputWindowPane.Activate();
                outputWindowPane.OutputStringThreadSafe("Trying to run go build command");
            }
            
            var processInfo = new ProcessStartInfo("go.exe");
            processInfo.Arguments = "build -v";
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            System.Diagnostics.Process.Start(processInfo);
        }

        private string GetDocumentPath(ITextSnapshot textSnapshot)
        {
            ITextDocument textDoc;
            bool rc = textSnapshot.TextBuffer.Properties.TryGetProperty(
                typeof(Microsoft.VisualStudio.Text.ITextDocument), out textDoc);
            if (rc && textDoc != null)
                return textDoc.FilePath;
            return null;
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GoBuildCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GoBuildCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GoBuildCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "GoBuildMenu";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
