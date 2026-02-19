using PhoneScoutAdmin.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    class SingleManufacturerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private const string ActiveManufacturerName = "Xiaomi";

        // =========================
        // VIEW MODE
        // =========================

        private bool _isPhoneView;
        public bool IsPhoneView
        {
            get => _isPhoneView;
            set
            {
                _isPhoneView = value;
                OnPropertyChanged(nameof(IsPhoneView));
                OnPropertyChanged(nameof(IsManufacturerView));
            }
        }

        public bool IsManufacturerView => !IsPhoneView;

        // =========================
        // DATA
        // =========================

        public ObservableCollection<Phone> Phones { get; } = new();

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
            }
        }

        public Manufacturer ActiveManufacturer { get; } = new();

        // =========================
        // PHONE EDIT FIELDS
        // =========================

        private string _phoneName;
        public string PhoneName
        {
            get => _phoneName;
            set { _phoneName = value; OnPropertyChanged(nameof(PhoneName)); }
        }

        private decimal _phonePrice;
        public decimal PhonePrice
        {
            get => _phonePrice;
            set { _phonePrice = value; OnPropertyChanged(nameof(PhonePrice)); }
        }

        private bool _isInStorage;
        public bool IsInStorage
        {
            get => _isInStorage;
            set { _isInStorage = value; OnPropertyChanged(nameof(IsInStorage)); }
        }

        private bool _isAvailable;
        public bool IsAvailable
        {
            get => _isAvailable;
            set { _isAvailable = value; OnPropertyChanged(nameof(IsAvailable)); }
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

        public RelayCommand SavePhoneCommand { get; }
        public RelayCommand DeletePhoneCommand { get; }
        public ICommand SaveManufacturerCommand { get; }

        // =========================
        // CONSTRUCTORS
        // =========================

        public SingleManufacturerViewModel(bool showPhones)
        {
            IsPhoneView = showPhones;

            SavePhoneCommand = new RelayCommand(
                async () => await SavePhone(),
                () => SelectedPhone != null);

            DeletePhoneCommand = new RelayCommand(
                async () => await DeletePhone(),
                () => SelectedPhone != null);

            SaveManufacturerCommand = new RelayCommand(async () => await SaveManufacturer());

            if (showPhones)
                _ = LoadPhones();
            else
                _ = LoadManufacturer();
        }

        public SingleManufacturerViewModel() : this(true) { }

        // =========================
        // LOGIC
        // =========================

        private async Task LoadPhones()
        {
            using HttpClient client = new();
            var json = await client.GetStringAsync("http://localhost:5175/api/wpfPhone");
            var list = JsonSerializer.Deserialize<List<Phone>>(json);

            Phones.Clear();
            foreach (var phone in list)
            {
                if (phone.manufacturerName == ActiveManufacturerName &&
                    phone.phoneAvailable == 0)
                {
                    Phones.Add(phone);
                }
            }
        }

        private async Task LoadManufacturer()
        {
            using HttpClient client = new();
            var json = await client.GetStringAsync("http://localhost:5175/api/wpfManufacturer");
            var list = JsonSerializer.Deserialize<List<Manufacturer>>(json);

            var manufacturer = list
                .Find(m => m.manufacturerName == ActiveManufacturerName);

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
            PhonePrice = SelectedPhone.phonePrice;
            IsInStorage = SelectedPhone.phoneInStore == 1;
            IsAvailable = SelectedPhone.phoneAvailable == 0;
        }

        private async Task SavePhone()
        {
            SelectedPhone.phoneName = PhoneName;
            SelectedPhone.phonePrice = (int)PhonePrice;
            SelectedPhone.phoneInStore = IsInStorage ? 1 : 0;
            SelectedPhone.phoneAvailable = IsAvailable ? 1 : 0;

            using HttpClient client = new();
            var json = JsonSerializer.Serialize(SelectedPhone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(
                $"http://localhost:5175/api/wpfPhone/{SelectedPhone.phoneID}",
                content);
        }

        private async Task DeletePhone()
        {
            using HttpClient client = new();
            await client.PutAsync(
                $"http://localhost:5175/api/wpfPhone/{SelectedPhone.phoneID}",
                new StringContent(
                    JsonSerializer.Serialize(new { phoneAvailable = 1 }),
                    Encoding.UTF8,
                    "application/json"));

            Phones.Remove(SelectedPhone);
            SelectedPhone = null;
        }

        private async Task SaveManufacturer()
        {
            ActiveManufacturer.manufacturerName = ManufacturerName;
            ActiveManufacturer.manufacturerEmail = ManufacturerEmail;
            ActiveManufacturer.manufacturerUrl = ManufacturerUrl;

            using HttpClient client = new();
            var json = JsonSerializer.Serialize(ActiveManufacturer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(
                $"http://localhost:5175/api/wpfManufacturer/{ActiveManufacturer.manufacturerId}",
                content);
        }
    }
}
