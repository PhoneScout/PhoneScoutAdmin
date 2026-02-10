using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
    public class Order
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("orderID")]
        public string orderID { get; set; }

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
}
