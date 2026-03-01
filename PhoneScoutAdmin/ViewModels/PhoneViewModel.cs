using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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

        private string _phoneNameFilter;
        public string PhoneNameFilter
        {
            get => _phoneNameFilter;
            set
            {
                _phoneNameFilter = value;
                OnPropertyChanged(nameof(PhoneNameFilter));
                PhonesView.Refresh();
            }
        }

        // ======================
        // COMMANDS
        // ======================

        public ICommand LoadPhonesCommand { get; }
        public ICommand SavePhoneCommand { get; }
        public ICommand CreatePhoneCommand { get; }
        public ICommand UpdatePhoneCommand { get; }
        public ICollectionView PhonesView { get; }

        public ICommand SignOut { get; }
        public ICommand ExitApp { get; }


        public PhoneViewModel()
        {
            LoadPhonesCommand = new RelayCommand(async () => await LoadPhones());
            SavePhoneCommand = new RelayCommand(async () => await SavePhone(), () => SelectedPhone != null);
            UpdatePhoneCommand = new RelayCommand(async () => await OpenUpdatePhoneWindow(), () => SelectedPhone != null);

            CreatePhoneCommand = new RelayCommand(OpenCreatePhoneWindow);

            PhonesView = CollectionViewSource.GetDefaultView(Phones);
            PhonesView.Filter = FilterPhones;

            

        }

        private void RaiseCommandStates()
        {
            (SavePhoneCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (UpdatePhoneCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

            var response = await client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("An error occurred while saving the phone!", "Error", MessageBoxButton.OK);
                return;
            }
            else
            {
                MessageBox.Show("Successfully updated.", "Update", MessageBoxButton.OK);
                PhonesView.Refresh();
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
            if (SelectedPhone == null) return;


            var window = new PhoneDetailsView();
            window.DataContext = new PhoneDetailsViewModel(SelectedPhone.phoneID);
            window.Show();
        }

        private bool FilterPhones(object obj)
        {
            if (obj is not Phone phone)
                return false;

            bool matchesphoneName = string.IsNullOrWhiteSpace(PhoneNameFilter)
                || phone.phoneName.ToString().Contains(PhoneNameFilter);



            return matchesphoneName;
        }

    }
}
