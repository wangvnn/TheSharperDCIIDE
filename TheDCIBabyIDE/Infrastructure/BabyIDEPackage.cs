using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.IO;
using KimHaiQuang.TheDCIBabyIDE.Services;
using KimHaiQuang.TheDCIBabyIDE.Infrastructure.Services;
using Microsoft.VisualStudio.ComponentModelHost;
using KimHaiQuang.TheDCIBabyIDE.Presentation.View;

namespace KimHaiQuang.TheDCIBabyIDE
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(BabyIDEToolWindow))]
    [Guid(GuidList.guidTheDCIBabyIDEPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class BabyIDEPackage : Package
    {
        #region Infrastructure

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public BabyIDEPackage()
        {
            WhenConstructPackage();
        }

        ~BabyIDEPackage()
        {
            Dispose(false);
        }

        private bool _Disposed = false; // to detect redundant calls
        protected override void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    WhenDisposeResource();
                }

                // new shared cleanup logic
                _Disposed = true;
            }

            base.Dispose(disposing);
        }


        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            WhenInitializePackage();
        }

        #endregion

        #region Implementations

        private void WhenConstructPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        private void WhenInitializePackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            Setup();
            RegisterCommand();
        }

        private void Setup()
        {
            VisualStudioServices.ServiceProvider = this;
            VisualStudioServices.OLEServiceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)VisualStudioServices.ServiceProvider.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider));

            VisualStudioServices.ComponentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));

            EditorService.Create(VisualStudioServices.ServiceProvider, VisualStudioServices.OLEServiceProvider);
            ProjectSelectionService.Create();
        }

        private void RegisterCommand()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidTheDCIBabyIDECmdSet, (int)PkgCmdIDList.cmdidDCIBabyIDE);

                var menuItem = new OleMenuCommand(WhenShowToolWindow, toolwndCommandID);
                menuItem.BeforeQueryStatus += MenuCommand_BeforeQueryStatus;

                mcs.AddCommand(menuItem);
            }
        }

        private void WhenShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(BabyIDEToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void MenuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                // start by assuming that the menu will not be shown
                menuCommand.Visible = false;
                menuCommand.Enabled = false;

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;

                if (ProjectSelectionService.Instance.IsSingleProjectItemSelection(out hierarchy, out itemid))
                {
                    // Get the file path
                    string itemFullPath = ProjectSelectionService.Instance.GetItemFullPath(hierarchy, itemid);
                    var transformFileInfo = new FileInfo(itemFullPath);

                    // then check if the .cs file
                    bool isCsFile = transformFileInfo.Extension.Contains("cs");
                    // if not leave the menu hidden
                    if (isCsFile)
                    {
                        menuCommand.Visible = true;
                        menuCommand.Enabled = true;
                    }
                }
            }
        }

        private void WhenDisposeResource()
        {
            EditorService.Destroy();
            ProjectSelectionService.Destroy();
        }
        #endregion
    }
}