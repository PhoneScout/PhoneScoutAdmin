using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // VIEWMODELS
        // ======================
        public PhoneViewModel PhoneVM { get; }
        public ManufacturerViewModel ManufacturerVM { get; }
        public UserViewModel UserVM { get; }
        public StorageViewModel StorageVM { get; }
        public RepairViewModel RepairVM { get; }
        public OrderViewModel OrderVM { get; }

        // ======================
        // CURRENT VIEW
        // ======================
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

        // ======================
        // COMMANDS
        // ======================
        public ICommand ShowPhonesCommand { get; }
        public ICommand ShowManufacturersCommand { get; }
        public ICommand ShowUsersCommand { get; }
        public ICommand ShowStorageCommand { get; }
        public ICommand ShowRepairsCommand { get; }
        public ICommand ShowOrdersCommand { get; }

        // ======================
        // CONSTRUCTOR
        // ======================
        public MainViewModel()
        {
            // Initialize all viewmodels
            PhoneVM = new PhoneViewModel();
            ManufacturerVM = new ManufacturerViewModel();
            UserVM = new UserViewModel();
            StorageVM = new StorageViewModel();
            RepairVM = new RepairViewModel();
            OrderVM = new OrderViewModel();

            // Commands to switch views
            ShowPhonesCommand = new RelayCommand(() =>
            {
                CurrentViewModel = PhoneVM;
                PhoneVM.LoadPhonesCommand?.Execute(null);
            });

            ShowManufacturersCommand = new RelayCommand(() =>
            {
                CurrentViewModel = ManufacturerVM;
                ManufacturerVM.LoadManufacturersCommand?.Execute(null);
            });

            ShowUsersCommand = new RelayCommand(() =>
            {
                CurrentViewModel = UserVM;
                UserVM.LoadUsersCommand?.Execute(null);
            });

            ShowStorageCommand = new RelayCommand(() =>
            {
                CurrentViewModel = StorageVM;
                StorageVM.LoadPartsCommand?.Execute(null);
            });

            ShowRepairsCommand = new RelayCommand(() =>
            {
                CurrentViewModel = RepairVM;
                RepairVM.LoadRepairsCommand?.Execute(null);
            });

            ShowOrdersCommand = new RelayCommand(() =>
            {
                CurrentViewModel = OrderVM;
                OrderVM.LoadOrdersCommand?.Execute(null);
            });

            // Default view
            CurrentViewModel = RepairVM;
        }
    }
}
