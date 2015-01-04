using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KimHaiQuang.TheDCIBabyIDE.Presentation.Operation;

namespace KimHaiQuang.TheDCIBabyIDE
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class BabyIDEEditor : 
        UserControl, ContextFileDisplayingContext.IContextFileViewerRole
    {
        public BabyIDEEditor()
        {
            InitializeComponent();
        }

        public ContentControl UsecaseView { get { return _UsecaseView; } }
        public ContentControl ProjectedCodeView { get { return _ProjectedCodeView; } }
        public FrameworkElement InteractionView { get { return _InteractionView; } }
    }
}