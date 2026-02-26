using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
    public class Event
    {
        public int eventID { get; set; }
        public string eventHostName { get; set; }
        public string eventName { get; set; }
        public DateTime eventDate { get; set; }
        public string eventURL { get; set; }
    }
}
