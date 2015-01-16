using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel.Base
{
    public class ViewModelBase<T> : CommandSink, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public ViewModelBase(T model)
        {
            Model = model;
        }

        public T Model { get; private set; }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            string propertyName = binder.Name;
            PropertyInfo property =
              this.Model.GetType().GetProperty(propertyName);

            if (property == null || property.CanWrite == false)
                return false;

            property.SetValue(this.Model, value, null);

            this.RaisePropertyChangedEvent(propertyName);
            return true;
        }

        public override bool TryGetMember(
          GetMemberBinder binder, out object result)
        {

            string propertyName = binder.Name;
            PropertyInfo property =
              this.Model.GetType().GetProperty(propertyName);

            if (property == null || property.CanRead == false)
            {
                result = null;
                return false;
            }

            result = property.GetValue(this.Model, null);
            return true;
        }
    }
}
