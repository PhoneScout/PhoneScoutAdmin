using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
    public class LoginResponse
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Privilege { get; set; }
        public string Token { get; set; }
        public int Active { get; set; }   // ADD THIS
    }
}
