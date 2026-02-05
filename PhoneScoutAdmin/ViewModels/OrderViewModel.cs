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
    public class OrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // DATA
        // ======================

        public ObservableCollection<Order> Orders { get; }
            = new ObservableCollection<Order>();

        public ObservableCollection<ComboItemOrderStorage> Statuses { get; }
            = new ObservableCollection<ComboItemOrderStorage>();

        // ======================
        // SELECTION
        // ======================

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
                LoadSelectedOrderIntoFields();
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

        private int _amount;
        public int Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount)); }
        }

        private ComboItemOrderStorage _selectedStatus;
        public ComboItemOrderStorage SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(nameof(SelectedStatus)); }
        }

        // ======================
        // COMMANDS
        // ======================

        public ICommand LoadOrdersCommand { get; }
        public ICommand SaveOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public OrderViewModel()
        {
            LoadOrdersCommand = new RelayCommand(async () => await LoadOrders());
            SaveOrderCommand = new RelayCommand(async () => await SaveOrder(), () => SelectedOrder != null);
            DeleteOrderCommand = new RelayCommand(async () => await DeleteOrder(), () => SelectedOrder != null);
        }

        private void RaiseCommandStates()
        {
            (SaveOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ======================
        // LOGIC
        // ======================

        private void LoadSelectedOrderIntoFields()
        {
            if (SelectedOrder == null)
            {
                Price = 0;
                Amount = 0;
                SelectedStatus = null;
                return;
            }

            Price = SelectedOrder.price;
            Amount = SelectedOrder.amount;

            SelectedStatus = Statuses
                .FirstOrDefault(s => s.statusCode == SelectedOrder.status);
        }

        private async Task LoadOrders()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/Profile/GetAllOrder";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var orderList = JsonSerializer.Deserialize<List<Order>>(json);

            Orders.Clear();
            foreach (var order in orderList)
                Orders.Add(order);
        }

        private async Task SaveOrder()
        {
            if (SelectedOrder == null) return;

            SelectedOrder.price = Price;
            SelectedOrder.amount = Amount;
            SelectedOrder.status = SelectedStatus.statusCode;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/Profile/updateOrderStatus/{SelectedOrder.orderID}";

            string json = JsonSerializer.Serialize(SelectedOrder);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(url, content);
        }

        private async Task DeleteOrder()
        {
            if (SelectedOrder == null) return;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/Profile/deleteOrder/{SelectedOrder.orderID}";

            await client.DeleteAsync(url);
            Orders.Remove(SelectedOrder);
            SelectedOrder = null;
        }
    }
}
