using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhoneScoutAdmin
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>

    public class Phone
    {
        [JsonPropertyName("phoneID")]
        public int phoneID { get; set; }

        [JsonPropertyName("phoneName")]
        public string phoneName { get; set; }

        [JsonPropertyName("phonePrice")]
        public int phonePrice { get; set; }

        [JsonPropertyName("phoneInstore")]
        public string phoneInStore { get; set; }

        [JsonPropertyName("phoneAvailable")]
        public int phoneAvailable { get; set; }
    }
    public class Manufacturer
    {

        [JsonPropertyName("manufacturerID")]
        public int manufacturerId { get; set; }

        [JsonPropertyName("manufacturerName")]
        public string manufacturerName { get; set; }

        [JsonPropertyName("manufacturerURL")]
        public string manufacturerUrl { get; set; }

        [JsonPropertyName("manufacturerEmail")]
        public string manufacturerEmail { get; set; }
    }

    public class User
    {

        [JsonPropertyName("userFullName")]
        public string fullName { get; set; }

        [JsonPropertyName("userEmail")]
        public string email { get; set; }

        [JsonPropertyName("userPrivilegeLevel")]
        public int privilegeLevel { get; set; }

        [JsonPropertyName("userPrivilegeName")]
        public string privilegeName { get; set; }

        [JsonPropertyName("userActive")]
        public int isActive { get; set; }
    }
    public class Storage
    {

        [JsonPropertyName("partID")]
        public int partID { get; set; }

        [JsonPropertyName("partName")]
        public string partName { get; set; }

        [JsonPropertyName("partAmount")]
        public int partAmount { get; set; }
    }
    public class Repair
    {

        [JsonPropertyName("repairID")]
        public string repairID { get; set; }

        [JsonPropertyName("name")]
        public string userEmail { get; set; }

        [JsonPropertyName("postalCode")]
        public int postalCode { get; set; }

        [JsonPropertyName("city")]
        public string city { get; set; }

        [JsonPropertyName("address")]
        public string address { get; set; }

        [JsonPropertyName("phoneNumber")]
        public long phoneNumber { get; set; }

        [JsonPropertyName("phoneName")]
        public string phoneName { get; set; }

        [JsonPropertyName("price")]
        public int price { get; set; }

        [JsonPropertyName("status")]
        public int status { get; set; }

        [JsonPropertyName("manufacturerName")]
        public string manufacturerName { get; set; }

        [JsonPropertyName("phoneInspection")]
        public int phoneInspection { get; set; }

        [JsonPropertyName("problemDescription")]
        public string problemDescription { get; set; }

        [JsonPropertyName("parts")]
        public List<string> parts { get; set; }
    }

    public class Order
    {

        [JsonPropertyName("userEmail")]
        public string userEmail { get; set; }

        [JsonPropertyName("postalCode")]
        public int postalCode { get; set; }

        [JsonPropertyName("city")]
        public string city { get; set; }

        [JsonPropertyName("address")]
        public string address { get; set; }

        [JsonPropertyName("phoneNumber")]
        public long phoneNumber { get; set; }

        [JsonPropertyName("phoneName")]
        public string phoneName { get; set; }

        [JsonPropertyName("price")]
        public int price { get; set; }

        [JsonPropertyName("status")]
        public int status { get; set; }

        [JsonPropertyName("amount")]
        public int amount { get; set; }

        [JsonPropertyName("phoneColorName")]
        public string phoneColorName { get; set; }

        [JsonPropertyName("phoneColorHex")]
        public string phoneColorHex { get; set; }

        [JsonPropertyName("phoneRam")]
        public int phoneRam { get; set; }

        [JsonPropertyName("phoneStorage")]
        public int phoneStorage { get; set; }

    }
    public class ComboItemUsers
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }

        public override string? ToString()
        {
            return $"{Level} - {Name}";
        }
    }
    public class ComboItemOrderStorage
    {
        public int Id { get; set; }
        public int statusCode { get; set; }
        public string Name { get; set; }

        public override string? ToString()
        {
            return $"{Name}";
        }
    }

    public partial class Home : Window
    {
        string selectedMenu = "phones";
        public ObservableCollection<Phone> phones { get; set; } = new ObservableCollection<Phone>();
        public ObservableCollection<Manufacturer> manufacturers { get; set; } = new ObservableCollection<Manufacturer>();
        public ObservableCollection<User> users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Storage> parts { get; set; } = new ObservableCollection<Storage>();
        public ObservableCollection<Repair> repairs { get; set; } = new ObservableCollection<Repair>();
        public ObservableCollection<Order> orders { get; set; } = new ObservableCollection<Order>();

        public ObservableCollection<ComboItemUsers> Items { get; } =
    new ObservableCollection<ComboItemUsers>
    {
        new ComboItemUsers { Id = 1, Level = 1, Name = "User" },
        new ComboItemUsers { Id = 2, Level = 2, Name = "Manufacturer" },
        new ComboItemUsers { Id = 3, Level = 3, Name = "Admin" },
    };
        public ComboItemUsers SelectedItem { get; set; }


        public ObservableCollection<ComboItemOrderStorage> statuses { get; } =
    new ObservableCollection<ComboItemOrderStorage>
    {
        new ComboItemOrderStorage { Id = 1, statusCode  = 0, Name = "Under processing" },
        new ComboItemOrderStorage { Id = 2, statusCode  = 1, Name = "Processed" },
        new ComboItemOrderStorage { Id = 3, statusCode = 2, Name = "Ready for pickup" },
        new ComboItemOrderStorage { Id = 4, statusCode = 3, Name = "Completed" },
    };
        public ComboItemOrderStorage selectedStatus { get; set; }



        public Home()
        {
            InitializeComponent();
            DataContext = this;
        }




        //Phones Requests
        //Manufacturers Requests
        //Users Requests
        //Storage Requests
        private async void loadStorage(object sender, RoutedEventArgs e)
        {
            selectedMenu = "storage";

            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/wpfStorage";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var partList = JsonSerializer.Deserialize<List<Storage>>(json);

                    if (partList != null)
                    {
                        parts.Clear();
                        foreach (var part in partList)
                        {
                            parts.Add(part);
                        }
                    }

                    storageDataGrid.ItemsSource = parts;
                    populateInformationPart();
                }
                else
                {
                    MessageBox.Show("Failed to load storage from API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void updateStorage(object sender, RoutedEventArgs e)
        {           
            var selectedPart = ((Storage)storageDataGrid.SelectedItem);
            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/wpfStorage/"+selectedPart.partID;
                selectedPart.partName = partName.Text;
                selectedPart.partAmount = int.Parse(partAmount.Text);

                

                // Convert object to JSON
                string json = JsonSerializer.Serialize(selectedPart);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url,content);



                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Sikeres frissítés");

                    //storageDataGrid.ItemsSource = parts;
                    //populateInformationPart();
                }
                else
                {
                    MessageBox.Show("Failed to load storage from API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }





        //Repairs Requests
        //Orders Requests




        private async void loadPhones(object sender, RoutedEventArgs e)
        {
            phoneDataGrid.SelectedItem = null;
            manufacturerDataGrid.SelectedItem = null;
            userDataGrid.SelectedItem = null;

            selectedMenu = "phone";
            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/wpfPhone"; // your API endpoint
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    // Deserialize into a list of phones
                    var phoneList = JsonSerializer.Deserialize<List<Phone>>(json);

                    if (phoneList != null)
                    {
                        phones.Clear();
                        foreach (var phone in phoneList)
                        {
                            phones.Add(phone);
                        }
                    }

                    phoneDataGrid.ItemsSource = phones;
                    populateInformationPart();
                }
                else
                {
                    MessageBox.Show("Failed to load phones from API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private async void loadManufacturers(object sender, RoutedEventArgs e)
        {
            phoneDataGrid.SelectedItem = null;
            manufacturerDataGrid.SelectedItem = null;
            userDataGrid.SelectedItem = null;

            selectedMenu = "manufacturer";

            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/wpfManufacturer";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var manufacturersList = JsonSerializer.Deserialize<List<Manufacturer>>(json);

                    if (manufacturersList != null)
                    {
                        manufacturers.Clear();
                        foreach (var manufacturer in manufacturersList)
                        {
                            manufacturers.Add(manufacturer);
                        }
                    }

                    manufacturerDataGrid.ItemsSource = manufacturersList;
                    populateInformationPart();

                }
                else
                {
                    MessageBox.Show("Failed to load manufacturers from API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private async void loadUsers(object sender, RoutedEventArgs e)
        {
            phoneDataGrid.SelectedItem = null;
            manufacturerDataGrid.SelectedItem = null;
            userDataGrid.SelectedItem = null;
            selectedMenu = "user";

            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/wpfUsers";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var userList = JsonSerializer.Deserialize<List<User>>(json);
                    MessageBox.Show(userList.Count().ToString());


                    if (userList != null)
                    {
                        users.Clear();
                        foreach (var user in userList)
                        {
                            users.Add(user);
                        }
                    }

                    userDataGrid.ItemsSource = userList;
                    populateInformationPart();

                }
                else
                {
                    MessageBox.Show("Failed to load users from API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void loadRepairs(object sender, RoutedEventArgs e)
        {
            phoneDataGrid.SelectedItem = null;
            manufacturerDataGrid.SelectedItem = null;
            userDataGrid.SelectedItem = null;
            selectedMenu = "repair";

            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/Profile/GetAllRepair";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var repairList = JsonSerializer.Deserialize<List<Repair>>(json);
                    


                    if (repairList != null)
                    {
                        repairs.Clear();
                        foreach (var repair in repairList)
                        {
                            repairs.Add(repair);
                        }
                    }

                    repairDataGrid.ItemsSource = repairs;
                    populateInformationPart();

                }
                else
                {
                    MessageBox.Show("Failed to orders users from API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void loadOrders(object sender, RoutedEventArgs e)
        {
            phoneDataGrid.SelectedItem = null;
            manufacturerDataGrid.SelectedItem = null;
            userDataGrid.SelectedItem = null;
            selectedMenu = "order";

            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/Profile/GetAllOrder";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var orderList = JsonSerializer.Deserialize<List<Order>>(json);



                    if (orderList != null)
                    {
                        orders.Clear();
                        foreach (var order in orderList)
                        {
                            orders.Add(order);
                        }
                    }

                    orderDataGrid.ItemsSource = orders;
                    populateInformationPart();

                }
                else
                {
                    MessageBox.Show("Failed to load orders from API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        




        public void populateInformationPart()
        {
            if (selectedMenu == "manufacturer")
            {
                manufacturerDetails.Visibility = Visibility.Visible;
                phoneDetails.Visibility = Visibility.Collapsed;
                userDetails.Visibility = Visibility.Collapsed;
                storageDetails.Visibility = Visibility.Collapsed;
            }
            else if (selectedMenu == "phone")
            {
                phoneDetails.Visibility = Visibility.Visible;
                userDetails.Visibility = Visibility.Collapsed;
                manufacturerDetails.Visibility = Visibility.Collapsed;
                storageDetails.Visibility = Visibility.Collapsed;
            }
            else if (selectedMenu == "user")
            {
                userDetails.Visibility = Visibility.Visible;
                manufacturerDetails.Visibility = Visibility.Collapsed;
                phoneDetails.Visibility = Visibility.Collapsed;
                storageDetails.Visibility = Visibility.Collapsed;
            }
            else if (selectedMenu == "storage")
            {
                userDetails.Visibility = Visibility.Collapsed;
                manufacturerDetails.Visibility = Visibility.Collapsed;
                phoneDetails.Visibility = Visibility.Collapsed;
                storageDetails.Visibility = Visibility.Visible;
            }
            else if (selectedMenu == "")
            {
                userDetails.Visibility = Visibility.Collapsed;
                manufacturerDetails.Visibility = Visibility.Collapsed;
                phoneDetails.Visibility = Visibility.Collapsed;
                storageDetails.Visibility = Visibility.Visible;
            }
        }

        private void showInfos(object sender, SelectionChangedEventArgs e)
        {
            if (selectedMenu == "phone")
            {
                if (phoneDataGrid.SelectedItem is Phone selectedPhone)
                {
                    phoneName.Text = selectedPhone.phoneName;
                    phonePrice.Text = selectedPhone.phonePrice.ToString();
                    phoneInStore.IsChecked = selectedPhone.phoneInStore == "van";
                    phoneAvailable.IsChecked = selectedPhone.phoneAvailable == 0;
                }
                else
                {
                    phoneName.Text = "";
                    phonePrice.Text = "";
                    phoneInStore.IsChecked = false;
                    phoneAvailable.IsChecked = false;
                }
            }

            if (selectedMenu == "manufacturer")
            {
                if (manufacturerDataGrid.SelectedItem is Manufacturer selectedManufacturer)
                {
                    manufacturerName.Text = selectedManufacturer.manufacturerName;
                    manufacturerURL.Text = selectedManufacturer.manufacturerUrl;
                    manufacturerEmail.Text = selectedManufacturer.manufacturerEmail;
                }
                else
                {
                    manufacturerName.Text = "";
                    manufacturerURL.Text = "";
                    manufacturerEmail.Text = "";
                }
            }
            if (selectedMenu == "user")
            {
                if (userDataGrid.SelectedItem is User selectedUser)
                {
                    userName.Text = selectedUser.fullName;
                    userEmail.Text = selectedUser.email;                    
                    userPrivilege.SelectedValue = selectedUser.privilegeLevel;
                    userActive.IsChecked = selectedUser.isActive == 1;
                }
                else
                {
                    userName.Text = "";
                    userEmail.Text = "";
                    userPrivilege.SelectedValue = 1;
                    userActive.IsChecked = false;
                }
            }
            if (selectedMenu == "storage")
            {
                if (storageDataGrid.SelectedItem is Storage selectedPart)
                {
                    partName.Text = selectedPart.partName;
                    partAmount.Text = selectedPart.partAmount.ToString();
                }
                else
                {
                    partName.Text = "";
                    partAmount.Text = "";
                }
            }
            if (selectedMenu == "repair")
            {
                if (repairDataGrid.SelectedItem is Repair selectedRepair)
                {
                    //repairID.Text = selectedRepair.repairID;
                    userEmail.Text = selectedRepair.userEmail;
                    address.Text = $"{selectedRepair.postalCode}, {selectedRepair.city} {selectedRepair.address}";
                    phoneNumber.Text = selectedRepair.phoneNumber.ToString();
                    if(selectedRepair.phoneInspection == 0)
                    {
                        phoneInspection.Foreground = Brushes.Red;
                        phoneInspection.Text = "Inspection is not required";
                    }
                    else
                    {
                        phoneInspection.Foreground = Brushes.Green;
                        phoneInspection.Text = "Inspection is required";
                    }
                    price.Text = selectedRepair.price.ToString();
                    repairStatus.SelectedValue = selectedRepair.status;
                }
                else
                {
                    partName.Text = "";
                    partAmount.Text = "";
                }
            }
            if (selectedMenu == "order")
            {
                if (orderDataGrid.SelectedItem is Order selectedOrder)
                {
                    userEmail.Text = selectedOrder.userEmail;
                    address.Text = $"{selectedOrder.postalCode}, {selectedOrder.city} {selectedOrder.address}";
                    phoneNumber.Text = selectedOrder.phoneNumber.ToString();
                    phoneName.Text = selectedOrder.phoneName;
                    phoneColorName.Text = selectedOrder.phoneColorName;
                    phoneColorHex.Background =
                    (SolidColorBrush)(new BrushConverter().ConvertFrom(selectedOrder.phoneColorHex));

                    phoneRamStorage.Text = $"{selectedOrder.phoneRam} / {selectedOrder.phoneStorage}";
                    priceOrder.Text = selectedOrder.price.ToString();
                    amountOrder.Text = selectedOrder.amount.ToString();
                    orderStatus.SelectedValue = selectedOrder.status;
                }
                else
                {
                    partName.Text = "";
                    partAmount.Text = "";
                }
            }
        }

        
    }
}
