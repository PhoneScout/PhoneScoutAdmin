using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PhoneScoutAdmin.Models
{
    public class FullPhone
    {

        public int? phoneId { get; set; }
        public string? phoneName { get; set; }
        public int? phoneAntutu { get; set; }
        public int? phoneResolutionHeight { get; set; }
        public int? phoneResolutionWidth { get; set; }
        public decimal? screenSize { get; set; }
        public int? screenRefreshRate { get; set; }
        public int? screenMaxBrightness { get; set; }
        public int? screenSharpness { get; set; }

        public double? connectionMaxWifi { get; set; }
        public decimal? connectionMaxBluetooth { get; set; }
        public int? connectionMaxMobileNetwork { get; set; }
        public int? connectionDualSim { get; set; }
        public int? connectionEsim { get; set; }
        public int? connectionNfc { get; set; }
        public decimal? connectionConnectionSpeed { get; set; }
        public int? connectionJack { get; set; }

        public int? sensorsInfrared { get; set; }

        public int? batteryCapacity { get; set; }
        public int? batteryMaxChargingWired { get; set; }
        public int? batteryMaxChargingWireless { get; set; }

        public decimal? caseHeight { get; set; }
        public decimal? caseWidth { get; set; }
        public decimal? caseThickness { get; set; }
        public decimal? phoneWeight { get; set; }
        public DateOnly? phoneReleaseDate { get; set; }
        public int? phonePrice { get; set; }
        public int? phoneInStore { get; set; }
        public int? phoneInStoreAmount { get; set; }

        // Related entities
        public string? backMaterial { get; set; }
        public string? batteryType { get; set; }
        public string? chargerType { get; set; }
        public string? cpuName { get; set; }
        public int? cpuClock { get; set; }
        public int? cpuCores { get; set; }
        public int? cpuTech { get; set; }
        public string? manufacturerName { get; set; }
        public string? manufacturerURL { get; set; }
        public string? ramSpeed { get; set; }
        public string? screenType { get; set; }
        public string? fingerprintType { get; set; }
        public string? fingerprintPlace { get; set; }
        public string? storageSpeed { get; set; }
        public string? waterproofType { get; set; }
        public string? speakerType { get; set; }

        // Collections
        public List<ColorDTO> colors { get; set; }
        public List<Camera> cameras { get; set; }
        public List<RamStorage> ramStoragePairs { get; set; }
    }
}


