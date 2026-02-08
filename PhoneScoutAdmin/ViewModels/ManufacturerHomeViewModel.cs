using System.ComponentModel;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class ManufacturerHomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand ShowPhonesCommand { get; }
        public ICommand ShowManufacturerCommand { get; }

        public ManufacturerHomeViewModel()
        {
            ShowPhonesCommand = new RelayCommand(() =>
                CurrentView = new SingleManufacturerViewModel(showPhones: true));

            ShowManufacturerCommand = new RelayCommand(() =>
                CurrentView = new SingleManufacturerViewModel(showPhones: false));

            // default view
            CurrentView = new SingleManufacturerViewModel(showPhones: true);
        }
    }
}
