using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using System.Collections.ObjectModel;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel
{
    public class ContextViewModel : ViewModelBase<DCIContext>
    {
        public ContextViewModel()
        {
        }

        public ContextViewModel(DCIContext model)
            : base(model)
        {
            Layout();
            RegisterRoutedCommandHandlers();
        }

        private void RegisterRoutedCommandHandlers()
        {
            base.RegisterCommand(
                            RoleViewModelRoutedCommands.SelectCommand,
                            param => { return true; },
                            param => this.SelectRole(param as RoleViewModel));
        }

        private void SelectRole(RoleViewModel role)
        {
            
        }

        private ObservableCollection<RoleViewModel> _Roles = new ObservableCollection<RoleViewModel>();

        public ObservableCollection<RoleViewModel> Roles
        {
            get
            {
                return _Roles;
            }
            set
            {
                _Roles = value;
            }
        }

        private void Layout()
        {
            int zindex = 0;
            foreach (var r in Model.Roles)
            {
                var roleViewModel = new RoleViewModel(r.Value);
                Roles.Add(roleViewModel);

                zindex++;

                roleViewModel.ZIndex = zindex;
                roleViewModel.CanvasLeft = zindex * 100.0;
                roleViewModel.CanvasTop = zindex * 100.0;

                SelectedItem = roleViewModel;
            }
        }
        private RoleViewModel _SelectedItem;
        public RoleViewModel SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                EnsureSelectedItemOnTop();
                RaisePropertyChangedEvent("SelectedItem");
            }
        }

        private void EnsureSelectedItemOnTop()
        {
            RoleViewModel maxZIndex = null;
            foreach (var rvm in Roles)
            {
                if (maxZIndex == null || rvm.ZIndex > maxZIndex.ZIndex)
                {
                    maxZIndex = rvm;
                }
            }

            int zindex = -1;

            zindex = _SelectedItem != null ? _SelectedItem.ZIndex : zindex;

            if (_SelectedItem != null && maxZIndex != null)
                _SelectedItem.ZIndex = maxZIndex.ZIndex;

            if (maxZIndex != null && zindex != -1)
                maxZIndex.ZIndex = zindex;
        }
    }
}
