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
        public ScreenViewModel ScreenVM { get; } = new ScreenViewModel();
        public ConnectivityViewModel ConnectivityVM { get; } = new ConnectivityViewModel();
        public BatteryChargingViewModel BatteryChargingVM { get; } = new BatteryChargingViewModel();
        public BodySpeakerViewModel BodySpeakerVM { get; } = new BodySpeakerViewModel();
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
        public ICommand ShowScreenCommand { get; }
        public ICommand ShowConnectivityCommand { get; }
        public ICommand ShowBatteryChargingCommand { get; }
        public ICommand ShowBodySpeakerCommand { get; }

        public double TotalProgress => CpuVM.Progress + SensorsVM.Progress + ConnectivityVM.Progress + ScreenVM.Progress; // expand later

        public PhoneDetailsViewModel()
        {
            ShowCpuCommand = new RelayCommand(() => CurrentViewModel = CpuVM);
            ShowSensorsCommand = new RelayCommand(() => CurrentViewModel = SensorsVM);
            ShowConnectivityCommand = new RelayCommand(() => CurrentViewModel = ConnectivityVM);
            ShowScreenCommand = new RelayCommand(() => CurrentViewModel = ScreenVM);
            ShowBatteryChargingCommand = new RelayCommand(() => CurrentViewModel = BatteryChargingVM);
            ShowBodySpeakerCommand = new RelayCommand(() => CurrentViewModel = BodySpeakerVM);

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
            ConnectivityVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ConnectivityVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            ScreenVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ScreenVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            BatteryChargingVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BatteryChargingVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            BodySpeakerVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BodySpeakerVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };

            // Default view
            CurrentViewModel = CpuVM;
        }
    }
}