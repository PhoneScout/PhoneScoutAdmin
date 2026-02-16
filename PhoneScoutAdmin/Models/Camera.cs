using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.Models
{
    public class Camera
    {
        public string cameraName { get; set; }
        public int cameraResolution { get; set; }
        public string cameraAperture { get; set; }
        public int cameraFocalLength { get; set; }
        public string cameraOis { get; set; }
        public string cameraType { get; set; }
    }
}
