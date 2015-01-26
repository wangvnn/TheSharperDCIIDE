using System.Windows.Controls;
using System.Windows.Input;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.View
{
    /// <summary>
    /// Interaction logic for RoleView.xaml
    /// </summary>
    public partial class RoleView : UserControl
    {
        public RoleView()
        {
            InitializeComponent();
        }

        private void RoleInterface_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var vm = item.DataContext as SignatureViewModel;
                if (vm != null)
                {
                    RoleViewModelRoutedCommands.SelectCode.Execute(vm.Model, item);
                }
            }
            e.Handled = false;
        }

        private void RoleMethod_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var vm = item.DataContext as MethodViewModel;
                if (vm != null)
                {
                    RoleViewModelRoutedCommands.SelectCode.Execute(vm.Model, item);
                }
            }
            e.Handled = false;
        }
    }
}
