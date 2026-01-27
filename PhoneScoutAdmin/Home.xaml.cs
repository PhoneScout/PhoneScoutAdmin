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

    public partial class Home : Window
    {
        string selectedMenu = "phones";
        public ObservableCollection<Phone> phones { get; set; } = new ObservableCollection<Phone>();
        public ObservableCollection<Manufacturer> manufacturers { get; set; } = new ObservableCollection<Manufacturer>();
        public Home()
        {
            InitializeComponent();
        }

















        private async void loadPhones(object sender, RoutedEventArgs e)
        {
            phoneDataGrid.SelectedItem = null;
            manufacturerDataGrid.SelectedItem = null;
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




        public void populateInformationPart()
        {
            if (selectedMenu == "manufacturer")
            {
                manufacturerDetails.Visibility = Visibility.Visible;
                phoneDetails.Visibility = Visibility.Collapsed;
            }
            else if (selectedMenu == "phone")
            {
                manufacturerDetails.Visibility = Visibility.Collapsed;
                phoneDetails.Visibility = Visibility.Visible;
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
        }



    }
}
