using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Windows.Forms;
using KimHaiQuang.TheDCIBabyIDE.Infrastructure.Services;
using Microsoft.VisualStudio.Text.Editor;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.Settings;
using KimHaiQuang.TheDCIBabyIDE.Presentation.Operation;
using System.Windows;
using System.Windows.Input;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base;
using System.ComponentModel;
using Microsoft.VisualStudio.Text;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.View
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
    public class BabyIDEToolWindow : ToolWindowPane, IOleCommandTarget, IVsWindowFrameNotify3,
        ContextFileOpeningContext.IContextFileEditorRole
    {
        #region Public funtions

        public void Display(DCIContext contextModel)
        {
            _BabyIDEEditor.View.Content = new BabyIDEEditorView();
            ContextModel = contextModel;
        }

        #endregion

        #region Infrastructure
        public int OnClose(ref uint pgrfSaveOptions)
        {
            WhenCloseWindow();
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnMove(int x, int y, int w, int h)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnShow(int fShow)
        {
            if (fShow == 12)
            {
                WhenShowWindow();
            }
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public BabyIDEToolWindow() :
            base(null)
        {
            WhenContructWindow();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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
            return WhenIOleCommandTargetExec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
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

        private void WhenCloseWindow()
        {
            if (ContextModel != null)
            {
                ContextModel = null;
                EditorService.Instance.CloseEditor();

                if (_BabyIDEEditor.View.Content != null)
                {
                    _BabyIDEEditor.View.Content = null;
                }
                
            }
        }


        private void WhenShowWindow()
        {
            OpenSelectedContextFile();
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


        private DCIContext _contextModel;
        private DCIContext ContextModel
        {
            get
            {
                return _contextModel;
            }
            set
            {
                _contextModel = value;
                if (_contextModel != null)
                {
                    this.UsecaseView = EditorService.Instance.CreateProjectionEditor(_contextModel.Filepath, _contextModel.UsecaseSpan.Start, _contextModel.UsecaseSpan.Length);
                    this.ProjectionView = EditorService.Instance.CreateProjectionEditor(_contextModel.Filepath, _contextModel.CodeSpan.Start, _contextModel.CodeSpan.Length);
                    this.InteractionViewModel = new ContextViewModel(_contextModel);
                }
                else
                {
                    this.UsecaseView = null;
                    this.ProjectionView = null;
                    this.InteractionViewModel = null;
                }
            }
        }

        private ContextViewModel _InteractionViewModel = null;
        private ContextViewModel InteractionViewModel
        {
            get
            {
                return _InteractionViewModel;
            }
            set
            {
                if (_InteractionViewModel != null)
                {
                    _InteractionViewModel.UnRegisterRoutedCommandHandlers();
                    _InteractionViewModel.ChangeCodeSpanRequest -= WhenCodeSpanRequest;
                }

                _InteractionViewModel = value;

                if (_InteractionViewModel != null)
                {
                    _InteractionViewModel.ChangeCodeSpanRequest += WhenCodeSpanRequest;
                }

                (_BabyIDEEditor.View.Content as BabyIDEEditorView).DataContext = _InteractionViewModel;
            }
        }

        private void WhenCodeSpanRequest(object sender, EventArgs e)
        {
            Span span = (sender as SpanObject).CodeSpan;
            this.ProjectionView = EditorService.Instance.CreateProjectionEditor(ContextModel.Filepath,
                span.Start,
                span.Length);
        }

        private IWpfTextViewHost _UsecaseView = null;
        private IWpfTextViewHost UsecaseView
        {
            set
            {
                if (_UsecaseView != null)
                {
                    (_UsecaseView as UIElement).LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.Editor_LostKeyboardFocus);
                    (_UsecaseView as UIElement).GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.Editor_GotKeyboardFocus);
                }

                _UsecaseView = value;

                if (_UsecaseView != null)
                {
                    (_UsecaseView as UIElement).LostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.Editor_LostKeyboardFocus);
                    (_UsecaseView as UIElement).GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.Editor_GotKeyboardFocus);
                }

               (_BabyIDEEditor.View.Content as BabyIDEEditorView).UsecaseView.Content = _UsecaseView;
            }
        }

        private IWpfTextViewHost _ProjectionView = null;
        private IWpfTextViewHost ProjectionView
        {
            get
            {
                return _ProjectionView;
            }
            set
            {
                if (_ProjectionView != null)
                {
                    (_ProjectionView as UIElement).LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.Editor_LostKeyboardFocus);
                    (_ProjectionView as UIElement).GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.Editor_GotKeyboardFocus);
                }

                _ProjectionView = value;

                if (_ProjectionView != null)
                {
                    (_ProjectionView as UIElement).LostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.Editor_LostKeyboardFocus);
                    (_ProjectionView as UIElement).GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.Editor_GotKeyboardFocus);
                }
                (_BabyIDEEditor.View.Content as BabyIDEEditorView).ProjectionCodeView.Content = _ProjectionView;
            }
        }

        private void Editor_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _FocusedTextViewHost = (IWpfTextViewHost)sender;
            e.Handled = true;
        }

        private void Editor_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _FocusedTextViewHost = null;
            e.Handled = true;
        }

        private BabyIDEEditor _BabyIDEEditor;
        private object WhenShowContent()
        {
            if (_BabyIDEEditor == null)
            {
                _BabyIDEEditor = new BabyIDEEditor();
            }

            return _BabyIDEEditor;
        }

        private void OpenSelectedContextFile()
        {
            if (ContextModel == null)
            {
                new ContextFileOpeningContext(ProjectSelectionService.Instance,
                    IDESettings, this).Open();
            }
            else if (ProjectSelectionService.Instance.GetSelectedItemFullPath() != ContextModel.Filepath)                
            {
                WhenCloseWindow();
                new ContextFileOpeningContext(ProjectSelectionService.Instance,
                    IDESettings, this).Open();
            }
        }

        private bool WhenPreProcessMessage(ref Message m)
        {
            if (_FocusedTextViewHost != null)
            {
                // copy the Message into a MSG[] array, so we can pass
                // it along to the active core editor's IVsWindowPane.TranslateAccelerator
                var pMsg = new MSG[1];
                pMsg[0].hwnd = m.HWnd;
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;

                var textView = EditorService.Instance.GetTextView(_FocusedTextViewHost);
                var vsWindowPane = (IVsWindowPane)textView;
                return vsWindowPane.TranslateAccelerator(pMsg) == 0;
            }

            return false;
        }
        private IWpfTextViewHost _FocusedTextViewHost;

        private int WhenIOleCommandTargetExec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
          IntPtr pvaIn, IntPtr pvaOut)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_FocusedTextViewHost != null)
            {
                var textView = EditorService.Instance.GetTextView(_FocusedTextViewHost);
                var cmdTarget = (IOleCommandTarget)textView;
                hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            return hr;
        }

        private int WhenIOleCommandTargetQueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[]
          prgCmds, IntPtr pCmdText)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_FocusedTextViewHost != null)
            {
                var textView = EditorService.Instance.GetTextView(_FocusedTextViewHost);
                var cmdTarget = (IOleCommandTarget)textView;
                hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }
            return hr;
        }


        #endregion
    }
}
