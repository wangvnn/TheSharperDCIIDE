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
    public class RoleViewModelRoutedCommands
    {
        public static readonly RoutedUICommand SelectCode =
            new RoutedUICommand("To select a CodeSpane", "SelectCode", typeof(RoleViewModelRoutedCommands));

        public static readonly RoutedUICommand ToggleInterfaceView =
            new RoutedUICommand("Toggle Interface View", "ToggleInterfaceView", typeof(RoleViewModelRoutedCommands));
    }

    public class RoleViewModel : ViewModelBase<DCIRole>
    {
        public int _ZIndex = 0;
        public int ZIndex
        {
            get
            {
                return _ZIndex;
            }
            set
            {
                _ZIndex = value;
                RaisePropertyChangedEvent("ZIndex");
            }
        }

        public double _CanvasTop = 0;
        public double CanvasTop
        {
            get
            {
                return _CanvasTop;
            }
            set
            {
                _CanvasTop = value;
                RaisePropertyChangedEvent("CanvasTop");
            }
        }

        public double _CanvasLeft = 0;
        public double CanvasLeft
        {
            get
            {
                return _CanvasLeft;
            }
            set
            {
                _CanvasLeft = value;
                RaisePropertyChangedEvent("CanvasLeft");
            }
        }

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

        private MethodViewModel _SelectedMethod = null;
        public MethodViewModel SelectedMethod
        {
            get
            {
                return _SelectedMethod;
            }
            set
            {
                _SelectedMethod = value;
                RaisePropertyChangedEvent("SelectedMethod");

            }
        }

        private InterfaceViewModel _Interface;
        public InterfaceViewModel Interface 
        { 
            get
            {
                return _Interface;
            } 
            private set
            {
                _Interface = value;
                RaisePropertyChangedEvent("Interface");
                RaisePropertyChangedEvent("HasInterface");
            }
        }
        public bool HasInterface { get { return _Interface != null; } }

        public RoleViewModel(DCIRole model)
            :base(model)
        {
            if (model.Interface != null)
            {
                Interface = new InterfaceViewModel(model.Interface);
            }

            foreach(var m in model.Methods)
            {
                Methods.Add(new MethodViewModel(m.Value));
            }
        }

        private bool _ShowInterface = false;

        public void ToggleInterfaceView()
        {
            _ShowInterface = !_ShowInterface;

            if (Interface != null)
                Interface.Show(_ShowInterface);
        }
    }
}
