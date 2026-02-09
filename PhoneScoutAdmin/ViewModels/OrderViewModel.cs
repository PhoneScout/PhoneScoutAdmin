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
    public class OrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // DATA
        // ======================
        public ObservableCollection<Order> Orders { get; } = new ObservableCollection<Order>();
        public ObservableCollection<ComboItemOrderStorage> Statuses { get; } = new ObservableCollection<ComboItemOrderStorage>();

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
                RaiseCommandStates();
            }
        }

        // ======================
        // COMMANDS
        // ======================
        public ICommand LoadOrdersCommand { get; }
        public ICommand SaveOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public OrderViewModel()
        {
            // Populate status options
            Statuses.Add(new ComboItemOrderStorage { statusCode = 0, statusName = "Pending" });
            Statuses.Add(new ComboItemOrderStorage { statusCode = 1, statusName = "Shipped" });
            Statuses.Add(new ComboItemOrderStorage { statusCode = 2, statusName = "Delivered" });

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

    // ======================
    // SUPPORT CLASSES
    // ======================
    public class Order
    {
        public int orderID { get; set; }
        public string userEmail { get; set; }
        public string address { get; set; }
        public long phoneNumber { get; set; }
        public string phoneName { get; set; }
        public string phoneColorName { get; set; }
        public string phoneColorHex { get; set; }
        public int phoneRam { get; set; }
        public int price { get; set; }
        public int amount { get; set; }
        public int status { get; set; } // This is now directly bound to ComboBox.SelectedValue
    }

    public class ComboItemOrderStorage
    {
        public int statusCode { get; set; }
        public string statusName { get; set; }
    }
}
