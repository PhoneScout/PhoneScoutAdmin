using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
    public class Part
    {
        [JsonPropertyName("partID")]
        public int partID { get; set; }

        [JsonPropertyName("partName")]
        public string partName { get; set; }

        [JsonPropertyName("partAmount")]
        public int partAmount { get; set; }
    }
}
