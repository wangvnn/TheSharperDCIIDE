using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.Settings;
using KimHaiQuang.TheDCIBabyIDE.Domain.Operation;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.Operation
{
    public class ContextFileOpeningContext
    {
        #region Usecase
        // Use case: View CONTEXT FILE
        // Primary Actor: USER
        // Precondition: Valid CONTEXT FILE
        // Postcondition: EDITOR displays CONTEXT INFO
        // Trigger: USER wants to view CONTEXT FILE
        //
        // Steps:
        // 1) User selects CONTEXT FILE to open it in EDITOR
        // 2) SOLUTION EXPLORER finds SELECTED CONTEXT FILE
        // 2) CONTEXT FILE PARSER parses CONTEXT FILE to get CONTEXT MODEL (Sub: Parse CONTEXT FILE)
        // 3) CONTEXT FILE EDITOR displays CONTEXT MODEL in the VIEWER  (Sub: Display CONTEXT MODEL)
        #endregion

        #region Roles

        private ISolutionExplorerRole SolutionExplorer { get; set; }
        public interface ISolutionExplorerRole
        {
            string GetSelectedItemFullPath();
        }
        private IContextFileParserRole Parser { get; set; }
        public interface IContextFileParserRole
        {
            DCIContext Parse(string filePath);
        }

        private IContextFileEditorRole Editor { get; set; }
        public interface IContextFileEditorRole
        {
            void Display(DCIContext contextModel);
        }

        #endregion

        #region Context

        public ContextFileOpeningContext(ISolutionExplorerRole explorer,
            BabyIDESettings settings,
            ContextFileDisplayingContext.IEditorFactory factory,
            ContextFileDisplayingContext.IContextFileViewerRole viewer)
        {
            SolutionExplorer = explorer;

            Parser = new ContextFileParsingContext(settings);
            Editor = new ContextFileDisplayingContext(factory, viewer);
        }

        public void Open()
        {
            SolutionExplorer_FindSelectedContextFile();
        }

        #endregion

        #region SolutionExplorer_Methods

        void SolutionExplorer_FindSelectedContextFile()
        {
            string filePath = SolutionExplorer.GetSelectedItemFullPath();

            Parser_ParseContextFile(filePath);
        }

        #endregion

        #region Parser_Methods
        private void Parser_ParseContextFile(string filePath)
        {
            var contextModel = Parser.Parse(filePath);
            Editor.Display(contextModel);
        }
        #endregion
    }
}