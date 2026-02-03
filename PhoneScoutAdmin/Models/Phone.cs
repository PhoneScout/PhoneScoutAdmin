using System.Text.Json.Serialization;

namespace PhoneScoutAdmin
{
    public class Phone
    {
        [JsonPropertyName("phoneID")]
        public int phoneID { get; set; }

        [JsonPropertyName("phoneName")]
        public string phoneName { get; set; }

        [JsonPropertyName("manufacturerName")]
        public string manufacturerName { get; set; }

        [JsonPropertyName("phonePrice")]
        public int phonePrice { get; set; }

        [JsonPropertyName("phoneInstore")]
        public string phoneInStore { get; set; }

        [JsonPropertyName("phoneAvailable")]
        public int phoneAvailable { get; set; }
    }
}
