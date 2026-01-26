using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace PhoneScoutAdmin
{
    public class Phone
    {
        [JsonPropertyName("phoneID")]
        public int phoneID { get; set; }

        [JsonPropertyName("phoneName")]
        public string phoneName { get; set; }

        [JsonPropertyName("phonePrice")]
        public int phonePrice { get; set; }

        [JsonPropertyName("phoneInStore")]
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

    public partial class AdminInterface : Window
    {
        string selectedMenu = "phones";
        public ObservableCollection<Phone> phones { get; set; } = new ObservableCollection<Phone>();
        public ObservableCollection<Manufacturer> manufacturers { get; set; } = new ObservableCollection<Manufacturer>();

        public AdminInterface()
        {
            InitializeComponent();
        }

        private async void loadManufacturers(object sender, RoutedEventArgs e)
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
                            manufacturers.Add(manufacturer);
                        }
                    }

                    dataGrid.ItemsSource = manufacturersList;
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

        private async void updateManufacturer(object sender, RoutedEventArgs e)
        {
            var id = ((Manufacturer)dataGrid.SelectedValue).manufacturerId;
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wpfManufacturer/{id}"; // your API endpoint

                // Create DTO from your input fields
                var manufacturer = new Manufacturer
                {
                    manufacturerName = manufacturerName.Text,
                    manufacturerEmail = manufacturerEmail.Text,
                    manufacturerUrl = manufacturerUrl.Text
                };

                // Serialize DTO to JSON
                string json = JsonSerializer.Serialize(manufacturer);
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

        private async void deleteManufacturer(object sender, RoutedEventArgs e)
        {
            var manufacturerID = ((Manufacturer)dataGrid.SelectedValue).manufacturerId;
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wpfManufacturer/{manufacturerID}"; // your API endpoint

                // Send PUT request
                var response = await client.DeleteAsync(url);

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



        private async void loadPhones(object sender, RoutedEventArgs e)
        {
            selectedMenu = "phone";
            using HttpClient client = new HttpClient();
            try
            {
                string url = "http://localhost:5175/api/filterPage/GetAllPhones"; // your API endpoint
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

                    dataGrid.ItemsSource = phones;
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
            var id = ((Phone)dataGrid.SelectedValue).phoneID;
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wpfManufacturer/{id}"; // your API endpoint

                // Create DTO from your input fields
                var phone = new Phone
                {
                    phoneName = phoneName.Text,
                    phoneInStore = phoneInStore.Text,
                    phonePrice = int.Parse(phonePrice.Text),
                    phoneAvailable = int.Parse(phoneAvailable.Text)
                };

                // Serialize DTO to JSON
                string json = JsonSerializer.Serialize(phone);
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
            var phoneID = ((Phone)dataGrid.SelectedValue).phoneID;
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wpfManufacturer/{phoneID}"; // your API endpoint

                // Send PUT request
                var response = await client.DeleteAsync(url);

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
        private async void modifyFullPhone(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedValue == null)
            {
                MessageBox.Show("No phone is selected!");
            }
            else
            {
                PhoneDetails window = new PhoneDetails(((Phone)dataGrid.SelectedValue).phoneID);
                window.Show();
            }
        }

        public void populateInformationPart()
        {
            if (selectedMenu == "manufacturer")
            {
                manufacturerInformations.Visibility = Visibility.Visible;
                phoneInformations.Visibility = Visibility.Collapsed;
            }
            else if (selectedMenu == "phone")
            {
                manufacturerInformations.Visibility = Visibility.Collapsed;
                phoneInformations.Visibility = Visibility.Visible;
            }
        }

        private void showInfos(object sender, SelectionChangedEventArgs e)
        {
            if (selectedMenu == "manufacturer")
            {
                manufacturerName.Text = "";
                manufacturerEmail.Text = "";
                manufacturerUrl.Text = "";
                manufacturerName.Text = ((Manufacturer)dataGrid.SelectedValue).manufacturerName;
                manufacturerEmail.Text = ((Manufacturer)dataGrid.SelectedValue).manufacturerEmail;
                manufacturerUrl.Text = ((Manufacturer)dataGrid.SelectedValue).manufacturerUrl;
            }

            if (selectedMenu == "phone")
            {
                phoneName.Text = "";
                phoneInStore.Text = "";
                phonePrice.Text = "";
                phoneAvailable.Text = "";
                phoneName.Text = ((Phone)dataGrid.SelectedValue).phoneName;
                phoneInStore.Text = ((Phone)dataGrid.SelectedValue).phoneInStore;
                phonePrice.Text = ((Phone)dataGrid.SelectedValue).phonePrice.ToString();
                phonePrice.Text = ((Phone)dataGrid.SelectedValue).phoneAvailable.ToString();


            }
        }
    } 
}