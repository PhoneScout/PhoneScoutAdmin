using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Policy;
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
using PhoneScoutAdmin.Models;

namespace PhoneScoutAdmin
{
    /// <summary>
    /// Interaction logic for ManufacturerHome.xaml
    /// </summary>
    public partial class ManufacturerHome : Window
    {
        public ObservableCollection<Phone> phones { get; set; } = new ObservableCollection<Phone>();
        public ObservableCollection<Manufacturer> manufacturers { get; set; } = new ObservableCollection<Manufacturer>();

        string selectedMenu = "phone";
        Manufacturer activeManufacturer = new Manufacturer();
        string activeManufacturerName = "Xiaomi";


        public ManufacturerHome()
        {
            InitializeComponent();
        }

        //Phones Requests
        private async void loadPhones(object sender, RoutedEventArgs e)
        {
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
                            
                            if (phone.manufacturerName == activeManufacturerName && phone.phoneAvailable == 0)
                            {
                                phones.Add(phone);
                            }
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
        private async void updatePhone(object sender, RoutedEventArgs e)
        {
            var selectedPhone = ((Phone)phoneDataGrid.SelectedValue);
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wpfPhone/" + selectedPhone.phoneID; // your API endpoint

                // Create DTO from your input fields

                selectedPhone.phoneName = phoneName.Text;
                if (phoneInStore.IsChecked == true)
                {
                    selectedPhone.phoneInStore = "van";
                }
                else
                {
                    selectedPhone.phoneInStore = "nincs";
                }
                selectedPhone.phonePrice = int.Parse(phonePrice.Text);
                if (phoneAvailable.IsChecked == true)
                {
                    selectedPhone.phoneAvailable = 1;
                }
                else
                {
                    selectedPhone.phoneAvailable = 0;
                }


                // Serialize DTO to JSON
                string json = JsonSerializer.Serialize(selectedPhone);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send PUT request
                var response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(result, "Success");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error: {error}", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
        private async void deletePhone(object sender, RoutedEventArgs e)
        {
            var selectedPhone = ((Phone)phoneDataGrid.SelectedValue);
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wpfPhone/" + selectedPhone.phoneID; // your API endpoint

                // Create DTO from your input fields

                selectedPhone.phoneName = phoneName.Text;
                if (phoneInStore.IsChecked == true)
                {
                    selectedPhone.phoneInStore = "van";
                }
                else
                {
                    selectedPhone.phoneInStore = "nincs";
                }
                selectedPhone.phonePrice = int.Parse(phonePrice.Text);
                
                selectedPhone.phoneAvailable = 1;
                


                // Serialize DTO to JSON
                string json = JsonSerializer.Serialize(selectedPhone);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send PUT request
                var response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(result, "Success");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error: {error}", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
        private async void createPhone(object sender, RoutedEventArgs e)
        {
            PhoneDetails window = new PhoneDetails(0);
            window.Show();
        }



        //Manufacturers Requests
        private async void loadManufacturer(object sender, RoutedEventArgs e)
        {
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
                            if (manufacturer.manufacturerName == activeManufacturerName)
                            {
                                activeManufacturer.manufacturerName = manufacturer.manufacturerName;
                                activeManufacturer.manufacturerEmail = manufacturer.manufacturerEmail;
                                activeManufacturer.manufacturerUrl = manufacturer.manufacturerUrl;
                            }
                            
                        }
                    }

                    
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

        private async void updateManufacturer(object sender, RoutedEventArgs e)
        {
            var selectedManufacturer = activeManufacturer;
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wpfManufacturer/" + selectedManufacturer.manufacturerId; // your API endpoint


                selectedManufacturer.manufacturerName = manufacturerName.Text;
                selectedManufacturer.manufacturerEmail = manufacturerEmail.Text;
                selectedManufacturer.manufacturerUrl = manufacturerURL.Text;


                // Serialize DTO to JSON
                string json = JsonSerializer.Serialize(selectedManufacturer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send PUT request
                var response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(result, "Success");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error: {error}", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }



        public void populateInformationPart()
        {
            phoneDetails.Visibility = Visibility.Collapsed;
            manufacturerDetails.Visibility = Visibility.Collapsed;

            if (selectedMenu == "phone")
            {
                phoneDetails.Visibility = Visibility.Visible;
            }
            if (selectedMenu == "manufacturer")
            {
                manufacturerDetails.Visibility = Visibility.Visible;
                manufacturerName.Text = activeManufacturer.manufacturerName;
                manufacturerEmail.Text = activeManufacturer.manufacturerEmail;
                manufacturerURL.Text = activeManufacturer.manufacturerUrl;
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

        }

    }
}
