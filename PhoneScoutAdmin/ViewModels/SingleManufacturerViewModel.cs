using PhoneScoutAdmin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        // VISIBILITY
        // =========================

        private bool _isPhoneView = true;
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
            }
        }

        public Manufacturer ActiveManufacturer { get; } = new();

        // =========================
        // PHONE EDIT FIELDS
        // =========================

        public string PhoneName { get; set; }
        public string PhonePrice { get; set; }
        public bool PhoneInStore { get; set; }
        public bool PhoneAvailable { get; set; }

        // =========================
        // MANUFACTURER EDIT FIELDS
        // =========================

        public string ManufacturerName { get; set; }
        public string ManufacturerEmail { get; set; }
        public string ManufacturerURL { get; set; }

        // =========================
        // COMMANDS
        // =========================

        public ICommand LoadPhonesCommand { get; }
        public ICommand LoadManufacturerCommand { get; }
        public ICommand SavePhoneCommand { get; }
        public ICommand DeletePhoneCommand { get; }
        public ICommand SaveManufacturerCommand { get; }

        public SingleManufacturerViewModel()
        {
            LoadPhonesCommand = new RelayCommand(async () => await LoadPhones());
            LoadManufacturerCommand = new RelayCommand(async () => await LoadManufacturer());

            SavePhoneCommand = new RelayCommand(async () => await SavePhone(), () => SelectedPhone != null);
            DeletePhoneCommand = new RelayCommand(async () => await DeletePhone(), () => SelectedPhone != null);
            SaveManufacturerCommand = new RelayCommand(async () => await SaveManufacturer());
        }

        // =========================
        // LOGIC
        // =========================

        private async Task LoadPhones()
        {
            IsPhoneView = true;

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
            IsPhoneView = false;

            using HttpClient client = new();
            var json = await client.GetStringAsync("http://localhost:5175/api/wpfManufacturer");
            var list = JsonSerializer.Deserialize<List<Manufacturer>>(json);

            var manufacturer = list
                .Find(m => m.manufacturerName == ActiveManufacturerName);

            if (manufacturer == null) return;

            ActiveManufacturer.manufacturerId = manufacturer.manufacturerId;
            ManufacturerName = manufacturer.manufacturerName;
            ManufacturerEmail = manufacturer.manufacturerEmail;
            ManufacturerURL = manufacturer.manufacturerUrl;

            OnPropertyChanged(nameof(ManufacturerName));
            OnPropertyChanged(nameof(ManufacturerEmail));
            OnPropertyChanged(nameof(ManufacturerURL));
        }

        private void LoadPhoneToEditor()
        {
            if (SelectedPhone == null) return;

            PhoneName = SelectedPhone.phoneName;
            PhonePrice = SelectedPhone.phonePrice.ToString();
            PhoneInStore = SelectedPhone.phoneInStore == "van";
            PhoneAvailable = SelectedPhone.phoneAvailable == 0;

            OnPropertyChanged(nameof(PhoneName));
            OnPropertyChanged(nameof(PhonePrice));
            OnPropertyChanged(nameof(PhoneInStore));
            OnPropertyChanged(nameof(PhoneAvailable));
        }

        private async Task SavePhone()
        {
            SelectedPhone.phoneName = PhoneName;
            SelectedPhone.phonePrice = int.Parse(PhonePrice);
            SelectedPhone.phoneInStore = PhoneInStore ? "van" : "nincs";
            SelectedPhone.phoneAvailable = PhoneAvailable ? 1 : 0;

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
                new StringContent(JsonSerializer.Serialize(
                    new { phoneAvailable = 1 }),
                    Encoding.UTF8, "application/json"));

            Phones.Remove(SelectedPhone);
            SelectedPhone = null;
        }

        private async Task SaveManufacturer()
        {
            ActiveManufacturer.manufacturerName = ManufacturerName;
            ActiveManufacturer.manufacturerEmail = ManufacturerEmail;
            ActiveManufacturer.manufacturerUrl = ManufacturerURL;

            using HttpClient client = new();
            var json = JsonSerializer.Serialize(ActiveManufacturer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(
                $"http://localhost:5175/api/wpfManufacturer/{ActiveManufacturer.manufacturerId}",
                content);
        }
    }
}