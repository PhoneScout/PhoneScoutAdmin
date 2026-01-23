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


namespace PhoneScoutAdmin
{
    public class Phone
    {

        [JsonPropertyName("phoneName")]
        public string phoneName { get; set; }

        [JsonPropertyName("phonePrice")]
        public int phonePrice { get; set; }

        [JsonPropertyName("phoneInStore")]
        public string phoneInStore { get; set; }
    }

    public class Manufacturer
    {

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
                string url = "http://localhost:5175/api/wcfManufacturer"; 
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

                    phonesPlace.ItemsSource = manufacturersList;
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
            var oldName = ((Manufacturer)phonesPlace.SelectedValue).manufacturerName;
            using HttpClient client = new HttpClient();
            try
            {
                string url = $"http://localhost:5175/api/wcfManufacturer/{oldName}"; // your API endpoint

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


        private async void loadPhones(object sender, RoutedEventArgs e)
        {
            selectedMenu = "phones";
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

                    phonesPlace.ItemsSource = phones;
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

        public void populateInformationPart()
        {
            if(selectedMenu == "manufacturer")
            {
                manufacturerInformations.Visibility = Visibility.Visible;
            }            
        }

        private void showInfos(object sender, SelectionChangedEventArgs e)
        {
            if (selectedMenu == "manufacturer")
            {
                manufacturerName.Text = ((Manufacturer)phonesPlace.SelectedValue).manufacturerName;
                manufacturerEmail.Text = ((Manufacturer)phonesPlace.SelectedValue).manufacturerEmail;
                manufacturerUrl.Text = ((Manufacturer)phonesPlace.SelectedValue).manufacturerUrl;
            }
        }
    }
}