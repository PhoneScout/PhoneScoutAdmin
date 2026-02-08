using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
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
}
