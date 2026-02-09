using PhoneScoutAdmin.Models;
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
    public class OrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // DATA
        // ======================
        public ObservableCollection<Order> Orders { get; } = new();
        public ObservableCollection<ComboItemOrderRepair> Statuses { get; } = new();

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
            Statuses.Add(new ComboItemOrderRepair { statusCode = 0, statusName = "Pending" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 1, statusName = "Shipped" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 2, statusName = "Delivered" });

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
            using HttpClient client = new();
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

            using HttpClient client = new();
            string url = $"http://localhost:5175/api/Profile/updateOrder/{SelectedOrder.orderID}";

            // Send full updated order (change to DTO if backend expects otherwise)
            string json = JsonSerializer.Serialize(SelectedOrder);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
                MessageBox.Show("Failed to save order.");
        }

        private async Task DeleteOrder()
        {
            if (SelectedOrder == null) return;

            using HttpClient client = new();
            string url = $"http://localhost:5175/api/Profile/deleteOrder/{SelectedOrder.orderID}";

            await client.DeleteAsync(url);
            Orders.Remove(SelectedOrder);
            SelectedOrder = null;
        }
    }
}
