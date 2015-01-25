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
                var vm = this.DataContext as RoleViewModel;
                if (vm.Interface != null && vm.Interface.SelectedSignature != null)
                {
                    RoleViewModelRoutedCommands.SelectCode.Execute(vm.Interface.SelectedSignature.Model, item);
                }
            }
            e.Handled = false;
        }

        private void RoleMethod_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var vm = this.DataContext as RoleViewModel;
                if (vm.SelectedMethod != null)
                {
                    RoleViewModelRoutedCommands.SelectCode.Execute(vm.SelectedMethod.Model, item);
                }
            }
            e.Handled = false;
        }
    }
}
