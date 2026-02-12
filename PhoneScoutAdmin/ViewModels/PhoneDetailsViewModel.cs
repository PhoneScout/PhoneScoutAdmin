using PhoneScoutAdmin.ViewModels;
using System.ComponentModel;
using System.Windows.Input;

namespace PhoneScoutAdmin
{
    public class PhoneDetailsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // SECTION VMs
        public CPUdetailsViewModel CpuVM { get; } = new CPUdetailsViewModel();
        public SensorsViewModel SensorsVM { get; } = new SensorsViewModel();
        // Add others later...

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        // COMMANDS
        public ICommand ShowCpuCommand { get; }
        public ICommand ShowSensorsCommand { get; }

        public double TotalProgress => CpuVM.Progress + SensorsVM.Progress; // expand later

        public PhoneDetailsViewModel()
        {
            ShowCpuCommand = new RelayCommand(() => CurrentViewModel = CpuVM);
            ShowSensorsCommand = new RelayCommand(() => CurrentViewModel = SensorsVM);

            CpuVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CPUdetailsViewModel.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            SensorsVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SensorsVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };

            // Default view
            CurrentViewModel = CpuVM;
        }
    }
}
