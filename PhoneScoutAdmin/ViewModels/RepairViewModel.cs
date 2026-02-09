using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

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

        public ObservableCollection<Repair> Repairs { get; }
            = new ObservableCollection<Repair>();

        public ObservableCollection<ComboItemOrderStorage> Statuses { get; }
            = new ObservableCollection<ComboItemOrderStorage>();

        public ObservableCollection<string> Parts { get; }
            = new ObservableCollection<string>();

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
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        private ComboItemOrderStorage _selectedStatus;
        public ComboItemOrderStorage SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
            }
        }

        private string _newPart;
        public string NewPart
        {
            get => _newPart;
            set
            {
                _newPart = value;
                OnPropertyChanged(nameof(NewPart));
            }
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
            LoadRepairsCommand = new RelayCommand(async () => await LoadRepairs());
            SaveRepairCommand = new RelayCommand(async () => await SaveRepair(), () => SelectedRepair != null);
            DeleteRepairCommand = new RelayCommand(async () => await DeleteRepair(), () => SelectedRepair != null);
            AddPartCommand = new RelayCommand(AddPart);
            //RemovePartCommand = new RelayCommand<string>(RemovePart);

            // Load statuses immediately
            _ = LoadStatuses();
        }

        private void RaiseCommandStates()
        {
            (SaveRepairCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteRepairCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ======================
        // STATUS SYNC
        // ======================

        private void SyncSelectedStatus()
        {
            if (SelectedRepair == null || Statuses.Count == 0)
                return;

            SelectedStatus = Statuses.FirstOrDefault(s => s.statusCode == SelectedRepair.status);
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
                SelectedStatus = null;
                return;
            }

            Price = SelectedRepair.price;

            if (SelectedRepair.parts != null)
            {
                foreach (var part in SelectedRepair.parts)
                    Parts.Add(part);
            }

            // Sync status after repair is loaded
            SyncSelectedStatus();
        }

        // ======================
        // API CALLS
        // ======================

        private async Task LoadRepairs()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/Profile/GetAllRepair";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var repairList = JsonSerializer.Deserialize<List<Repair>>(json);

            Repairs.Clear();
            foreach (var repair in repairList)
                Repairs.Add(repair);
        }

        private async Task LoadStatuses()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/Profile/GetAllStatuses";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var statuses = JsonSerializer.Deserialize<List<ComboItemOrderStorage>>(json);

            Statuses.Clear();
            foreach (var status in statuses)
                Statuses.Add(status);

            // Refresh the selected status if a repair is already selected
            if (SelectedRepair != null)
                SyncSelectedStatus();
        }

        private async Task SaveRepair()
        {
            if (SelectedRepair == null) return;

            SelectedRepair.price = Price;
            SelectedRepair.status = SelectedStatus?.statusCode ?? SelectedRepair.status;
            SelectedRepair.parts = Parts.ToList();

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/Profile/updateRepair/{SelectedRepair.repairID}";

            string json = JsonSerializer.Serialize(SelectedRepair);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(url, content);
        }

        private async Task DeleteRepair()
        {
            if (SelectedRepair == null) return;

            using HttpClient client = new HttpClient();
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
