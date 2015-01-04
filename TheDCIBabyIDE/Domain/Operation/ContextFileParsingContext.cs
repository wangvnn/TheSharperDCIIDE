using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.VisualStudio.Text;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.Settings;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Domain.Reader.Injectionless;
using KimHaiQuang.TheDCIBabyIDE.Domain.Reader.Marvin;
using KimHaiQuang.TheDCIBabyIDE.Presentation.Operation;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Operation
{
    class RegionNodes
    {
        public SyntaxTrivia RegionDirective;
        public SyntaxTrivia EndRegionDirective;

        public string RegionName
        {
            get { return RegionDirective.ToFullString(); }
        }

        public TextSpan RegionSpan
        {
            get
            {
                var start = RegionDirective.Span.Start;
                var end = EndRegionDirective.Span.Start + EndRegionDirective.Span.Length;
                if (end >= start)
                {
                    return new TextSpan(start, end - start);
                }
                else
                {
                    return new TextSpan(start, 0);
                }
            }
        }
        public List<SyntaxNode> Nodes = new List<SyntaxNode>();
        public void AddNode(SyntaxNode node)
        {
            if (RegionSpan.Contains(node.Span))
                Nodes.Add(node);
        }
    }

    public class ContextFileParsingContext :
        ContextFileOpeningContext.IContextFileParserRole
    {
        #region Usecase
        /* USE CASE 2: Parse CONTEXT FILE
        # Primary Actor: SYSTEM
        # Precondition: CONTEXT FILE is valid
        # Postcondition: CONTEX INFO will be returned
        # Trigger: SYSTEM asks the PARSER to parse the FILE
        # Main Success Scenario:
        # 1. READER FACTORY reads IDE SETTINGS to create suitable READER
        # 2. READER reads and returns INFO 
        # Alternatives:
        # 1.1 SETTINGS is unknown CONTEXT FILE TYPE
        # 1.2 SYSTEM throws error message
        */
        #endregion

        #region Roles

        private ContextFileParsingContext DCIContextReaderFactory { get; set; }
        private IDCIContextReader DCIContextReader { get; set; }
        public interface IDCIContextReader
        {
            DCIContext Read(string filePath);
        }
        private BabyIDESettings IDESettings { get; set; }
        private DCIContext ContextFileModel = null;

        #endregion

        #region Context

        public ContextFileParsingContext(BabyIDESettings settings)
        {
            IDESettings = settings;
            ContextFileModel = new DCIContext();

            DCIContextReaderFactory = this;
            DCIContextReader = DCIContextReaderFactory_GetReader();
        }

        public DCIContext Parse(string filePath)
        {
            DCIContextReader.Read(filePath);

            return ContextFileModel;
        }

        #endregion

        #region DCIContextReaderFactory_Methods

        private IDCIContextReader DCIContextReaderFactory_GetReader()
        {
            switch (IDESettings.ContextFileTypeSettings)
            {
                case BabyIDESettings.ContextFiletype.ContextFiletype_Injectionless:
                {
                    return new DCIInjectionlessContextReader(ContextFileModel);
                }
                case BabyIDESettings.ContextFiletype.ContextFiletype_Marvin:
                {
                    return new DCIMarvinContextReader(ContextFileModel);
                }
                default:
                {
                    throw new Exception("Cannot create DCI Context Reader for unknown Context File Type");
                }
            }
        }

        #endregion
    }
}