using PhoneScoutAdmin.Commands;
using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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

        private string _orderIdFilter;
        public string OrderIdFilter
        {
            get => _orderIdFilter;
            set
            {
                _orderIdFilter = value;
                OnPropertyChanged(nameof(OrderIdFilter));
                OrdersView.Refresh();
            }
        }

        private string _emailFilter;
        public string EmailFilter
        {
            get => _emailFilter;
            set
            {
                _emailFilter = value;
                OnPropertyChanged(nameof(EmailFilter));
                OrdersView.Refresh();
            }
        }

        // ======================
        // COMMANDS
        // ======================
        public ICommand LoadOrdersCommand { get; }
        public ICommand SaveOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICollectionView OrdersView { get; }


        public OrderViewModel()
        {
            Statuses.Add(new ComboItemOrderRepair { statusCode = 0, statusName = "Pending" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 1, statusName = "Shipped" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 2, statusName = "Delivered" });

            LoadOrdersCommand = new RelayCommand(async () => await LoadOrders());
            SaveOrderCommand = new RelayCommand(async () => await SaveOrder(), () => SelectedOrder != null);
            DeleteOrderCommand = new RelayCommand(async () => await DeleteOrder(), () => SelectedOrder != null);

            OrdersView = CollectionViewSource.GetDefaultView(Orders);
            OrdersView.Filter = FilterOrders;
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
            string url = $"http://localhost:5175/api/Profile/updateOrder/{SelectedOrder.ID}";
            MessageBox.Show(url);

            // Send full updated order (change to DTO if backend expects otherwise)
            string json = JsonSerializer.Serialize(SelectedOrder);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);     
            
            

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("An error occurred while saving the order!"+response.Content, "Error", MessageBoxButton.OK);
                return;
            }
            else
            {
                MessageBox.Show("Successfully updated.", "Update", MessageBoxButton.OK);

                string emailTargy = "Rendelésének állapota megváltozott!";
                string emailTorzs = $@"
                    <div style=""background: linear-gradient(135deg, #2300B3 0%, #68F145 100%); margin: 0; padding: 0; min-height: 100vh;"">
                        <!-- Külső táblázat a teljes magasság és a vertikális középre igazítás miatt -->
                        <table width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""min-height: 100vh; width: 100%;"">
                            <tr>
                                <td align=""center"" valign=""middle"" style=""padding: 20px;"">
                                    
                                    <!-- Fehér kártya (formContainer stílus) -->
                                    <div style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 12px; box-shadow: 0 0 20px rgba(0, 0, 0, 0.25); overflow: hidden; display: inline-block; text-align: left;"">
                                        <div style=""padding: 40px 30px; text-align: center;"">
                                            
                                            <h2 style=""color: #333333; font-size: 28px; margin-bottom: 30px; margin-top: 0; font-family: Arial, sans-serif;"">Rendelésének státusza megváltozott.</h2>
                                            
                                            <h3 style=""color: #333333; font-size: 18px; margin-bottom: 20px; font-family: Arial, sans-serif;"">Kedves {SelectedOrder.userName}!</h3>
                                            
                                            <p style=""color: #555555; font-size: 16px; line-height: 1.5; margin-bottom: 35px; font-family: Arial, sans-serif;"">
Tájékoztatjuk, hogy a <i>{SelectedOrder.orderID}</i> azonosítójú rendelésének státusza a következőre változott: <strong>{Statuses[SelectedOrder.status].statusName}</strong>.<br><br>
                                                Rendelését továbbra is nyomon követheti a fiókjában.: <br><br>
                                            </p>

                                           

                                            <!-- Lábjegyzet -->
                                            <div style=""margin-top: 45px; border-top: 1px solid #eeeeee; padding-top: 20px; text-align: left;"">
                                                <p style=""color: #333333; font-size: 14px; margin: 0; font-family: Arial, sans-serif;"">Üdvözlettel,<br><strong>PhoneScout Team</strong></p>
                                                <p style=""font-size: 12px; color: #777777; margin-top: 15px; line-height: 1.4; font-family: Arial, sans-serif;"">Ez egy automatikusan generált üzenet, kérjük ne válaszoljon rá.</p>
                                            </div>
                                        </div>
                                    </div>

                                </td>
                            </tr>
                        </table>
                    </div>";



                // Email küldése (Program.cs-ben lévő metódussal)
                await EmailSending.SendEmail(SelectedOrder.userEmail, emailTargy, emailTorzs);




                OrdersView.Refresh();
            }
        }

        private async Task DeleteOrder()
        {
            if (SelectedOrder == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the selected order: {SelectedOrder.orderID}?",
                "Delete order",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using HttpClient client = new();
                string url = $"http://localhost:5175/api/Profile/deleteOrder/{SelectedOrder.ID}";

                var response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("An error occurred while deleting the order!", "Error", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    MessageBox.Show("Successfully deleted.", "Update", MessageBoxButton.OK);
                    Orders.Remove(SelectedOrder);
                    SelectedOrder = null;
                    OrdersView.Refresh();
                }
            }

            

        }

        private bool FilterOrders(object obj)
        {
            if (obj is not Order order)
                return false;

            bool matchesOrderId = string.IsNullOrWhiteSpace(OrderIdFilter)
                || order.orderID.ToString().Contains(OrderIdFilter);

            bool matchesEmail = string.IsNullOrWhiteSpace(EmailFilter)
                || (!string.IsNullOrEmpty(order.userEmail) &&
                    order.userEmail.ToLower().Contains(EmailFilter.ToLower()));

            return matchesOrderId && matchesEmail;
        }
    }
}
