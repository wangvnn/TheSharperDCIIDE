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
    public class InterfaceViewModel : ViewModelBase<DCIRoleInterface>
    {
        public InterfaceViewModel(DCIRoleInterface model)
            : base(model)
        {
            foreach (var s in model.Signatures)
            {
                Signatures.Add(new SignatureViewModel(s.Value));
            }
        }

        private ObservableCollection<SignatureViewModel> _Signatures = new ObservableCollection<SignatureViewModel>();

        public ObservableCollection<SignatureViewModel> Signatures
        {
            get
            {
                return _Signatures;
            }
            set
            {
                _Signatures = value;
            }
        }
    }
}
