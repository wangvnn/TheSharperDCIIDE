using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel
{
    public class SignatureViewModel : ViewModelBase<DCIInterfaceSignature>
    {
        public SignatureViewModel(DCIInterfaceSignature model)
            : base(model)
        {

        }
    }
}
