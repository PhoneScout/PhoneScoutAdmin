using PhoneScoutAdmin.Models;
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
    public class RepairViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // DATA
        // ======================

        public ObservableCollection<Repair> Repairs { get; } = new();
        public ObservableCollection<ComboItemOrderRepair> Statuses { get; } = new();
        public ObservableCollection<string> Parts { get; } = new();

        // ======================
        // SELECTION
        // ======================

        private Repair _selectedRepair;
        public Repair SelectedRepair
        {
            get => _selectedRepair;
            set
            {
                _selectedRepair = value;
                OnPropertyChanged(nameof(SelectedRepair));
                LoadSelectedRepairIntoFields();
                RaiseCommandStates();
            }
        }

        // ======================
        // EDIT FIELDS
        // ======================

        private int _price;
        public int Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        private string _newPart;
        public string NewPart
        {
            get => _newPart;
            set { _newPart = value; OnPropertyChanged(nameof(NewPart)); }
        }

        // ======================
        // COMMANDS
        // ======================

        public ICommand LoadRepairsCommand { get; }
        public ICommand SaveRepairCommand { get; }
        public ICommand DeleteRepairCommand { get; }
        public ICommand AddPartCommand { get; }
        public ICommand RemovePartCommand { get; }

        public RepairViewModel()
        {
            Statuses.Add(new ComboItemOrderRepair { statusCode = 0, statusName = "Pending" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 1, statusName = "Shipped" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 2, statusName = "Delivered" });

            LoadRepairsCommand = new RelayCommand(async () => await LoadRepairs());
            SaveRepairCommand = new RelayCommand(async () => await SaveRepair(), () => SelectedRepair != null);
            DeleteRepairCommand = new RelayCommand(async () => await DeleteRepair(), () => SelectedRepair != null);

            AddPartCommand = new RelayCommand(AddPart);
            RemovePartCommand = new RelayCommand<string>(RemovePart);

        }

        private void RaiseCommandStates()
        {
            (SaveRepairCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteRepairCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ======================
        // LOAD SELECTED REPAIR
        // ======================

        private void LoadSelectedRepairIntoFields()
        {
            Parts.Clear();

            if (SelectedRepair == null)
            {
                Price = 0;
                return;
            }

            Price = SelectedRepair.price;

            if (SelectedRepair.parts != null)
            {
                foreach (var part in SelectedRepair.parts)
                    Parts.Add(part);
            }
        }

        // ======================
        // API
        // ======================

        private async Task LoadRepairs()
        {
            using HttpClient client = new();
            string url = "http://localhost:5175/api/Profile/GetAllRepair";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<Repair>>(json);

            Repairs.Clear();
            foreach (var repair in list)
                Repairs.Add(repair);


        }

        private async Task SaveRepair()
        {
            if (SelectedRepair == null) return;

            SelectedRepair.price = Price;
            SelectedRepair.parts = Parts.ToList();
            // status is already updated via binding (same as Order)

            using HttpClient client = new();
            string url = $"http://localhost:5175/api/Profile/updateRepair/{SelectedRepair.repairID}";

            MessageBox.Show(url);


            var json = JsonSerializer.Serialize(SelectedRepair);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
                MessageBox.Show(response.Content.ToString());
        }

        private async Task DeleteRepair()
        {
            if (SelectedRepair == null) return;

            using HttpClient client = new();
            string url = $"http://localhost:5175/api/Profile/deleteRepair/{SelectedRepair.repairID}";

            await client.DeleteAsync(url);
            Repairs.Remove(SelectedRepair);
            SelectedRepair = null;
        }

        // ======================
        // PARTS
        // ======================

        private void AddPart()
        {
            if (!string.IsNullOrWhiteSpace(NewPart))
            {
                Parts.Add(NewPart);
                NewPart = string.Empty;
            }
        }

        private void RemovePart(string part)
        {
            if (!string.IsNullOrEmpty(part))
                Parts.Remove(part);
        }
    }
}
