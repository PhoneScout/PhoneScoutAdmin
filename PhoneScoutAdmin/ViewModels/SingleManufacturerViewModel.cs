using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace PhoneScoutAdmin.ViewModels
{
    public enum ManufacturerSubView
    {
        Phones,
        ManufacturerDetails,
        Events
    }

    public class SingleManufacturerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // =========================
        // SUBVIEW MANAGEMENT
        // =========================

        private ManufacturerSubView _currentSubView;
        public ManufacturerSubView CurrentSubView
        {
            get => _currentSubView;
            set
            {
                _currentSubView = value;
                OnPropertyChanged(nameof(CurrentSubView));
                UpdateCurrentView();
            }
        }

        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        private void UpdateCurrentView()
        {
            switch (CurrentSubView)
            {
                case ManufacturerSubView.Phones:
                    CurrentView = new SingleManufacturerPhonesView { DataContext = this };
                    break;
                case ManufacturerSubView.ManufacturerDetails:
                    CurrentView = new SingleManufacturerView { DataContext = this };
                    break;
                case ManufacturerSubView.Events:
                    var eventsView = new SingleManufacturerEventView();
                    eventsView.DataContext = EventsVM;
                    CurrentView = eventsView;
                    break;
            }
        }

        // =========================
        // DATA
        // =========================

        public ObservableCollection<Phone> Phones { get; } = new();
        public EventViewModel EventsVM { get; } = new EventViewModel();
        public Manufacturer ActiveManufacturer { get; } = new();

        private Phone _selectedPhone;
        public Phone SelectedPhone
        {
            get => _selectedPhone;
            set
            {
                _selectedPhone = value;
                LoadPhoneToEditor();
                OnPropertyChanged(nameof(SelectedPhone));
                SavePhoneCommand.RaiseCanExecuteChanged();
                DeletePhoneCommand.RaiseCanExecuteChanged();
                RaiseCommandStates();

                //UpdatePhoneCommand.RaiseCanExecuteChanged();
            }
        }

        // =========================
        // PHONE EDIT FIELDS
        // =========================

        private string _phoneName;
        public string PhoneName
        {
            get => _phoneName;
            set { _phoneName = value; OnPropertyChanged(nameof(PhoneName)); }
        }

        private int _phonePrice;
        public int PhonePrice
        {
            get => _phonePrice;
            set { _phonePrice = value; OnPropertyChanged(nameof(PhonePrice)); }
        }

        private bool _phoneInStore;
        public bool PhoneInStore
        {
            get => _phoneInStore;
            set { _phoneInStore = value; OnPropertyChanged(nameof(PhoneInStore)); }
        }

        private bool _phoneAvailable;
        public bool PhoneAvailable
        {
            get => _phoneAvailable;
            set { _phoneAvailable = value; OnPropertyChanged(nameof(PhoneAvailable)); }
        }

        // =========================
        // MANUFACTURER EDIT FIELDS
        // =========================

        private string _manufacturerName;
        public string ManufacturerName
        {
            get => _manufacturerName;
            set { _manufacturerName = value; OnPropertyChanged(nameof(ManufacturerName)); }
        }

        private string _manufacturerEmail;
        public string ManufacturerEmail
        {
            get => _manufacturerEmail;
            set { _manufacturerEmail = value; OnPropertyChanged(nameof(ManufacturerEmail)); }
        }

        private string _manufacturerUrl;
        public string ManufacturerUrl
        {
            get => _manufacturerUrl;
            set { _manufacturerUrl = value; OnPropertyChanged(nameof(ManufacturerUrl)); }
        }

        // =========================
        // COMMANDS
        // =========================

        public RelayCommand ShowPhonesCommand { get; }
        public RelayCommand ShowManufacturerCommand { get; }
        public RelayCommand ShowEventsCommand { get; }

        public RelayCommand SavePhoneCommand { get; }
        public RelayCommand DeletePhoneCommand { get; }

        public ICommand CreatePhoneCommand { get; }
        public ICommand UpdatePhoneCommand { get; }
        public ICommand SaveManufacturerCommand { get; }

        public ICommand SignOut { get; }
        public ICommand ExitApp { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        private string _activeManufacturerName;
        public string ActiveManufacturerName
        {
            get => _activeManufacturerName;
            set { _activeManufacturerName = value; OnPropertyChanged(nameof(ActiveManufacturerName)); }
        }

        public SingleManufacturerViewModel(string loggedInManufacturer)
        {
            ActiveManufacturerName = loggedInManufacturer;

            // Initialize view
            CurrentSubView = ManufacturerSubView.Phones;

            ShowPhonesCommand = new RelayCommand(() => CurrentSubView = ManufacturerSubView.Phones);
            ShowManufacturerCommand = new RelayCommand(() => CurrentSubView = ManufacturerSubView.ManufacturerDetails);
            ShowEventsCommand = new RelayCommand(() => CurrentSubView = ManufacturerSubView.Events);

            SavePhoneCommand = new RelayCommand(async () => await SavePhone(), () => SelectedPhone != null);
            DeletePhoneCommand = new RelayCommand(async () => await DeletePhone(), () => SelectedPhone != null);
            SaveManufacturerCommand = new RelayCommand(async () => await SaveManufacturer());

            UpdatePhoneCommand = new RelayCommand(async () => await OpenUpdatePhoneWindow(), () => SelectedPhone != null);

            CreatePhoneCommand = new RelayCommand(OpenCreatePhoneWindow);

            _ = LoadPhones();
            _ = LoadManufacturer();
            _ = EventsVM.LoadEvents(); // preload events

            SignOut = new RelayCommand(SignOutLogic);
            ExitApp = new RelayCommand(() => Application.Current.Shutdown());
        }

        private void RaiseCommandStates()
        {
            (SavePhoneCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // =========================
        // LOGIC
        // =========================

        private async Task LoadPhones()
        {
            using HttpClient client = new();
            var json = await client.GetStringAsync("http://localhost:5175/api/wpfPhone");
            var list = JsonSerializer.Deserialize<ObservableCollection<Phone>>(json);

            Phones.Clear();
            foreach (var phone in list)
            {
                if (phone.manufacturerName == ActiveManufacturerName && phone.phoneAvailable == 0)
                    Phones.Add(phone);
            }
        }

        private async Task LoadManufacturer()
        {
            using HttpClient client = new();
            var json = await client.GetStringAsync("http://localhost:5175/api/wpfManufacturer");
            var list = JsonSerializer.Deserialize<ObservableCollection<Manufacturer>>(json);

            var manufacturer = list?.FirstOrDefault(m => m.manufacturerName == ActiveManufacturerName);
            if (manufacturer == null) return;

            ActiveManufacturer.manufacturerId = manufacturer.manufacturerId;
            ManufacturerName = manufacturer.manufacturerName;
            ManufacturerEmail = manufacturer.manufacturerEmail;
            ManufacturerUrl = manufacturer.manufacturerUrl;
        }

        private void LoadPhoneToEditor()
        {
            if (SelectedPhone == null) return;
            PhoneName = SelectedPhone.phoneName;
        }

        private async Task SavePhone()
        {
            SelectedPhone.phoneName = PhoneName;
            using HttpClient client = new();
            var json = JsonSerializer.Serialize(SelectedPhone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PutAsync($"http://localhost:5175/api/wpfPhone/{SelectedPhone.phoneID}", content);
        }

        private async Task DeletePhone()
        {
            if (SelectedPhone == null) return;

            SelectedPhone.phoneName = PhoneName;
            SelectedPhone.phonePrice = PhonePrice;
            SelectedPhone.phoneInStore = PhoneInStore ? 1 : 0;
            SelectedPhone.phoneAvailable = 1;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/wpfPhone/{SelectedPhone.phoneID}";

            string json = JsonSerializer.Serialize(SelectedPhone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("An error occurred while deleting the phone!", "Error", MessageBoxButton.OK);
                return;
            }
            else
            {
                MessageBox.Show("Successfully deleted.", "Update", MessageBoxButton.OK);
                Phones.Remove(SelectedPhone);
                SelectedPhone = null;
                await LoadPhones();
                return;
            }
        }
        private void OpenCreatePhoneWindow()
        {
            var window = new PhoneDetailsView();
            window.DataContext = new PhoneDetailsViewModel();
            window.Show();
        }

        private async Task OpenUpdatePhoneWindow()
        {
            MessageBox.Show("Loading phone details. Please wait...", "Loading", MessageBoxButton.OK, MessageBoxImage.Information);

            if (SelectedPhone == null) return;


            var window = new PhoneDetailsView();
            window.DataContext = new PhoneDetailsViewModel(SelectedPhone.phoneID);
            window.Show();
        }

        private async Task SaveManufacturer()
        {
            ActiveManufacturer.manufacturerName = ManufacturerName;
            ActiveManufacturer.manufacturerEmail = ManufacturerEmail;
            ActiveManufacturer.manufacturerUrl = ManufacturerUrl;

            using HttpClient client = new();
            var json = JsonSerializer.Serialize(ActiveManufacturer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PutAsync($"http://localhost:5175/api/wpfManufacturer/{ActiveManufacturer.manufacturerId}", content);
        }

        private void SignOutLogic()
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

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                        break;
                    }
                }
            }
        }
    }
}