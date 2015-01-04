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

namespace KimHaiQuang.TheDCIBabyIDE.Infrastructure.Services
{
    public class EditorService :
        ContextFileDisplayingContext.IEditorFactory
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

        public void CreateProjectionEditor(string filePath, int start, int length, out IWpfTextViewHost host, out IVsTextView view)
        {
            IntPtr zero = IntPtr.Zero;
            Guid gUID = typeof(IVsTextLines).GUID;
            ErrorHandler.ThrowOnFailure(this.GetInvisibleEditor(filePath).GetDocData(1, ref gUID, out zero));

            IVsTextLines objectForIUnknown = (IVsTextLines)Marshal.GetObjectForIUnknown(zero);

            IVsCodeWindow window = EditorAdaptersFactoryService.CreateVsCodeWindowAdapter(OLEServiceProvider);
            ErrorHandler.ThrowOnFailure(window.SetBuffer(objectForIUnknown));
            ErrorHandler.ThrowOnFailure(window.GetPrimaryView(out view));

            // NOTE: using MEF 
            // add new ROLE to mark this as DCI BABY IDE so that we can provide MEF service provider to provide as special ViewModel for it.

            string[] dciIDERole = new string[] { DCI_BABY_IDE };
            IEnumerable<string> roles = TextEditorFactoryService.DefaultRoles.Where<string>( role => role != "ZOOMABLE").Concat<string>(dciIDERole);
            Guid riidKey = VSConstants.VsTextBufferUserDataGuid.VsTextViewRoles_guid;

            // NOTE: using MEF
            // this TextView creation will look for our TextViewModel Provider to provide TextViewModel
            ((IVsUserData)window).SetData(ref riidKey, TextEditorFactoryService.CreateTextViewRoleSet(roles).ToString());

            IVsTextBuffer bufferAdapter = objectForIUnknown;
            ITextBuffer dataBuffer = EditorAdaptersFactoryService.GetDataBuffer(bufferAdapter);

            if (dataBuffer.Properties.ContainsProperty("StartPosition"))
            {
                dataBuffer.Properties.RemoveProperty("StartPosition");
            }
            if (dataBuffer.Properties.ContainsProperty("EndPosition"))
            {
                dataBuffer.Properties.RemoveProperty("EndPosition");
            }

            dataBuffer.Properties.AddProperty("StartPosition", start);
            dataBuffer.Properties.AddProperty("EndPosition", start + length);

            host = EditorService.EditorAdaptersFactoryService.GetWpfTextViewHost(view);
        }

        private IVsInvisibleEditor _CurrentInvisibleEditor = null;
        private IVsInvisibleEditor GetInvisibleEditor(string filePath)
        {
            if (_CurrentInvisibleEditor == null)
            {
                IVsHierarchy hierarchy;
                uint num;
                IntPtr ptr;
                uint num2;
                IVsInvisibleEditor editor;
                RunningDocumentTable.FindAndLockDocument(1, filePath, out hierarchy, out num, out ptr, out num2);
                ErrorHandler.ThrowOnFailure(InvisibleEditorManager.RegisterInvisibleEditor(filePath, null, 1, null, out editor));
                _CurrentInvisibleEditor = editor;
            }
            return _CurrentInvisibleEditor;
        }

        private void Cleanup()
        {
            _CurrentInvisibleEditor = null;
        }

        #endregion

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
                //Create a projection buffer based on the specified start and end position.
                var projectionBuffer = CreateProjectionBuffer(dataModel);
                //Display this projection buffer in the visual buffer, while still maintaining
                //the full file buffer as the underlying data buffer.
                var textViewModel = new DCIBabyIDETextViewModel(dataModel, projectionBuffer);
                return textViewModel;
            }

            private IProjectionBuffer CreateProjectionBuffer(ITextDataModel dataModel)
            {
                //retrieve start and end position that we saved in MyToolWindow.CreateEditor()
                var startPosition = (int)dataModel.DataBuffer.Properties.GetProperty("StartPosition");
                var endPosition = (int)dataModel.DataBuffer.Properties.GetProperty("EndPosition");
                var length = endPosition - startPosition;

                //Take a snapshot of the text within these indices.
                var textSnapshot = dataModel.DataBuffer.CurrentSnapshot;
                var trackingSpan = textSnapshot.CreateTrackingSpan(startPosition, length, SpanTrackingMode.EdgeExclusive);

                //Create the actual projection buffer
                var projectionBuffer = ProjectionBufferFactory.CreateProjectionBuffer(
                    null
                    , new List<object>() { trackingSpan }
                    , ProjectionBufferOptions.None
                    );
                return projectionBuffer;
            }


            [Import]
            public IProjectionBufferFactoryService ProjectionBufferFactory { get; set; }
        }

        internal class DCIBabyIDETextViewModel : ITextViewModel
        {
            private readonly ITextDataModel _dataModel;
            private readonly IProjectionBuffer _projectionBuffer;
            private readonly PropertyCollection _properties;

            public DCIBabyIDETextViewModel(ITextDataModel dataModel, IProjectionBuffer projectionBuffer)
            {
                this._dataModel = dataModel;
                this._projectionBuffer = projectionBuffer;
                this._properties = new PropertyCollection();
            }

            //The underlying source buffer from which the projection was created
            public ITextBuffer DataBuffer
            {
                get
                {
                    return _dataModel.DataBuffer;
                }
            }

            public ITextDataModel DataModel
            {
                get
                {
                    return _dataModel;
                }
            }

            public ITextBuffer EditBuffer
            {
                get
                {
                    return _projectionBuffer;
                }
            }

            // Displays our projection 
            public ITextBuffer VisualBuffer
            {
                get
                {
                    return _projectionBuffer;
                }
            }

            public PropertyCollection Properties
            {
                get
                {
                    return _properties;
                }
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
        }

        #endregion
    }
}