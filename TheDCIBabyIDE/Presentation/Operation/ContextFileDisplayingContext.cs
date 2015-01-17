﻿using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel;
using Microsoft.VisualStudio.Text.Editor;

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
        // 2) EDITOR asks PROJECTION VIEW FACTORY to create USECASE VIEW
        // 3) EDITOR asks PROJECTION VIEW FACTORY to create PROJECTION CODE VIEW (show CONTEXT CODE by default)
        // 4) EDITOR creates INTERACTION VIEW MODEL to display CONTEXT's INTERACTION 
        #endregion

        #region Roles

        public interface IContextFileViewerRole
        {
            IWpfTextViewHost UsecaseView { set; }
            IWpfTextViewHost ProjectionView { set; }
            ContextViewModel InteractionViewModel { set; }
        }
        private IContextFileViewerRole ContextFileViewer { get; set; }

        private IEditorFactory EditorFactory { get; set; }
        public interface IEditorFactory
        {
            IWpfTextViewHost CreateProjectionEditor(string filePath, int start, int length);
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
            ProjectionViewFactory_Display(contextModel);
        }

        #endregion

        #region ProjectedViewFactory_Methods

        void ProjectionViewFactory_Display(DCIContext contextModel)
        {
            ContextFileViewer.UsecaseView = EditorFactory.CreateProjectionEditor(contextModel.Filepath, contextModel.UsecaseSpan.Start, contextModel.UsecaseSpan.Length);
            ContextFileViewer.ProjectionView = EditorFactory.CreateProjectionEditor(contextModel.Filepath, contextModel.ContextSpan.Start, contextModel.ContextSpan.Length);
            ContextFileViewer.InteractionViewModel = new ContextViewModel(contextModel);
        }

        #endregion
    }
}