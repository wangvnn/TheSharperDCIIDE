using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel
{
    public class RoleViewModel : ViewModelBase<DCIRole>
    {
        public int ZIndex { get; set; }
        public double CanvasLeft { get; set; }
        public double CanvasTop { get; set; }


        private ObservableCollection<MethodViewModel> _Methods = new ObservableCollection<MethodViewModel>();

        public ObservableCollection<MethodViewModel> Methods
        {
            get
            {
                return _Methods;
            }
            set
            {
                _Methods = value;
            }
        }

        public InterfaceViewModel Interface { get; private set; }

        public RoleViewModel(DCIRole model)
            :base(model)
        {
            if (model.Interface != null)
                Interface = new InterfaceViewModel(model.Interface);
            foreach(var m in model.Methods)
            {
                Methods.Add(new MethodViewModel(m.Value));
            }
        }
    }
}
