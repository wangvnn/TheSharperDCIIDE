using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.View
{
    /// <summary>
    /// Interaction logic for BabyIDEEditorView.xaml
    /// </summary>
    public partial class BabyIDEEditorView : UserControl
    {
        public BabyIDEEditorView()
        {
            InitializeComponent();
        }


        private void Role_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var vm = this.DataContext as ContextViewModel;
                if (vm.SelectedItem != null)
                {
                    RoleViewModelRoutedCommands.SelectCode.Execute(vm.SelectedItem.Model, item);
                }
            }
            e.Handled = false;
        }

        private void DragCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var vm = this.DataContext as ContextViewModel;
                RoleViewModelRoutedCommands.SelectCode.Execute(vm.Model, sender as IInputElement);
            }
            e.Handled = false;
        }
    }
}
