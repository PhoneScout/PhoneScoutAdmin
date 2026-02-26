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

        [JsonPropertyName("userEmail")]
        public string userEmail { get; set; }

        [JsonPropertyName("userName")]
        public string userName { get; set; }

        [JsonPropertyName("billingPostalCode")]
        public int billingPostalCode { get; set; }

        [JsonPropertyName("billingCity")]
        public string billingCity { get; set; }

        [JsonPropertyName("billingAddress")]
        public string billingAddress { get; set; }

        [JsonPropertyName("billingPhoneNumber")]
        public long billingPhoneNumber { get; set; }

        [JsonPropertyName("deliveryPostalCode")]
        public int deliveryPostalCode { get; set; }

        [JsonPropertyName("deliveryCity")]
        public string deliveryCity { get; set; }

        [JsonPropertyName("deliveryAddress")]
        public string deliveryAddress { get; set; }

        [JsonPropertyName("deliveryPhoneNumber")]
        public long deliveryPhoneNumber { get; set; }

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
