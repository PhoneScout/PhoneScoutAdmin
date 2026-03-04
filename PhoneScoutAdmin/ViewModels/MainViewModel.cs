using PhoneScoutAdmin.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        public EventViewModel EventVM { get; }

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
        public ICommand ShowEventsCommand { get; }

        public ICommand SignOut { get; }
        public ICommand ExitApp { get; }


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
            EventVM = new EventViewModel(true);

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

            ShowEventsCommand = new RelayCommand(() =>
            {
                CurrentViewModel = EventVM;
                EventVM.LoadEventsCommand?.Execute(null);
            });

            SignOut = new RelayCommand(() =>
            {
                var result = MessageBox.Show(
                "Are you sure you want to sign out?",
                "Confirm Sign Out",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var loginWindow = new LoginView
                    {
                        Width = 275,
                        Height = 580,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        ResizeMode = ResizeMode.NoResize
                    };

                    loginWindow.Show();

                    // Close the window hosting this UserControl
                    foreach (Window window in Application.Current.Windows)
                    {
                        // Check if the window's DataContext is THIS ViewModel
                        if (window.DataContext == this)
                        {
                            window.Close(); // ✅ closes the window hosting this UserControl
                            break;
                        }
                    }
                }
                
            });

            ExitApp = new RelayCommand(() =>
            {
                var result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            });

            // Default view
            CurrentViewModel = PhoneVM;
        }
    }
}
