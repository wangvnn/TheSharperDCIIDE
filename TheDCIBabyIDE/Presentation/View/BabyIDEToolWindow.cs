using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Windows.Forms;
using KimHaiQuang.TheDCIBabyIDE.Infrastructure.Services;
using Microsoft.VisualStudio.Text.Editor;
using KimHaiQuang.TheDCIBabyIDE.Domain.Operation;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.Settings;
using KimHaiQuang.TheDCIBabyIDE.Presentation.Operation;

namespace KimHaiQuang.TheDCIBabyIDE.View
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// 
    /// Note:
    /// In MVC pattern this class is the Controller code to facilitate the View (ToolWindow)
    /// The child content will be implememted using WPF MVVM pattern
    /// 
    /// </summary>
    [Guid("7df8029b-658a-4eb8-81ce-fa45d0dd1def")]
    public class BabyIDEToolWindow : ToolWindowPane, IOleCommandTarget
    {
        #region Infrastructure

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public BabyIDEToolWindow() :
            base(null)
        {
            this.WhenContructWindow();
        }

        /// <summary>
        /// Return the content of this window to be displayed after this window is constructed.
        /// Note: use this to trigger ContextCreation use case
        /// </summary>
        /// <returns></returns>
        public override object Content
        {
            get
            {
                return this.WhenShowContent();
            }
        }

        /// <summary>
        ///We need to set up the tool window to respond to key bindings
        ///They're passed to the tool window and its buffers via Query() and Exec()
        /// </summary>
        public override void OnToolWindowCreated()
        {
            var windowFrame = (IVsWindowFrame)Frame;
            var cmdUi = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory;
            windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUi);
            base.OnToolWindowCreated();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override bool PreProcessMessage(ref Message m)
        {
            if (WhenPreProcessMessage(ref m))
            {
                return true;
            }

            return base.PreProcessMessage(ref m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pguidCmdGroup"></param>
        /// <param name="nCmdID"></param>
        /// <param name="nCmdexecopt"></param>
        /// <param name="pvaIn"></param>
        /// <param name="pvaOut"></param>
        /// <returns></returns>
        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
          IntPtr pvaIn, IntPtr pvaOut)
        {
            return WhenIOleCommandTargetExec(ref pguidCmdGroup, nCmdexecopt, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pguidCmdGroup"></param>
        /// <param name="cCmds"></param>
        /// <param name="prgCmds"></param>
        /// <param name="pCmdText"></param>
        /// <returns></returns>
        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[]
          prgCmds, IntPtr pCmdText)
        {
            return WhenIOleCommandTargetQueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        #endregion

        #region Implementations

        private void WhenContructWindow()
        {
            SetupWindow();
            ReadIDESettings();
        }

        private void SetupWindow()
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
        }

        private BabyIDESettings IDESettings { get; set; }
        private void ReadIDESettings()
        {
            // TODO: load from Options Page
            IDESettings = new BabyIDESettings();
            IDESettings.ContextFileTypeSettings = BabyIDESettings.ContextFiletype.ContextFiletype_Injectionless;
        }

        private BabyIDEEditor _BabyIDEEditor;
        private object WhenShowContent()
        {
            if (_BabyIDEEditor == null)
            {
                _BabyIDEEditor = new BabyIDEEditor();
                new ContextFileOpeningContext(ProjectSelectionService.Instance, 
                    IDESettings, EditorService.Instance, _BabyIDEEditor).Open();

                //_myBabyIDEEditor.InteractionView.Graph.DataContext = interactionVM;
                //_myBabyIDEEditor.UsecaseView.Content = UsecaseViewHost;
                //_myBabyIDEEditor.ProjectedCodeView.Content = ProjectedTextViewHost;
            }

            return _BabyIDEEditor;
        }

        private bool WhenPreProcessMessage(ref Message m)
        {
            if (UsecaseViewHost != null)
            {
                // copy the Message into a MSG[] array, so we can pass
                // it along to the active core editor's IVsWindowPane.TranslateAccelerator
                var pMsg = new MSG[1];
                pMsg[0].hwnd = m.HWnd;
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;

                var vsWindowPane = (IVsWindowPane)_FocusedTextView;
                return vsWindowPane.TranslateAccelerator(pMsg) == 0;
            }

            return false;
        }
        private IVsTextView _FocusedTextView;

        private int WhenIOleCommandTargetExec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
          IntPtr pvaIn, IntPtr pvaOut)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_FocusedTextView != null)
            {
                var cmdTarget = (IOleCommandTarget)_FocusedTextView;
                hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            return hr;
        }

        private int WhenIOleCommandTargetQueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[]
          prgCmds, IntPtr pCmdText)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_FocusedTextView != null)
            {
                var cmdTarget = (IOleCommandTarget)_FocusedTextView;
                hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }
            return hr;
        }


        #endregion


//        #region Move to editor manager

//        private IVsInvisibleEditor invisibleEditor = null;
//        /// <summary>
//        /// Creates an invisible editor for a given filePath. 
//        /// If you're frequently creating projection buffers, it may be worth caching
//        /// these editors as they're somewhat expensive to create.
//        /// </summary>
//        private IVsInvisibleEditor RegisterInvisibleEditor(string filePath)
//        {
//            if (invisibleEditor != null)
//            {
//                return invisibleEditor;
//            }
//            ErrorHandler.ThrowOnFailure(this._invisibleEditorManager.RegisterInvisibleEditor(
//                filePath
//                , pProject: null
//                , dwFlags: (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING
//                , pFactory: null
//                , ppEditor: out invisibleEditor));

//            return invisibleEditor;
//        }

//        public IWpfTextViewHost CreateEditor(string filePath, int start = 0, int end = 0, bool createProjectedEditor = false)
//        {
//            //IVsInvisibleEditors are in-memory represenations of typical Visual Studio editors.
//            //Language services, highlighting and error squiggles are hooked up to these editors
//            //for us once we convert them to WpfTextViews. 
//            var invisibleEditor = RegisterInvisibleEditor(filePath);
//            var docDataPointer = IntPtr.Zero;
//            Guid guidIVsTextLines = typeof(IVsTextLines).GUID;

//            ErrorHandler.ThrowOnFailure(invisibleEditor.GetDocData(
//                fEnsureWritable: 1
//                , riid: ref guidIVsTextLines
//                , ppDocData: out docDataPointer));

//            IVsTextLines docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

//            //Create a code window adapter
//            var codeWindow = _editorAdapter.CreateVsCodeWindowAdapter(VisualStudioServices.OLEServiceProvider);
//            ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(docData));

//            //Get a text view for our editor which we will then use to get the WPF control for that editor.
//            IVsTextView textView;
//            ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out textView));


//            if (createProjectedEditor)
//            {
//                //We add our own role to this text view. Later this will allow us to selectively modify
//                //this editor without getting in the way of Visual Studio's normal editors.
//                var roles = _editorFactoryService.DefaultRoles.Concat(new string[] { "CustomProjectionRole" });

//                var vsTextBuffer = docData as IVsTextBuffer;
//                var textBuffer = _editorAdapter.GetDataBuffer(vsTextBuffer);

//                textBuffer.Properties.AddProperty("StartPosition", start);
//                textBuffer.Properties.AddProperty("EndPosition", end);
//                var guid = VSConstants.VsTextBufferUserDataGuid.VsTextViewRoles_guid;
//                ((IVsUserData)codeWindow).SetData(ref guid, _editorFactoryService.CreateTextViewRoleSet(roles).ToString());
//            }


//            _FocusedTextView = textView;
//            var textViewHost = _editorAdapter.GetWpfTextViewHost(textView);
//            return textViewHost;
//        }
//        private IWpfTextViewHost _usecaseTextViewHost;
//        public IWpfTextViewHost UsecaseViewHost
//        {
//            get
//            {
//                if (_usecaseTextViewHost == null)
//                {
//                    _usecaseTextViewHost = CreateEditor(Filename);
//                }
//                return _usecaseTextViewHost;
//            }
//        }

//        private IWpfTextViewHost _projectedTextViewHost;
//        public IWpfTextViewHost ProjectedTextViewHost
//        {
//            get
//            {
//                if (_projectedTextViewHost == null)
//                {
//                    _projectedTextViewHost = CreateEditor(Filename, start: 0, end: 100, createProjectedEditor: true);
//                }
//                return _projectedTextViewHost;
//            }
//        }
//#endregion
    }

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    //[Guid("7df8029b-658a-4eb8-81ce-fa45d0dd1def")]
    //public class BabyIDEToolWindow : ToolWindowPane, IOleCommandTarget
    //{
    //    private string filePath = @"c:\users\lenovo\documents\visual studio 2013\Projects\forDCIBabyIDETesting\forDCIBabyIDETesting\Program.cs";

    //    IComponentModel _componentModel;
    //    IVsInvisibleEditorManager _invisibleEditorManager;

    //    //This adapter allows us to convert between Visual Studio 2010 editor components and
    //    //the legacy components from Visual Studio 2008 and earlier.
    //    IVsEditorAdaptersFactoryService _editorAdapter;
    //    ITextEditorFactoryService _editorFactoryService;
    //    IVsTextView _currentlyFocusedTextView;

    //    /// <summary>
    //    /// Standard constructor for the tool window.
    //    /// </summary>
    //    public BabyIDEToolWindow() :
    //        base(null)
    //    {
    //        this.Caption = Resources.ToolWindowTitle;
    //        this.BitmapResourceID = 301;
    //        this.BitmapIndex = 1;

    //        _componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
    //        _invisibleEditorManager = (IVsInvisibleEditorManager)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsInvisibleEditorManager));
    //        if (_componentModel != null)
    //        {
    //            _editorAdapter = _componentModel.GetService<IVsEditorAdaptersFactoryService>();
    //            _editorFactoryService = _componentModel.GetService<ITextEditorFactoryService>();
    //        }




    //    }
    //    private IVsInvisibleEditor invisibleEditor = null;
    //    /// <summary>
    //    /// Creates an invisible editor for a given filePath. 
    //    /// If you're frequently creating projection buffers, it may be worth caching
    //    /// these editors as they're somewhat expensive to create.
    //    /// </summary>
    //    private IVsInvisibleEditor RegisterInvisibleEditor(string filePath)
    //    {
    //        if (invisibleEditor != null)
    //        {
    //            return invisibleEditor;
    //        }
    //        ErrorHandler.ThrowOnFailure(this._invisibleEditorManager.RegisterInvisibleEditor(
    //            filePath
    //            , pProject: null
    //            , dwFlags: (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING
    //            , pFactory: null
    //            , ppEditor: out invisibleEditor));

    //        return invisibleEditor;
    //    }

    //    public IWpfTextViewHost CreateEditor(string filePath, int start = 0, int end = 0, bool createProjectedEditor = false)
    //    {
    //        //IVsInvisibleEditors are in-memory represenations of typical Visual Studio editors.
    //        //Language services, highlighting and error squiggles are hooked up to these editors
    //        //for us once we convert them to WpfTextViews. 
    //        var invisibleEditor = RegisterInvisibleEditor(filePath);
    //        var docDataPointer = IntPtr.Zero;
    //        Guid guidIVsTextLines = typeof(IVsTextLines).GUID;

    //        ErrorHandler.ThrowOnFailure(invisibleEditor.GetDocData(
    //            fEnsureWritable: 1
    //            , riid: ref guidIVsTextLines
    //            , ppDocData: out docDataPointer));

    //        IVsTextLines docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

    //        IVsTextBuffer buffer = (IVsTextBuffer)docData;

    //        var textBuffer = _editorAdapter.GetDataBuffer(buffer);

    //        var textView = _editorFactoryService.CreateTextView(textBuffer);
    //        _currentlyFocusedTextView = _editorAdapter.GetViewAdapter(textView);
    //        var textViewHost = _editorFactoryService.CreateTextViewHost(textView, true);
    //        return textViewHost;
    //        ////Create a code window adapter
    //        //var codeWindow = _editorAdapter.CreateVsCodeWindowAdapter(BabyIDEVisualStudioServices.OLEServiceProvider);
    //        //ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(docData));

    //        ////Get a text view for our editor which we will then use to get the WPF control for that editor.
    //        //IVsTextView textView;
    //        //ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out textView));


    //        //if (createProjectedEditor)
    //        //{
    //        //    //We add our own role to this text view. Later this will allow us to selectively modify
    //        //    //this editor without getting in the way of Visual Studio's normal editors.
    //        //    var roles = _editorFactoryService.DefaultRoles.Concat(new string[] { "CustomProjectionRole" });

    //        //    var vsTextBuffer = docData as IVsTextBuffer;
    //        //    var textBuffer = _editorAdapter.GetDataBuffer(vsTextBuffer);

    //        //    textBuffer.Properties.AddProperty("StartPosition", start);
    //        //    textBuffer.Properties.AddProperty("EndPosition", end);
    //        //    var guid = VSConstants.VsTextBufferUserDataGuid.VsTextViewRoles_guid;
    //        //    ((IVsUserData)codeWindow).SetData(ref guid, _editorFactoryService.CreateTextViewRoleSet(roles).ToString());
    //        //}


    //        //_currentlyFocusedTextView = textView;
    //        //var textViewHost = _editorAdapter.GetWpfTextViewHost(textView);
    //        //return textViewHost;
    //    }
    //    private IWpfTextViewHost _completeTextViewHost;
    //    public IWpfTextViewHost CompleteTextViewHost
    //    {
    //        get
    //        {
    //            if (_completeTextViewHost == null)
    //            {
    //                _completeTextViewHost = CreateEditor(filePath);
    //            }
    //            return _completeTextViewHost;
    //        }
    //    }

    //    private IWpfTextViewHost _projectedTextViewHost;
    //    public IWpfTextViewHost ProjectedTextViewHost
    //    {
    //        get
    //        {
    //            if (_projectedTextViewHost == null)
    //            {
    //                _projectedTextViewHost = CreateEditor(filePath, start: 0, end: 0, createProjectedEditor: true);
    //            }
    //            return _projectedTextViewHost;
    //        }
    //    }

    //    private BabyIDEEditor _myBabyIDEEditor;
    //    public override object Content
    //    {
    //        get
    //        {
    //            if (_myBabyIDEEditor == null)
    //            {
    //                _myBabyIDEEditor = new BabyIDEEditor();
    //                _myBabyIDEEditor.GeneratedCode.Content = CompleteTextViewHost;
    //                _myBabyIDEEditor.ProjectedCode.Content = ProjectedTextViewHost;
    //            }
    //            return _myBabyIDEEditor;
    //        }
    //    }

    //    public override void OnToolWindowCreated()
    //    {
    //        //We need to set up the tool window to respond to key bindings
    //        //They're passed to the tool window and its buffers via Query() and Exec()
    //        var windowFrame = (IVsWindowFrame)Frame;
    //        var cmdUi = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory;
    //        windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUi);
    //        base.OnToolWindowCreated();
    //    }

    //    protected override bool PreProcessMessage(ref Message m)
    //    {
    //        if (CompleteTextViewHost != null)
    //        {
    //            // copy the Message into a MSG[] array, so we can pass
    //            // it along to the active core editor's IVsWindowPane.TranslateAccelerator
    //            var pMsg = new MSG[1];
    //            pMsg[0].hwnd = m.HWnd;
    //            pMsg[0].message = (uint)m.Msg;
    //            pMsg[0].wParam = m.WParam;
    //            pMsg[0].lParam = m.LParam;

    //            var vsWindowPane = (IVsWindowPane)_currentlyFocusedTextView;
    //            return vsWindowPane.TranslateAccelerator(pMsg) == 0;
    //        }
    //        return base.PreProcessMessage(ref m);
    //    }

    //    int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
    //      IntPtr pvaIn, IntPtr pvaOut)
    //    {
    //        var hr =
    //          (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

    //        if (_currentlyFocusedTextView != null)
    //        {
    //            var cmdTarget = (IOleCommandTarget)_currentlyFocusedTextView;
    //            hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
    //        }
    //        return hr;
    //    }

    //    int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[]
    //      prgCmds, IntPtr pCmdText)
    //    {
    //        var hr =
    //          (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

    //        if (_currentlyFocusedTextView != null)
    //        {
    //            var cmdTarget = (IOleCommandTarget)_currentlyFocusedTextView;
    //            hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    //        }
    //        return hr;
    //    }

    //}
}
