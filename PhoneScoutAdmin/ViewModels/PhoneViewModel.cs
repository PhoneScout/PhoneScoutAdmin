using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class PhoneViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // DATA
        // ======================

        public ObservableCollection<Phone> Phones { get; }
            = new ObservableCollection<Phone>();

        // ======================
        // SELECTION
        // ======================

        private Phone _selectedPhone;
        public Phone SelectedPhone
        {
            get => _selectedPhone;
            set
            {
                _selectedPhone = value;
                OnPropertyChanged(nameof(SelectedPhone));
                LoadSelectedPhoneIntoFields();
                RaiseCommandStates();
            }
        }

        // ======================
        // EDIT FIELDS
        // ======================

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

        // ======================
        // COMMANDS
        // ======================

        public ICommand LoadPhonesCommand { get; }
        public ICommand SavePhoneCommand { get; }
        public ICommand DeletePhoneCommand { get; }
        public ICommand CreatePhoneCommand { get; }
        public ICommand UpdatePhoneCommand { get; }


        public PhoneViewModel()
        {
            LoadPhonesCommand = new RelayCommand(async () => await LoadPhones());
            SavePhoneCommand = new RelayCommand(async () => await SavePhone(), () => SelectedPhone != null);
            DeletePhoneCommand = new RelayCommand(async () => await DeletePhone(), () => SelectedPhone != null);

            CreatePhoneCommand = new RelayCommand(OpenCreatePhoneWindow);
            UpdatePhoneCommand = new RelayCommand(OpenUpdatePhoneWindow);
        }

        private void RaiseCommandStates()
        {
            (SavePhoneCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeletePhoneCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ======================
        // LOGIC
        // ======================

        private void LoadSelectedPhoneIntoFields()
        {
            if (SelectedPhone == null)
            {
                PhoneName = "";
                PhonePrice = 0;
                PhoneInStore = false;
                PhoneAvailable = false;
                return;
            }

            PhoneName = SelectedPhone.phoneName;
            PhonePrice = SelectedPhone.phonePrice;
            PhoneInStore = SelectedPhone.phoneInStore == 1;
            PhoneAvailable = SelectedPhone.phoneAvailable == 1;
        }

        private async Task LoadPhones()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/wpfPhone";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var phoneList = JsonSerializer.Deserialize<List<Phone>>(json);

            Phones.Clear();
            foreach (var phone in phoneList)
                Phones.Add(phone);
        }

        private async Task SavePhone()
        {
            if (SelectedPhone == null) return;

            SelectedPhone.phoneName = PhoneName;
            SelectedPhone.phonePrice = PhonePrice;
            SelectedPhone.phoneInStore = PhoneInStore ? 1 : 0;
            SelectedPhone.phoneAvailable = PhoneAvailable ? 1 : 0;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/wpfPhone/{SelectedPhone.phoneID}";

            string json = JsonSerializer.Serialize(SelectedPhone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(url, content);
        }

        private async Task DeletePhone()
        {
            if (SelectedPhone == null) return;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/wpfPhone/{SelectedPhone.phoneID}";

            await client.DeleteAsync(url);
            Phones.Remove(SelectedPhone);
            SelectedPhone = null;
        }

        private void OpenCreatePhoneWindow()
        {
            var window = new PhoneDetailsView();
            window.DataContext = new PhoneDetailsViewModel();
            window.Show();          
        }

        private void OpenUpdatePhoneWindow()
        {
            var window = new PhoneDetailsView();
            window.DataContext = new PhoneDetailsViewModel(SelectedPhone.phoneID);
            window.Show();
        }

    }
}
