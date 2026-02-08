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
using System.Windows;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class ManufacturerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        //  DATA
        public ObservableCollection<Manufacturer> Manufacturers { get; set; } = new ObservableCollection<Manufacturer>();

        //  SELECTION

        private Manufacturer _selectedManufacturer;

        public Manufacturer SelectedManufacturer
        {
            get => _selectedManufacturer;
            set
            {
                _selectedManufacturer = value;
                OnPropertyChanged(nameof(SelectedManufacturer));
                LoadSelectedManufterIntoFields();
                RaiseCommandStates();
            }
        }

        //  EDIT FIELDS

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

        private string _manufacturerURL;
        public string ManufacturerURL
        {
            get => _manufacturerURL;
            set { _manufacturerURL = value; OnPropertyChanged(nameof(_manufacturerURL)); }
        }



        //  COMMANDS

        public ICommand LoadManufacturersCommand { get; }
        public ICommand SaveManufacturerCommand { get; }
        public ICommand DeleteManufacturerCommand { get; }


        public ManufacturerViewModel()
        {
            LoadManufacturersCommand = new RelayCommand(async () => await LoadManufacturers());
            SaveManufacturerCommand = new RelayCommand(async () => await SaveManufacturer(), () => SelectedManufacturer != null);
            DeleteManufacturerCommand = new RelayCommand(async () => await DeleteManufacturer(), () => SelectedManufacturer != null);
        }

        private void RaiseCommandStates()
        {
            (SaveManufacturerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteManufacturerCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }



        //  LOGIC

        private void LoadSelectedManufterIntoFields()
        {
            if (SelectedManufacturer == null)
            {
                ManufacturerName = "";
                ManufacturerEmail = "";
                ManufacturerURL = "";
                return;
            }

            ManufacturerName = SelectedManufacturer.manufacturerName;
            ManufacturerEmail = SelectedManufacturer.manufacturerEmail;
            ManufacturerURL = SelectedManufacturer.manufacturerUrl;
        }


        private async Task LoadManufacturers()
        {

            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/wpfManufacturer";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();

            var manufacturersList = JsonSerializer.Deserialize<List<Manufacturer>>(json);

            Manufacturers.Clear();
            foreach (var manufacturer in manufacturersList)
            {
                Manufacturers.Add(manufacturer);
            }
        }

        private async Task SaveManufacturer()
        {
            if (SelectedManufacturer == null) return;

            SelectedManufacturer.manufacturerName = ManufacturerName;
            SelectedManufacturer.manufacturerEmail = ManufacturerEmail;
            SelectedManufacturer.manufacturerUrl = ManufacturerURL;

            using HttpClient client = new HttpClient();

            string url = $"http://localhost:5175/api/wpfManufacturer/" + SelectedManufacturer.manufacturerId;

            string json = JsonSerializer.Serialize(SelectedManufacturer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);
        }


        private async Task DeleteManufacturer()
        {
            if (SelectedManufacturer == null) return;
            using HttpClient client = new HttpClient();

            string url = $"http://localhost:5175/api/wpfManufacturer/" + SelectedManufacturer.manufacturerId;

            var response = await client.DeleteAsync(url);
            Manufacturers.Remove(SelectedManufacturer);
            SelectedManufacturer = null;

        }
    }
}
