using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
    public class UserInfo
    {
        public string email { get; set; }
        public int privilege { get; set; }
        public int active { get; set; }
    }
}
