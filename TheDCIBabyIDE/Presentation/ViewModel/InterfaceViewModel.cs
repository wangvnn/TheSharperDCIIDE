using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel
{
    public class InterfaceViewModel : ViewModelBase<DCIRoleInterface>
    {
        public InterfaceViewModel(DCIRoleInterface model)
            : base(model)
        {
            Show(false);
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

        private SignatureViewModel _SelectedSignature;
        public SignatureViewModel SelectedSignature
        {
            get
            {
                return _SelectedSignature;
            }
            set
            {
                _SelectedSignature = value;
                RaisePropertyChangedEvent("SelectedSignature");
            }       

        }

        public void Show(bool show)
        {
            Signatures.Clear();
            if (show)
            {
                
                foreach (var s in Model.Signatures)
                {
                    Signatures.Add(new SignatureViewModel(s.Value));
                }
            }
        }
    }
}
