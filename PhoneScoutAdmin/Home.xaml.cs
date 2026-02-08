using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
using PhoneScoutAdmin.ViewModels;

namespace PhoneScoutAdmin
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>

    
    

    public class User
    {
        [JsonPropertyName("userID")]
        public int userID { get; set; }

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

        [JsonPropertyName("userID")]
        public int userID { get; set; }

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
        [JsonPropertyName("orderID")]
        public int orderID { get; set; }

        [JsonPropertyName("userID")]
        public int userID { get; set; }

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
        
        public Home()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        
        
    }
}
