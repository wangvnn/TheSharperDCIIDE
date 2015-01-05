using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel
{
    public class ContextInteractionViewModel : ViewModelBase
    {
        public ContextInteractionViewModel(DCIContext model)
        {
            Model = model;
        }

        public DCIContext Model { get; private set; }
    }
}
