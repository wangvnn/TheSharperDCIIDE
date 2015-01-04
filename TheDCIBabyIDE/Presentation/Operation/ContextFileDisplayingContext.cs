using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using Microsoft.Internal.VisualStudio.PlatformUI;
using KimHaiQuang.TheDCIBabyIDE.Presentation.View;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel;
using CKimHaiQuang.TheDCIBabyIDE.ViewModel;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.Operation
{
    public class ContextFileDisplayingContext :
        ContextFileOpeningContext.IContextFileEditorRole
    {
        #region Usecase
        // Use case: Display CONTEXT MODEL
        // Primary Actor: SYSTEM
        // Precondition: Valid CONTEXT MODEL
        // Postcondition: EDITOR displays CONTEXT MODEL
        // Trigger: SYSTEM asks EDITOR to display CONTEXT MODEL
        //
        // Steps:
        // 1) SYSTEM asks EDITOR to display CONTEXT MODEL
        // 2) EDITOR asks PROJECTED VIEW FACTORY to create USECASE EDITOR
        // 3) USECASE EDITOR asks VIEWMODEL PROVIDER to provide USECASE TEXT VIEW MODEL
        // 4) EDITOR asks PROJECTED VIEW FACTORY to create PROJECTED CODE EDITOR (show CONTEXT CODE by default)
        // 5) PROJECTED CODE EDITOR asks VIEWMODEL PROVIDER to provide PROJECTED TEXT VIEW MODEL
        // 6) EDITOR creates INTERACTION VIEW MODEL to display CONTEXT's INTERACTION 
        #endregion

        #region Roles

        public interface IContextFileViewerRole
        {
            ContentControl UsecaseView { get; }
            ContentControl ProjectedCodeView { get; }
            FrameworkElement InteractionView { get; }
        }
        private IContextFileViewerRole ContextFileViewer { get; set; }

        private IEditorFactory EditorFactory { get; set; }
        public interface IEditorFactory
        {
            void CreateProjectionEditor(string filePath, int start, int length, out IWpfTextViewHost host, out IVsTextView view);
        }
        #endregion

        #region Context

        public ContextFileDisplayingContext(IEditorFactory factory, IContextFileViewerRole viewer)
        {
            ContextFileViewer = viewer;
            EditorFactory = factory;
        }

        public void Display(DCIContext contextModel)
        {
            ProjectedViewFactory_CreateUsecaseEditor();
        }

        #endregion

        #region ProjectedViewFactory_Methods

        void ProjectedViewFactory_CreateUsecaseEditor()
        {
            var useCaseEditor = ProjectedViewFactory.CreateEditor();
            ContextFileViewer.UsecaseView.Content = useCaseEditor.TextViewHost;

            var codeEditor = ProjectedViewFactory.CreateEditor();
            ContextFileViewer.ProjectedCodeView.Content = codeEditor.TextViewHost;

            var interactionViewModel = new ContextInteractionViewModel();
            ContextFileViewer.InteractionView.DataContext = interactionViewModel;
        }
        
        #endregion
    }
}