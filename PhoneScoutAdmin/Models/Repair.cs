using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
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
}
