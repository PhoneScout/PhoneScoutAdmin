using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Text.Json.Serialization;

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

    public partial class AdminInterface : Window
    {
        public ObservableCollection<Phone> phones { get; set; } = new ObservableCollection<Phone>();

        public AdminInterface()
        {
            InitializeComponent();

            // Bind the DataGrid to the ObservableCollection
            phonesPlace.ItemsSource = phones;

            // Load data from API
            LoadPhones();
        }

        private async void LoadPhones()
        {
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
    }
}