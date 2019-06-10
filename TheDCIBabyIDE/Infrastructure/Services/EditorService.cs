using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Linq;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Projection;
using KimHaiQuang.TheDCIBabyIDE.Presentation.Operation;
using System.Collections;

namespace KimHaiQuang.TheDCIBabyIDE.Infrastructure.Services
{
    public class EditorService
    {
        #region CONST
        private  const string DCI_BABY_IDE = "DCI_BABY_IDE";
        #endregion

        #region Singleton

        private static Microsoft.VisualStudio.OLE.Interop.IServiceProvider OLEServiceProvider { get; set; }
        private static System.IServiceProvider ServiceProvider { get; set; }
        private static IVsInvisibleEditorManager InvisibleEditorManager { get; set;}
        private static IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }
        private static ITextEditorFactoryService TextEditorFactoryService { get; set; }
        private static IVsRunningDocumentTable RunningDocumentTable { get; set; }

        public static void Create(System.IServiceProvider serviceProvider, Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider)
        {
            OLEServiceProvider = oleServiceProvider;
            ServiceProvider = serviceProvider;

            InvisibleEditorManager = (IVsInvisibleEditorManager)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsInvisibleEditorManager));
            RunningDocumentTable = (IVsRunningDocumentTable)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsRunningDocumentTable));
            var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));

            if (componentModel != null)
            {
                EditorAdaptersFactoryService = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                TextEditorFactoryService = componentModel.GetService<ITextEditorFactoryService>();
            }

            if (_Instance == null)
            {
                _Instance = new EditorService();
            }
        }

        public static void Destroy()
        {
            if (_Instance != null)
            {
                _Instance.Cleanup();
                _Instance = null;
            }
        }

        private static EditorService _Instance = null;
        public static EditorService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    throw new Exception("Must call Create before using the Singleton");
                }

                return _Instance;
            }
        }

        #endregion

        #region Instance's Implementation

        public EditorService()
        {
        }

        public IVsTextView GetTextView(IWpfTextViewHost host)
        {
            return EditorService.EditorAdaptersFactoryService.GetViewAdapter(host.TextView);
        }

        public IWpfTextViewHost CreateProjectionEditor(string filePath, int start, int length, bool isReadonly = true)
        {
            //IVsInvisibleEditors are in-memory represenations of typical Visual Studio editors.
            //Language services, highlighting and error squiggles are hooked up to these editors
            //for us once we convert them to WpfTextViews. 
            var invisibleEditor = GetInvisibleEditor(filePath);

            var docDataPointer = IntPtr.Zero;
            Guid guidIVsTextLines = typeof(IVsTextLines).GUID;

            ErrorHandler.ThrowOnFailure(invisibleEditor.GetDocData(
                fEnsureWritable: 1
                , riid: ref guidIVsTextLines
                , ppDocData: out docDataPointer));

            IVsTextLines docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

            // This will actually be defined as _codewindowbehaviorflags2.CWB_DISABLEDIFF once the latest version of
            // Microsoft.VisualStudio.TextManager.Interop.16.0.DesignTime is published. Setting the flag will have no effect
            // on releases prior to d16.0.
            const _codewindowbehaviorflags CWB_DISABLEDIFF = (_codewindowbehaviorflags)0x04;

            //Create a code window adapter
            var codeWindow = EditorAdaptersFactoryService.CreateVsCodeWindowAdapter(OLEServiceProvider);

            // You need to disable the dropdown, splitter and -- for d16.0 -- diff since you are extracting the code window's TextViewHost and using it.
            ((IVsCodeWindowEx)codeWindow).Initialize((uint)_codewindowbehaviorflags.CWB_DISABLESPLITTER | (uint)_codewindowbehaviorflags.CWB_DISABLEDROPDOWNBAR | (uint)CWB_DISABLEDIFF,
                                                     VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter,
                                                     string.Empty,
                                                     string.Empty,
                                                     0,
                                                     new INITVIEW[1]);

            ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(docData));

            //Get a text view for our editor which we will then use to get the WPF control for that editor.
            IVsTextView textView;
            ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out textView));

            //We add our own role to this text view. Later this will allow us to selectively modify
            //this editor without getting in the way of Visual Studio's normal editors.
            var roles = TextEditorFactoryService.DefaultRoles.Concat(new string[] { DCI_BABY_IDE });

            var vsTextBuffer = docData as IVsTextBuffer;
            var textBuffer = EditorAdaptersFactoryService.GetDataBuffer(vsTextBuffer);

            if (textBuffer.Properties.ContainsProperty("StartPosition"))
            {
                textBuffer.Properties.RemoveProperty("StartPosition");
            }
            if (textBuffer.Properties.ContainsProperty("EndPosition"))
            {
                textBuffer.Properties.RemoveProperty("EndPosition");
            }

            textBuffer.Properties.AddProperty("StartPosition", start);
            textBuffer.Properties.AddProperty("EndPosition", start + length);
            var guid = VSConstants.VsTextBufferUserDataGuid.VsTextViewRoles_guid;
            ((IVsUserData)codeWindow).SetData(ref guid, TextEditorFactoryService.CreateTextViewRoleSet(roles).ToString());

            var textViewHost = EditorService.EditorAdaptersFactoryService.GetWpfTextViewHost(textView);
            return textViewHost;         
        }

        private uint _docCookie = 0;
        public void CloseEditor()
        {
            Cleanup();
        }

        private IVsInvisibleEditor _CurrentInvisibleEditor = null;
        private IVsInvisibleEditor GetInvisibleEditor(string filePath)
        {
            if (_CurrentInvisibleEditor == null)
            {
                IVsHierarchy hierarchy; 
                uint itemID;
                IntPtr docData;

                IVsInvisibleEditor editor;

                RunningDocumentTable.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, filePath, 
                    out hierarchy, out itemID, out docData, out _docCookie);

                ErrorHandler.ThrowOnFailure(InvisibleEditorManager.RegisterInvisibleEditor(filePath, null, 1, null, out editor));
                _CurrentInvisibleEditor = editor;
            }
            return _CurrentInvisibleEditor;
        }

        private void Cleanup()
        {
            if (_docCookie != 0)
                RunningDocumentTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, _docCookie);

            _docCookie = 0;
            _CurrentInvisibleEditor = null;
        }

        #endregion

        /// MEF is broken in VS2019
        /// https://developercommunity.visualstudio.com/content/problem/498617/projectionbuffertutorial-gives-error-in-dev16.html
        /// https://developercommunity.visualstudio.com/content/problem/460954/extensions-that-use-custom-itextviewmodelprovider.html
        /// Temporarily use this tutorial: https://github.com/JoshVarty/ProjectionBufferTutorial/blob/master/ProjectionBufferTutorial/ProjBufferToolWindow.cs

        #region MEF service provider point
        /// <summary>
        /// Whenever CSharp WpfTextViews are created with the CustomProjectionRole role
        /// this class will run and create a custom text view model for the WpfTextView
        /// </summary>
        [Export(typeof(ITextViewModelProvider)), ContentType("CSharp"), TextViewRole(DCI_BABY_IDE)]
        internal class DCIBabyIDETextViewModelProvider : ITextViewModelProvider
        {
            public ITextViewModel CreateTextViewModel(ITextDataModel dataModel, ITextViewRoleSet roles)
            {
                int start = (int)dataModel.DataBuffer.Properties.GetProperty("StartPosition");
                int end = (int)dataModel.DataBuffer.Properties.GetProperty("EndPosition");
                Span block = new Span(start, end-start);
                int leadingCharCount = this.CalculateLeadingWhitespace(dataModel.DataBuffer, block);
                char leadingCharacter = this.GetLeadingCharacter(dataModel.DataBuffer, block);
                return new DCIBabyIDETextViewModel(dataModel, this.CreateElisionBuffer(dataModel, start, end, leadingCharCount), leadingCharCount, leadingCharacter);
            }

            // Methods
            private int CalculateLeadingWhitespace(ITextBuffer buffer, Span block)
            {
                var line = (from n in buffer.CurrentSnapshot.Lines
                            where block.OverlapsWith((Span)n.Extent)
                            select n).FirstOrDefault<ITextSnapshotLine>();
                string text = line != null ?  line.GetText() : "";
                return this.CountLeadingWhiteSpace(text);
            }

            private int CountLeadingWhiteSpace(string line)
            {
                int num = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if ((line[i] != ' ') && (line[i] != '\t'))
                    {
                        return num;
                    }
                    num++;
                }
                return num;
            }

            public IElisionBuffer CreateElisionBuffer(ITextDataModel dataModel, int startPosition, int endPosition, int leadingCharCount)
            {
                int length = endPosition - startPosition;
                Span span = new Span(startPosition, length);
                SnapshotSpan span2 = new SnapshotSpan(dataModel.DataBuffer.CurrentSnapshot, span);
                return this._projectionBufferFactory.CreateElisionBuffer(null, new NormalizedSnapshotSpanCollection(span2), ElisionBufferOptions.None);
            }
            private char GetLeadingCharacter(ITextBuffer buffer, Span block)
            {
                var line = (from n in buffer.CurrentSnapshot.Lines
                               where block.OverlapsWith((Span)n.Extent)
                               select n).FirstOrDefault<ITextSnapshotLine>();

                string text = line != null ? line.GetText() : "";

                if (text.Length != 0)
                {
                    return text[0];
                }
                return ' ';
            }

            // Properties
            [Import]
            private IProjectionBufferFactoryService _projectionBufferFactory { get; set; }
        }

        internal class DCIBabyIDETextViewModel : ITextViewModel, IPropertyOwner, IDisposable
        {
            private readonly ITextDataModel _dataModel;
            private readonly IElisionBuffer _elisionBuffer;
            private char _leadingCharacter;
            private int _leadingCharCount;
            private readonly PropertyCollection _properties;

            // Methods
            public DCIBabyIDETextViewModel(ITextDataModel dataModel, IElisionBuffer elisionBuffer, int leadingCharCount, char leadingCharacter)
            {
                this._dataModel = dataModel;
                this._elisionBuffer = elisionBuffer;
                this._leadingCharCount = leadingCharCount;
                this._leadingCharacter = leadingCharacter;
                this._properties = new PropertyCollection();

                this.ElideIndentation();
            }

            private void ElideIndentation()
            {
                var spans = new List<Span>();
                foreach (Tuple<ITextSnapshotLine, ITextSnapshotLine> lineInfo in this._elisionBuffer.CurrentSnapshot.GetSnapshotLinesAndSourceLines(this.DataBuffer))
                {
                    ITextSnapshotLine snapshotLine = lineInfo.Item1;
                    ITextSnapshotLine bufferLine = lineInfo.Item2;
                    if ( ( (snapshotLine.Length > 0) && (snapshotLine.Length > this._leadingCharCount) ) &&
                         ( (snapshotLine.GetText().Substring(0, this._leadingCharCount).Trim().Length == 0) &&  ( ( snapshotLine.Length - this._leadingCharCount) != bufferLine.Length) ) )
                    {
                        spans.Add(new Span(bufferLine.Start.Position, this._leadingCharCount));
                    }
                }
                this._elisionBuffer.ElideSpans(new NormalizedSpanCollection(spans));
            }

            public void Dispose()
            {
            }

            public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
            {
                return editBufferPoint;
            }

            public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode)
            {
                return editBufferPoint.TranslateTo(targetVisualSnapshot, trackingMode);
            }


            public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
            {
                return true;
            }


            // Properties
            public ITextBuffer DataBuffer
            {
                get
                {
                    return this._dataModel.DataBuffer;
                }
            }

            public ITextDataModel DataModel
            {
                get
                {
                    return this._dataModel;
                }
            }

            public ITextBuffer EditBuffer
            {
                get
                {
                    return this._elisionBuffer;
                }
            }

            public PropertyCollection Properties
            {
                get
                {
                    return this._properties;
                }
            }

            public ITextBuffer VisualBuffer
            {
                get
                {
                    return this._elisionBuffer;
                }
            }
        }
        #endregion
    }

    public static class IElisionSnapshotExtensions
    {
        public static IEnumerable<Tuple<ITextSnapshotLine, ITextSnapshotLine>> GetSnapshotLinesAndSourceLines(this IElisionSnapshot snapshot, ITextBuffer source)
        {
            return (from editLine in snapshot.Lines
                    let position = snapshot.MapToSourceSnapshot(editLine.Start.Position)
                    select new { editLine, dataLine = source.CurrentSnapshot.GetLineFromPosition(position) }).Select(x => Tuple.Create<ITextSnapshotLine, ITextSnapshotLine>(x.editLine, x.dataLine)).ToList<Tuple<ITextSnapshotLine, ITextSnapshotLine>>();
        }
    }

}