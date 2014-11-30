using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.ComponentModelHost;
using System.Windows.Forms;

namespace KimHaiQuang.TheDCIBabyIDE
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("7df8029b-658a-4eb8-81ce-fa45d0dd1def")]
    public class BabyIDEToolWindow : ToolWindowPane, IOleCommandTarget
    {
        private string filePath = @"C:\Users\Lenovo\documents\visual studio 2013\Projects\test\test\Program.cs";

        IComponentModel _componentModel;
        IVsInvisibleEditorManager _invisibleEditorManager;

        //This adapter allows us to convert between Visual Studio 2010 editor components and
        //the legacy components from Visual Studio 2008 and earlier.
        IVsEditorAdaptersFactoryService _editorAdapter;
        ITextEditorFactoryService _editorFactoryService;
        IVsTextView _currentlyFocusedTextView;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public BabyIDEToolWindow() :
            base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            _componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            _invisibleEditorManager = (IVsInvisibleEditorManager)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsInvisibleEditorManager));
            if (_componentModel != null)
            {
                _editorAdapter = _componentModel.GetService<IVsEditorAdaptersFactoryService>();
                _editorFactoryService = _componentModel.GetService<ITextEditorFactoryService>();
            }
        }

        /// <summary>
        /// Creates an invisible editor for a given filePath. 
        /// If you're frequently creating projection buffers, it may be worth caching
        /// these editors as they're somewhat expensive to create.
        /// </summary>
        private IVsInvisibleEditor RegisterInvisibleEditor(string filePath)
        {
            IVsInvisibleEditor invisibleEditor;
            ErrorHandler.ThrowOnFailure(this._invisibleEditorManager.RegisterInvisibleEditor(
                filePath
                , pProject: null
                , dwFlags: (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING
                , pFactory: null
                , ppEditor: out invisibleEditor));

            return invisibleEditor;
        }

        public IWpfTextViewHost CreateEditor(string filePath, int start = 0, int end = 0, bool createProjectedEditor = false)
        {
            //IVsInvisibleEditors are in-memory represenations of typical Visual Studio editors.
            //Language services, highlighting and error squiggles are hooked up to these editors
            //for us once we convert them to WpfTextViews. 
            var invisibleEditor = RegisterInvisibleEditor(filePath);

            var docDataPointer = IntPtr.Zero;
            Guid guidIVsTextLines = typeof(IVsTextLines).GUID;

            ErrorHandler.ThrowOnFailure(invisibleEditor.GetDocData(
                fEnsureWritable: 1
                , riid: ref guidIVsTextLines
                , ppDocData: out docDataPointer));

            IVsTextLines docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

            //Create a code window adapter
            var codeWindow = _editorAdapter.CreateVsCodeWindowAdapter(BabyIDEVisualStudioServices.OLEServiceProvider);
            ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(docData));

            //Get a text view for our editor which we will then use to get the WPF control for that editor.
            IVsTextView textView;
            ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out textView));

            if (createProjectedEditor)
            {
                //We add our own role to this text view. Later this will allow us to selectively modify
                //this editor without getting in the way of Visual Studio's normal editors.
                var roles = _editorFactoryService.DefaultRoles.Concat(new string[] { "CustomProjectionRole" });

                var vsTextBuffer = docData as IVsTextBuffer;
                var textBuffer = _editorAdapter.GetDataBuffer(vsTextBuffer);

                textBuffer.Properties.AddProperty("StartPosition", start);
                textBuffer.Properties.AddProperty("EndPosition", end);
                var guid = VSConstants.VsTextBufferUserDataGuid.VsTextViewRoles_guid;
                ((IVsUserData)codeWindow).SetData(ref guid, _editorFactoryService.CreateTextViewRoleSet(roles).ToString());
            }


            _currentlyFocusedTextView = textView;
            var textViewHost = _editorAdapter.GetWpfTextViewHost(textView);
            return textViewHost;
        }
        private IWpfTextViewHost _completeTextViewHost;
        public IWpfTextViewHost CompleteTextViewHost
        {
            get
            {
                if (_completeTextViewHost == null)
                {
                    _completeTextViewHost = CreateEditor(filePath);
                }
                return _completeTextViewHost;
            }
        }

        private IWpfTextViewHost _projectedTextViewHost;
        public IWpfTextViewHost ProjectedTextViewHost
        {
            get
            {
                if (_projectedTextViewHost == null)
                {
                    _projectedTextViewHost = CreateEditor(filePath, start: 0, end: 200, createProjectedEditor: true);
                }
                return _projectedTextViewHost;
            }
        }

        private BabyIDEEditor _myBabyIDEEditor;
        public override object Content
        {
            get
            {
                if (_myBabyIDEEditor == null)
                {
                    _myBabyIDEEditor = new BabyIDEEditor();
                    _myBabyIDEEditor.fullFile.Content = CompleteTextViewHost;
                    _myBabyIDEEditor.partialFile.Content = ProjectedTextViewHost;
                }
                return _myBabyIDEEditor;
            }
        }

        public override void OnToolWindowCreated()
        {
            //We need to set up the tool window to respond to key bindings
            //They're passed to the tool window and its buffers via Query() and Exec()
            var windowFrame = (IVsWindowFrame)Frame;
            var cmdUi = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory;
            windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUi);
            base.OnToolWindowCreated();
        }

        protected override bool PreProcessMessage(ref Message m)
        {
            if (CompleteTextViewHost != null)
            {
                // copy the Message into a MSG[] array, so we can pass
                // it along to the active core editor's IVsWindowPane.TranslateAccelerator
                var pMsg = new MSG[1];
                pMsg[0].hwnd = m.HWnd;
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;

                var vsWindowPane = (IVsWindowPane)_currentlyFocusedTextView;
                return vsWindowPane.TranslateAccelerator(pMsg) == 0;
            }
            return base.PreProcessMessage(ref m);
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
          IntPtr pvaIn, IntPtr pvaOut)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_currentlyFocusedTextView != null)
            {
                var cmdTarget = (IOleCommandTarget)_currentlyFocusedTextView;
                hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            return hr;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[]
          prgCmds, IntPtr pCmdText)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_currentlyFocusedTextView != null)
            {
                var cmdTarget = (IOleCommandTarget)_currentlyFocusedTextView;
                hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }
            return hr;
        }

    }
}
