using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhoneScoutAdmin.Models
{
    public class PhoneImage
    {
        public byte[] ImageData { get; set; }      
        public string FileName { get; set; }       
        public BitmapImage Preview { get; set; }
    }
}
