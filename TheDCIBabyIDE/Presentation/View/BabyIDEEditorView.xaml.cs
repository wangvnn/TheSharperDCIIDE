using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphSharp.Controls;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel;
using QuickGraph;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.View
{
    /// <summary>
    /// Interaction logic for BabyIDEEditorView.xaml
    /// </summary>
    public partial class BabyIDEEditorView : UserControl
    {
        public BabyIDEEditorView()
        {
            System.Reflection.Assembly.Load("WPFExtensions, Version=1.0.3437.34043, Culture=neutral, PublicKeyToken=null");
            //System.Reflection.Assembly.Load("GraphSharp.Controls, Version=1.0.5506.20680, Culture=neutral, PublicKeyToken=null");

            InitializeComponent();

            InteractionGraphView.DataContextChanged += new DependencyPropertyChangedEventHandler(UserControl1_DataContextChanged);
        }

        void UserControl1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var model = (InteractionGraphView.DataContext as ContextViewModel);
            if (model != null)
            {
                InteractionGraphView.Graph = model.InteractionGraph;
                InteractionGraphView.LayoutAlgorithmType = "CompoundFDP";
                InteractionGraphView.AsyncCompute = true;
                InteractionGraphView.ShowAllStates = false;
                InteractionGraphView.HighlightAlgorithmType = "Simple";
                InteractionGraphView.OverlapRemovalConstraint = AlgorithmConstraints.Automatic;
                InteractionGraphView.OverlapRemovalAlgorithmType = "FSA";
                InteractionGraphView.OverlapRemovalConstraint = AlgorithmConstraints.Must;               
            }
        }

        private void Graph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var item = sender as RoleInteractionGraph;
                if (item != null)
                {
                    var vm = this.DataContext as ContextViewModel;
                    RoleViewModelRoutedCommands.SelectCode.Execute(vm.Model, sender as IInputElement);
                }
            }
        }

        private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as RoleView;
            if (item != null)
            {
                var vm = item.DataContext as RoleViewModel;
                if (vm != null)
                {
                    RoleViewModelRoutedCommands.SelectCode.Execute(vm.Model, item);
                }
            }
        }

        private void DragCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
    

}
