using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using System.Collections.ObjectModel;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel
{
    public class ContextViewModel : ViewModelBase<DCIContext>
    {
        public ContextViewModel(DCIContext model)
            : base(model)
        {
            Layout();
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

            }
        }

    }
}
