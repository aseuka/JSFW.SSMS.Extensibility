using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using System.Reflection;

namespace JSFW.SSMS.Extensibility.Cmds
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ObjectFinder
    { 
        private IVsWindowFrame m_windowFrame = null;
        ObjectFindForm objectFindForm = null;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1682ddce-f8b8-4f00-94a9-00cf439b8458");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectFinder"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ObjectFinder(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ObjectFinder Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
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
        public static void Initialize(Package package)
        {
            Instance = new ObjectFinder(package);
        }
        
        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "ObjectFinder";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.ServiceProvider,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            try
            {
                string currentSelectedQuery = "";
                var _applicationObject = ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
                 
                TextDocument textDoc = (TextDocument)_applicationObject.ActiveWindow.Document.Object("TextDocument");
                if (textDoc != null)
                {
                    textDoc.StartPoint.CreateEditPoint();
                    currentSelectedQuery = textDoc.Selection.Text;
                    textDoc = null;
                }

                ShowToolWindow(currentSelectedQuery);
            }
            catch (Exception ex)
            {
            }
        }
         

        private void ShowToolWindow(string currentSelectedQuery)
        {
            // TODO: Change this Guid
            const string TOOLWINDOW_GUID = "C350A22C-5A7C-47E8-A11F-A5C294831598";

            if (objectFindForm == null)
            {
                objectFindForm = new ObjectFindForm() { TopLevel = false };
            }
            objectFindForm.SetVSPackage( this.package as VSPackage );
            objectFindForm.SetFindObject(currentSelectedQuery);
            
            if (m_windowFrame == null)
            {
                m_windowFrame = CreateToolWindow("기능 목록 보기", TOOLWINDOW_GUID, objectFindForm);
                // TODO: Initialize m_userControl if required adding a method like:
                //    internal void Initialize(VSPackageToolWindowPackage package)
                // and pass this instance of the package:
                //    m_userControl.Initialize(this);
            } 
            m_windowFrame.Show();
        }

        private IVsWindowFrame CreateToolWindow(string caption, string guid, Control content)
        {
            const int TOOL_WINDOW_INSTANCE_ID = 0; // Single-instance toolwindow

            IVsUIShell uiShell;
            Guid toolWindowPersistenceGuid;
            Guid guidNull = Guid.Empty;
            int[] position = new int[1];
            int result;
            IVsWindowFrame windowFrame = null;
            uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));
            toolWindowPersistenceGuid = new Guid(guid);
            result = uiShell.CreateToolWindow((uint)__VSCREATETOOLWIN.CTW_fInitNew,
                  TOOL_WINDOW_INSTANCE_ID, content, ref guidNull, ref toolWindowPersistenceGuid,
                  ref guidNull, null, caption, position, out windowFrame);

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(result);
            return windowFrame;
        }
    }
}
