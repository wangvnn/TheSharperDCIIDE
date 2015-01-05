using System.ComponentModel;


namespace KimHaiQuang.TheDCIBabyIDE.Presentation.ViewModel
{
    public class ViewModelBase : CommandSink, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
