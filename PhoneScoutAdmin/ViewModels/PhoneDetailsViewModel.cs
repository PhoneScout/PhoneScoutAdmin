using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.ViewModels;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PhoneScoutAdmin
{
    public class PhoneDetailsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // SECTION VMs
        public GeneralInfosViewModel GeneralInfosVM { get; } = new GeneralInfosViewModel();
        public CPUdetailsViewModel CpuVM { get; } = new CPUdetailsViewModel();
        public SensorsViewModel SensorsVM { get; } = new SensorsViewModel();
        public ScreenViewModel ScreenVM { get; } = new ScreenViewModel();
        public ConnectivityViewModel ConnectivityVM { get; } = new ConnectivityViewModel();
        public BatteryChargingViewModel BatteryChargingVM { get; } = new BatteryChargingViewModel();
        public BodySpeakerViewModel BodySpeakerVM { get; } = new BodySpeakerViewModel();
        public CameraSectionViewModel CameraSectionVM { get; } = new CameraSectionViewModel();
        public ColorSectionViewModel ColorSectionVM { get; } = new ColorSectionViewModel();
        public RamStorageSectionViewModel RamStorageSectionVM { get; } = new RamStorageSectionViewModel();
        public RamStorageViewModel RamStorageVM { get; } = new RamStorageViewModel();
        public ImageViewModel ImageVM { get; } = new ImageViewModel();


        // Add others later...

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        private int? _currentPhoneId;
        public int? CurrentPhoneId
        {
            get => _currentPhoneId;
            set
            {
                _currentPhoneId = value;
                OnPropertyChanged(nameof(CurrentPhoneId));
            }
        }


        // COMMANDS
        public ICommand ShowGeneralInfosCommand { get; }
        public ICommand ShowCpuCommand { get; }
        public ICommand ShowSensorsCommand { get; }
        public ICommand ShowScreenCommand { get; }
        public ICommand ShowConnectivityCommand { get; }
        public ICommand ShowBatteryChargingCommand { get; }
        public ICommand ShowBodySpeakerCommand { get; }
        public ICommand ShowCameraCommand { get; }
        public ICommand ShowColorCommand { get; }
        public ICommand ShowRamStorageCommand { get; }
        public ICommand ShowImagesCommand { get; }
        public ICommand SavePhoneCommand { get; }
        public ICommand LoadPhoneCommand { get; }




        public double TotalProgress => CpuVM.Progress + SensorsVM.Progress + ConnectivityVM.Progress + ScreenVM.Progress; // expand later

        public PhoneDetailsViewModel()
        {
            ShowGeneralInfosCommand = new RelayCommand(() => CurrentViewModel = GeneralInfosVM);
            ShowCpuCommand = new RelayCommand(() => CurrentViewModel = CpuVM);
            ShowSensorsCommand = new RelayCommand(() => CurrentViewModel = SensorsVM);
            ShowConnectivityCommand = new RelayCommand(() => CurrentViewModel = ConnectivityVM);
            ShowScreenCommand = new RelayCommand(() => CurrentViewModel = ScreenVM);
            ShowBatteryChargingCommand = new RelayCommand(() => CurrentViewModel = BatteryChargingVM);
            ShowBodySpeakerCommand = new RelayCommand(() => CurrentViewModel = BodySpeakerVM);
            ShowCameraCommand = new RelayCommand(() => CurrentViewModel = CameraSectionVM);
            ShowColorCommand = new RelayCommand(() => CurrentViewModel = ColorSectionVM);
            ShowRamStorageCommand = new RelayCommand(() => CurrentViewModel = RamStorageSectionVM);
            ShowImagesCommand = new RelayCommand(() => CurrentViewModel = ImageVM);
            SavePhoneCommand = new RelayCommand(() => CurrentViewModel = CreatePhone());
            LoadPhoneCommand = new RelayCommand<object>(async (param) =>
            {
                if (param == null) return;

                if (int.TryParse(param.ToString(), out int id))
                {
                    await LoadPhone(id);
                    MessageBox.Show("gomb");
                }
            });





            GeneralInfosVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GeneralInfosVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            CpuVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CpuVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            SensorsVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SensorsVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            ConnectivityVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ConnectivityVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            ScreenVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ScreenVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            BatteryChargingVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BatteryChargingVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            BodySpeakerVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BodySpeakerVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };
            ImageVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ImageVM.Progress))
                    OnPropertyChanged(nameof(TotalProgress));
            };


            // Default view
            CurrentViewModel = GeneralInfosVM;
        }

        private async Task LoadPhone(int phoneId)
        {
            CurrentPhoneId = phoneId;
            phoneId = 1; // for testing, remove later

            MessageBox.Show($"Loading phone with ID: {phoneId}");

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/phonePage/{phoneId}";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync();

            var phone = JsonSerializer.Deserialize<FullPhone>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (phone == null)
                return;

            MapPhoneToViewModels(phone);
        }

        private void MapPhoneToViewModels(FullPhone phone)
        {
            // GENERAL
            GeneralInfosVM.PhoneName = phone.phoneName;
            GeneralInfosVM.PhonePrice = phone.phonePrice.ToString();
            GeneralInfosVM.PhoneWeight = phone.phoneWeight.ToString();
            GeneralInfosVM.ManufacturerName = phone.manufacturerName;
            GeneralInfosVM.ReleaseDate = phone.phoneReleaseDate.ToString();
            GeneralInfosVM.PhoneInStore = phone.phoneInStore == 1 ? true : false;

            //CPU
            CpuVM.CpuName = phone.cpuName;
            CpuVM.AntutuScore = phone.phoneAntutu.ToString();
            CpuVM.ClockSpeed = phone.cpuClock.ToString();
            CpuVM.CoreNumber = phone.cpuCores.ToString();
            CpuVM.ManufacturingTech = phone.cpuTech.ToString();

            //SENSORS
            SensorsVM.FinSenPlace = phone.fingerprintPlace;
            SensorsVM.FinSenType = phone.fingerprintType;
            SensorsVM.Infrared = phone.sensorsInfrared == 1 ? true : false;

            //SCREEN
            ScreenVM.ScreenType = phone.screenType;
            ScreenVM.ScreenResH = phone.phoneResolutionHeight.ToString();
            ScreenVM.ScreenResW = phone.phoneResolutionWidth.ToString();
            ScreenVM.ScreenSize = phone.screenSize.ToString();
            ScreenVM.ScreenRefreshRate = phone.screenRefreshRate.ToString();
            ScreenVM.ScreenMaxBrightness = phone.screenMaxBrightness.ToString();

            //CONNECTIVITY
            ConnectivityVM.Wifi = phone.connectionMaxWifi.ToString();
            ConnectivityVM.Bluetooth = phone.connectionMaxBluetooth.ToString();
            ConnectivityVM.MobileNetwork = phone.connectionMaxMobileNetwork.ToString();
            ConnectivityVM.DualSim = phone.connectionDualSim == 1 ? true : false;
            ConnectivityVM.ESim = phone.connectionEsim == 1 ? true : false;
            ConnectivityVM.Nfc = phone.connectionNfc == 1 ? true : false;
            ConnectivityVM.Jack = phone.connectionJack == 1 ? true : false;
            ConnectivityVM.ConnectionSpeed = phone.connectionConnectionSpeed.ToString();

            //BATTERY
            BatteryChargingVM.BatteryCapacity = (int)phone.batteryCapacity;
            BatteryChargingVM.BatteryType = phone.batteryType;
            BatteryChargingVM.MaxWiredCharging = (int)phone.batteryMaxChargingWired;
            BatteryChargingVM.MaxWirelessCharging = (int)phone.batteryMaxChargingWireless;
            BatteryChargingVM.BatteryType = phone.batteryType;

            //BODY SPEAKER
            BodySpeakerVM.BodyHeight = phone.caseHeight.ToString();
            BodySpeakerVM.BodyWidth = phone.caseWidth.ToString();
            BodySpeakerVM.BodyThickness = phone.caseThickness.ToString();
            BodySpeakerVM.WaterproofType = phone.waterproofType;
            BodySpeakerVM.BodyBackMaterial = phone.backMaterial;
            BodySpeakerVM.SpeakerType = phone.speakerType;

            //COLORS
            ColorSectionVM.Colors.Clear();

            foreach (var color in phone.colors ?? new List<ColorDTO>())
            {
                ColorSectionVM.Colors.Add(new ColorViewModel
                {
                    ColorName = color.colorName,
                    ColorHex = color.colorHex
                });
            }

            CameraSectionVM.Cameras.Clear();

            foreach (var camera in phone.cameras ?? new List<Camera>())
            {
                CameraSectionVM.Cameras.Add(new CameraViewModel
                {
                    CameraName = camera.cameraName,
                    CameraResolution = camera.cameraResolution,
                    CameraAperture = camera.cameraAperture.ToString(),
                    CameraFocalLength = camera.cameraFocalLength,
                    CameraOIS = camera.cameraOis==1?true:false,
                    CameraType = camera.cameraType.ToString(),
                });
            }


            RamStorageVM.RamSpeed = phone.ramSpeed;
            RamStorageVM.StorageSpeed = phone.storageSpeed;

            RamStorageSectionVM.RamStorages.Clear();

            foreach (var ramStorage in phone.ramStoragePairs ?? new List<RamStorage>())
            {
                RamStorageSectionVM.RamStorages.Add(new RamStorageViewModel
                {
                    RamAmount = ramStorage.ramAmount,
                    StorageAmount = ramStorage.storageAmount,
                });
            }



        }


        private async Task CreatePhone()
        {
            try
            {
                List<ColorDTO> selectedColors = new List<ColorDTO>();
                List<Camera> selectedCameras = new List<Camera>();
                List<RamStorage> selectedRamStorages = new List<RamStorage>();



                foreach (var color in ColorSectionVM.Colors)
                {
                    selectedColors.Add(new ColorDTO
                    {
                        colorName = color.ColorName,
                        colorHex = color.ColorHex
                    });
                }

                foreach (var camera in CameraSectionVM.Cameras)
                {
                    selectedCameras.Add(new Camera
                    {
                        cameraName = camera.CameraName,
                        cameraResolution = camera.CameraResolution,
                        cameraAperture = camera.CameraAperture,
                        cameraFocalLength = camera.CameraFocalLength,
                        cameraOis = camera.CameraOIS == true ? 1 : 0,
                        cameraType = camera.CameraType,
                    });
                }

                foreach (var ramStorage in RamStorageSectionVM.RamStorages)
                {
                    selectedRamStorages.Add(new RamStorage
                    {
                        ramAmount = ramStorage.RamAmount,
                        storageAmount = ramStorage.StorageAmount,
                    });
                }


                var newPhone = new FullPhone
                {
                    // GENERAL
                    phoneName = (GeneralInfosVM.PhoneName != "" ? GeneralInfosVM.PhoneName : ""),
                    phonePrice = (GeneralInfosVM.PhonePrice is int ? int.Parse(GeneralInfosVM.PhonePrice) : 0),
                    phoneWeight = (GeneralInfosVM.PhoneWeight is int ? int.Parse(GeneralInfosVM.PhoneWeight) : 0),
                    manufacturerName = (GeneralInfosVM.ManufacturerName != "" ? GeneralInfosVM.ManufacturerName : ""),
                    phoneReleaseDate = (GeneralInfosVM.ReleaseDate is DateOnly ? DateOnly.Parse(GeneralInfosVM.ReleaseDate) : DateOnly.MinValue),
                    phoneInStore = (GeneralInfosVM.PhoneInStore == true ? 1 : 0),


                    //CPU
                    cpuName = (CpuVM.CpuName != "" ? CpuVM.CpuName : ""),
                    phoneAntutu = (CpuVM.AntutuScore is int ? int.Parse(CpuVM.AntutuScore) : 0),
                    cpuClock = (CpuVM.ClockSpeed is int ? int.Parse(CpuVM.ClockSpeed) : 0),
                    cpuCores = (CpuVM.CoreNumber is int ? int.Parse(CpuVM.CoreNumber) : 0),
                    cpuTech = (CpuVM.ManufacturingTech is int ? int.Parse(CpuVM.ManufacturingTech) : 0),

                    //SENSORS
                    fingerprintPlace = (SensorsVM.FinSenPlace != "" ? SensorsVM.FinSenPlace : ""),
                    fingerprintType = (SensorsVM.FinSenType != "" ? SensorsVM.FinSenType : ""),
                    sensorsInfrared = (SensorsVM.Infrared == true ? 1 : 0),

                    //SCREEN
                    screenType = (ScreenVM.ScreenType != "" ? ScreenVM.ScreenType : ""),
                    phoneResolutionHeight = (ScreenVM.ScreenResH is int ? int.Parse(ScreenVM.ScreenResH) : 0),
                    phoneResolutionWidth = (ScreenVM.ScreenResW is int ? int.Parse(ScreenVM.ScreenResW) : 0),
                    screenSize = (ScreenVM.ScreenSize is int ? int.Parse(ScreenVM.ScreenSize) : 0),
                    screenRefreshRate = (ScreenVM.ScreenRefreshRate is int ? int.Parse(ScreenVM.ScreenRefreshRate) : 0),
                    screenMaxBrightness = (ScreenVM.ScreenMaxBrightness is int ? int.Parse(ScreenVM.ScreenMaxBrightness) : 0),

                    //CONNECTIVITY
                    connectionMaxWifi = (ConnectivityVM.Wifi is double ? double.Parse(ConnectivityVM.Wifi) : 0),
                    connectionMaxBluetooth = (ConnectivityVM.Bluetooth is decimal ? decimal.Parse(ConnectivityVM.Bluetooth) : 0),
                    connectionMaxMobileNetwork = (ConnectivityVM.MobileNetwork is int ? int.Parse(ConnectivityVM.MobileNetwork) : 0),
                    connectionDualSim = (ConnectivityVM.DualSim == true ? 1 : 0),
                    connectionEsim = (ConnectivityVM.ESim == true ? 1 : 0),
                    connectionNfc = (ConnectivityVM.Jack == true ? 1 : 0),
                    connectionJack = (ConnectivityVM.Jack == true ? 1 : 0),
                    connectionConnectionSpeed = (ConnectivityVM.ConnectionSpeed is double ? double.Parse(ConnectivityVM.ConnectionSpeed) : 0),

                    //BATTERY
                    batteryCapacity = (BatteryChargingVM.BatteryCapacity is int ? BatteryChargingVM.BatteryCapacity : 0),
                    batteryType = (BatteryChargingVM.BatteryType != "" ? BatteryChargingVM.BatteryType : ""),
                    batteryMaxChargingWired = (BatteryChargingVM.MaxWiredCharging is int ? BatteryChargingVM.MaxWiredCharging : 0),
                    batteryMaxChargingWireless = (BatteryChargingVM.MaxWirelessCharging is int ? BatteryChargingVM.MaxWirelessCharging : 0),

                    //BODY SPEAKER
                    caseHeight = (BodySpeakerVM.BodyHeight is decimal ? decimal.Parse(BodySpeakerVM.BodyHeight) : 0),
                    caseWidth = (BodySpeakerVM.BodyWidth is decimal ? decimal.Parse(BodySpeakerVM.BodyWidth) : 0),
                    caseThickness = (BodySpeakerVM.BodyThickness is decimal ? decimal.Parse(BodySpeakerVM.BodyThickness) : 0),
                    waterproofType = (BodySpeakerVM.WaterproofType != "" ? BodySpeakerVM.WaterproofType : ""),
                    backMaterial = (BodySpeakerVM.BodyBackMaterial != "" ? BodySpeakerVM.BodyBackMaterial : ""),
                    speakerType = (BodySpeakerVM.SpeakerType != "" ? BodySpeakerVM.SpeakerType : ""),

                    //COLORS

                    colors = selectedColors,
                    cameras = selectedCameras,
                    ramStoragePairs = selectedRamStorages,

                    ramSpeed = (RamStorageVM.RamSpeed != "" ? RamStorageVM.RamSpeed : ""),
                    storageSpeed = (RamStorageVM.StorageSpeed != "" ? RamStorageVM.StorageSpeed : ""),
                
            };

                using HttpClient client = new HttpClient();
                string url = "http://localhost:5175/api/wpfPhone/phonePost";

                string json = JsonSerializer.Serialize(newPhone);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Save successful");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Save failed: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

    }
}