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

        [JsonPropertyName("partName")]
        public string partName { get; set; }

        [JsonPropertyName("partAmount")]
        public int partAmount { get; set; }
    }
    public class ComboItem
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }

        public override string? ToString()
        {
            return $"{Level} - {Name}";
        }
    }


    public partial class Home : Window
    {
        string selectedMenu = "phones";
        public ObservableCollection<Phone> phones { get; set; } = new ObservableCollection<Phone>();
        public ObservableCollection<Manufacturer> manufacturers { get; set; } = new ObservableCollection<Manufacturer>();
        public ObservableCollection<User> users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Storage> parts { get; set; } = new ObservableCollection<Storage>();

        public ObservableCollection<ComboItem> Items { get; } =
    new ObservableCollection<ComboItem>
    {
        new ComboItem { Id = 1, Level = 1, Name = "User" },
        new ComboItem { Id = 2, Level = 2, Name = "Manufacturer" },
        new ComboItem { Id = 3, Level = 3, Name = "Admin" },
    };
        public ComboItem SelectedItem { get; set; }






        public Home()
        {
            InitializeComponent();
            DataContext = this;
        }

















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
        private async void loadStorage(object sender, RoutedEventArgs e)
        {
            phoneDataGrid.SelectedItem = null;
            manufacturerDataGrid.SelectedItem = null;
            userDataGrid.SelectedItem = null;
            storageDataGrid.SelectedItem = null;
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
                        partList.Clear();
                        foreach (var part in partList)
                        {
                            partList.Add(part);
                        }
                    }

                    storageDataGrid.ItemsSource = partList;
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
                if (userDataGrid.SelectedItem is Storage selectedPart)
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
        }
    }
}
