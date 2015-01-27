using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using System.Collections.ObjectModel;
using KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base;
using Microsoft.VisualStudio.Text;
using System;
using QuickGraph;
using System.Linq;

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

        public void UnRegisterRoutedCommandHandlers()
        {
            base.UnregisterCommand(RoleViewModelRoutedCommands.SelectCode);
        }

        private void RegisterRoutedCommandHandlers()
        {
            base.RegisterCommand(
                            RoleViewModelRoutedCommands.SelectCode,
                            param => { return true; },
                            param => this.SelectCode(param as SpanObject));
        }

        private void SelectCode(SpanObject codeSpan)
        {
            if (codeSpan != null && ChangeCodeSpanRequest != null)
                ChangeCodeSpanRequest(codeSpan, EventArgs.Empty);
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


        private BidirectionalGraph<RoleViewModel, IEdge<RoleViewModel>> _Graph;
        public BidirectionalGraph<RoleViewModel, IEdge<RoleViewModel>> InteractionGraph
        {
            get { return _Graph;  }
            set
            {
                _Graph = value;
                RaisePropertyChangedEvent("InteractionGraph");
            }
        }
        private void Layout()
        {
            InteractionGraph = new BidirectionalGraph<RoleViewModel, IEdge<RoleViewModel>>();


            for  (int zindex = Model.Roles.Values.Count-1; zindex >=0; --zindex)
            {
                var r = Model.Roles.Values.ElementAt(zindex);

                var roleViewModel = new RoleViewModel(r);
                Roles.Add(roleViewModel);

                roleViewModel.ZIndex = zindex;
                SelectedItem = SelectedItem == null ? roleViewModel : SelectedItem;

                _Graph.AddVertex(roleViewModel);
            }

            foreach (var interaction in Model.Interactions)
            {
                var source = Roles.FirstOrDefault(r => r.Model == interaction.Value.Source);
                var target = Roles.FirstOrDefault(r => r.Model == interaction.Value.Target);
                _Graph.AddEdge(new Edge<RoleViewModel>(source, target));
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
        public delegate void ChangCodeSpanRequestHandler(object sender, EventArgs e);
        public event ChangCodeSpanRequestHandler ChangeCodeSpanRequest;
    }
}
