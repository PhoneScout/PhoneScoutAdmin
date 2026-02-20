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
    public class StorageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public ObservableCollection<Part> parts { get; set; } = new ObservableCollection<Part>();




        private Part _selectedPart;
        public Part SelectedPart
        {
            get => _selectedPart;
            set
            {
                _selectedPart = value;
                OnPropertyChanged(nameof(SelectedPart));
                LoadSelectedPartIntoFields();
                RaiseCommandStates();
            }
        }



        private string _partName;
        public string PartName
        {
            get => _partName;
            set { _partName = value; OnPropertyChanged(nameof(PartName)); }
        }

        private int _partAmount;
        public int PartAmount
        {
            get => _partAmount;
            set { _partAmount = value; OnPropertyChanged(nameof(PartAmount)); }
        }

        private string _newPartName;
        public string NewPartName
        {
            get => _newPartName;
            set { _newPartName = value; OnPropertyChanged(nameof(NewPartName)); }
        }

        private int _newPartAmount;
        public int NewPartAmount
        {
            get => _newPartAmount;
            set { _newPartAmount = value; OnPropertyChanged(nameof(NewPartAmount)); }
        }


        public ICommand LoadPartsCommand { get; }
        public ICommand SavePartCommand { get; }
        public ICommand CreatePartCommand { get; }
        public ICommand DeletePartCommand { get; }

        public StorageViewModel()
        {
            LoadPartsCommand = new RelayCommand(async () => await LoadParts());
            SavePartCommand = new RelayCommand(async () => await SavePart(), () => SelectedPart != null);
            CreatePartCommand = new RelayCommand(async () => await CreatePart());
            DeletePartCommand = new RelayCommand(async () => await DeletePart(), () => SelectedPart != null);
        }

        private void RaiseCommandStates()
        {
            (SavePartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CreatePartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeletePartCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }



        private void LoadSelectedPartIntoFields()
        {
            if (SelectedPart == null)
            {
                PartName = "";
                PartAmount = 0;
                return;
            }

            PartName = SelectedPart.partName;
            PartAmount = SelectedPart.partAmount;
        }

        private async Task LoadParts()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/wpfStorage";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var partList = JsonSerializer.Deserialize<List<Part>>(json);

            parts.Clear();
            foreach (var phone in partList)
                parts.Add(phone);
        }

        private async Task SavePart()
        {
            if (SelectedPart == null) return;

            SelectedPart.partName = PartName;
            SelectedPart.partAmount = PartAmount;


            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/wpfStorage/{SelectedPart.partID}";

            string json = JsonSerializer.Serialize(SelectedPart);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(url, content);
        }

        private async Task CreatePart()
        {
            // 1. Create a new object using the "New" properties from your XAML bindings
            var newPart = new Part // Assuming your class name is Part
            {
                partName = NewPartName,
                partAmount = NewPartAmount
            };

            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/wpfStorage";

            // 2. Serialize the new object
            string json = JsonSerializer.Serialize(newPart);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 3. POST to the collection URL
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // Optional: Clear fields after successful upload
                NewPartName = string.Empty;
                NewPartAmount = 0;
            }
        }



        private async Task DeletePart()
        {
            if (SelectedPart == null) return;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/wpfStorage/{SelectedPart.partID}";

            await client.DeleteAsync(url);
            parts.Remove(SelectedPart);
            SelectedPart = null;
        }
    }
}