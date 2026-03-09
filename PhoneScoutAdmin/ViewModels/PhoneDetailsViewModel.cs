using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.ViewModels;
using PhoneScoutAdmin.Views;
using System;
using System.ComponentModel;
using System.IO;
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

        public ICommand SignOut { get; }
        public ICommand ExitApp { get; }


        public double TotalProgress =>
    Math.Round(
        GeneralInfosVM.Progress +
        CpuVM.Progress +
        SensorsVM.Progress +
        ConnectivityVM.Progress +
        ScreenVM.Progress +
        BatteryChargingVM.Progress +
        BodySpeakerVM.Progress,
        1);

        // expand later

        public PhoneDetailsViewModel(int phoneID)
        {
            CurrentPhoneId = phoneID;

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



            SavePhoneCommand = new RelayCommand(async () => await SavePhoneWithImages());


            _ = LoadPhone(phoneID);



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


            ExitApp = new RelayCommand(() =>
            {
                var result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            });

            // Default view
            CurrentViewModel = GeneralInfosVM;
        }

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
            SavePhoneCommand = new RelayCommand(async () => await SavePhoneWithImages());






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


            // Default view
            CurrentViewModel = GeneralInfosVM;
        }

        private async Task SavePhoneWithImages()
        {
            if (CurrentPhoneId == null)
                await CreatePhone();
            else
                await UpdatePhone();

            await UploadImages(); // this will now be called after save
        }


        private async Task LoadPhone(int phoneId)
        {
            CurrentPhoneId = phoneId;

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
            await ImageVM.LoadImages(phoneId);
            ImageVM.CurrentPhoneId = phoneId;
        }

        private void MapPhoneToViewModels(FullPhone phone)
        {
            // GENERAL
            GeneralInfosVM.PhoneName = phone.phoneName;
            GeneralInfosVM.PhoneWeight = phone.phoneWeight.ToString();
            GeneralInfosVM.ManufacturerName = phone.manufacturerName;
            GeneralInfosVM.ReleaseDate = (DateOnly)phone.phoneReleaseDate;


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
            ScreenVM.ScreenSharpness = phone.screenSharpness.ToString();
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
            BatteryChargingVM.BatteryCapacity = phone.batteryCapacity.ToString();
            BatteryChargingVM.BatteryType = phone.batteryType;
            BatteryChargingVM.MaxWiredCharging = phone.batteryMaxChargingWired.ToString();
            BatteryChargingVM.MaxWirelessCharging = phone.batteryMaxChargingWireless.ToString();
            BatteryChargingVM.BatteryType = phone.batteryType;
            BatteryChargingVM.ChargerType = phone.chargerType;

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
                    CameraOIS = camera.cameraOis == 1 ? true : false,
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

            MessageBox.Show(RamStorageVM.RamSpeed + RamStorageVM.StorageSpeed);

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
                    phonePrice = 0,
                    phoneWeight = decimal.TryParse(GeneralInfosVM.PhoneWeight, out var phoneWeightValue) ? phoneWeightValue : 0,
                    manufacturerName = (GeneralInfosVM.ManufacturerName != "" ? GeneralInfosVM.ManufacturerName : ""),
                    manufacturerEmail = "",
                    manufacturerURL = "",
                    phoneReleaseDate = GeneralInfosVM.ReleaseDate,
                    phoneInStore = 0,
                    phoneAvailable = 0,


                    //CPU
                    cpuName = (CpuVM.CpuName != "" ? CpuVM.CpuName : ""),
                    phoneAntutu = int.TryParse(CpuVM.AntutuScore, out var phoneAntutuValue) ? phoneAntutuValue : 0,
                    cpuClock = int.TryParse(CpuVM.ClockSpeed, out var cpuClockValue) ? cpuClockValue : 0,
                    cpuCores = int.TryParse(CpuVM.CoreNumber, out var cpuCoresValue) ? cpuCoresValue : 0,
                    cpuTech = int.TryParse(CpuVM.ManufacturingTech, out var cpuTechValue) ? cpuTechValue : 0,


                    //SENSORS
                    fingerprintPlace = (SensorsVM.FinSenPlace != "" ? SensorsVM.FinSenPlace : ""),
                    fingerprintType = (SensorsVM.FinSenType != "" ? SensorsVM.FinSenType : ""),
                    sensorsInfrared = (SensorsVM.Infrared == true ? 1 : 0),

                    //SCREEN
                    screenType = (ScreenVM.ScreenType != "" ? ScreenVM.ScreenType : ""),
                    phoneResolutionHeight = int.TryParse(ScreenVM.ScreenResH, out var phoneResolutionHeightValue) ? phoneResolutionHeightValue : 0,
                    phoneResolutionWidth = int.TryParse(ScreenVM.ScreenResW, out var phoneResolutionWidthValue) ? phoneResolutionWidthValue : 0,
                    screenSize = decimal.TryParse(ScreenVM.ScreenSize, out var screenSizeValue) ? screenSizeValue : 0,
                    screenRefreshRate = int.TryParse(ScreenVM.ScreenRefreshRate, out var screenRefreshRateValue) ? screenRefreshRateValue : 0,
                    screenMaxBrightness = int.TryParse(ScreenVM.ScreenMaxBrightness, out var screenMaxBrightnessValue) ? screenMaxBrightnessValue : 0,
                    screenSharpness = int.TryParse(ScreenVM.ScreenSharpness, out var ScreenSharpnessValue) ? ScreenSharpnessValue : 0,

                    //CONNECTIVITY
                    connectionMaxWifi = double.TryParse(ConnectivityVM.Wifi, out var connectionMaxWifiValue) ? connectionMaxWifiValue : 0,
                    connectionMaxBluetooth = decimal.TryParse(ConnectivityVM.Bluetooth, out var connectionMaxBluetoothValue) ? connectionMaxBluetoothValue : 0,
                    connectionMaxMobileNetwork = int.TryParse(ConnectivityVM.MobileNetwork, out var connectionMaxMobileNetworkValue) ? connectionMaxMobileNetworkValue : 0,
                    connectionDualSim = (ConnectivityVM.DualSim == true ? 1 : 0),
                    connectionEsim = (ConnectivityVM.ESim == true ? 1 : 0),
                    connectionNfc = (ConnectivityVM.Nfc == true ? 1 : 0),
                    connectionJack = (ConnectivityVM.Jack == true ? 1 : 0),
                    connectionConnectionSpeed = decimal.TryParse(ConnectivityVM.ConnectionSpeed, out var connectionConnectionSpeedValue) ? connectionConnectionSpeedValue : 0,

                    //BATTERY
                    batteryCapacity = int.TryParse(BatteryChargingVM.BatteryCapacity, out var batteryCapacityValue) ? batteryCapacityValue : 0,

                    batteryMaxChargingWired = int.TryParse(BatteryChargingVM.MaxWiredCharging, out var batteryMaxChargingWiredValue) ? batteryMaxChargingWiredValue : 0,

                    batteryMaxChargingWireless = int.TryParse(BatteryChargingVM.MaxWirelessCharging, out var batteryMaxChargingWirelessValue) ? batteryMaxChargingWirelessValue : 0,

                    batteryType = (BatteryChargingVM.BatteryType != "" ? BatteryChargingVM.BatteryType : ""),
                    chargerType = (BatteryChargingVM.ChargerType != "" ? BatteryChargingVM.ChargerType : ""),


                    //BODY SPEAKER
                    caseHeight = decimal.TryParse(BodySpeakerVM.BodyHeight, out var caseHeightValue) ? caseHeightValue : 0,
                    caseWidth = decimal.TryParse(BodySpeakerVM.BodyWidth, out var caseWidthValue) ? caseWidthValue : 0,
                    caseThickness = decimal.TryParse(BodySpeakerVM.BodyThickness, out var caseThicknessValue) ? caseThicknessValue : 0,
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
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var createdId = JsonSerializer.Deserialize<int>(resultJson);
                    CurrentPhoneId = createdId;
                    MessageBox.Show($"Phone created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"An error occured while creating the phone!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    MessageBox.Show(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private async Task UpdatePhone()
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
                    phoneWeight = decimal.TryParse(GeneralInfosVM.PhoneWeight, out var phoneWeightValue) ? phoneWeightValue : 0,
                    manufacturerName = (GeneralInfosVM.ManufacturerName != "" ? GeneralInfosVM.ManufacturerName : ""),
                    phoneReleaseDate = GeneralInfosVM.ReleaseDate,



                    //CPU
                    cpuName = (CpuVM.CpuName != "" ? CpuVM.CpuName : ""),
                    phoneAntutu = int.TryParse(CpuVM.AntutuScore, out var phoneAntutuValue) ? phoneAntutuValue : 0,
                    cpuClock = int.TryParse(CpuVM.ClockSpeed, out var cpuClockValue) ? cpuClockValue : 0,
                    cpuCores = int.TryParse(CpuVM.CoreNumber, out var cpuCoresValue) ? cpuCoresValue : 0,
                    cpuTech = int.TryParse(CpuVM.ManufacturingTech, out var cpuTechValue) ? cpuTechValue : 0,


                    //SENSORS
                    fingerprintPlace = (SensorsVM.FinSenPlace != "" ? SensorsVM.FinSenPlace : ""),
                    fingerprintType = (SensorsVM.FinSenType != "" ? SensorsVM.FinSenType : ""),
                    sensorsInfrared = (SensorsVM.Infrared == true ? 1 : 0),

                    //SCREEN
                    screenType = (ScreenVM.ScreenType != "" ? ScreenVM.ScreenType : ""),
                    phoneResolutionHeight = int.TryParse(ScreenVM.ScreenResH, out var phoneResolutionHeightValue) ? phoneResolutionHeightValue : 0,
                    phoneResolutionWidth = int.TryParse(ScreenVM.ScreenResW, out var phoneResolutionWidthValue) ? phoneResolutionWidthValue : 0,
                    screenSize = decimal.TryParse(ScreenVM.ScreenSize, out var screenSizeValue) ? screenSizeValue : 0,
                    screenRefreshRate = int.TryParse(ScreenVM.ScreenRefreshRate, out var screenRefreshRateValue) ? screenRefreshRateValue : 0,
                    screenMaxBrightness = int.TryParse(ScreenVM.ScreenMaxBrightness, out var screenMaxBrightnessValue) ? screenMaxBrightnessValue : 0,
                    screenSharpness = int.TryParse(ScreenVM.ScreenSharpness, out var screenSharpnessValue) ? screenSharpnessValue : 0,

                    //CONNECTIVITY
                    connectionMaxWifi = double.TryParse(ConnectivityVM.Wifi, out var connectionMaxWifiValue) ? connectionMaxWifiValue : 0,
                    connectionMaxBluetooth = decimal.TryParse(ConnectivityVM.Bluetooth, out var connectionMaxBluetoothValue) ? connectionMaxBluetoothValue : 0,
                    connectionMaxMobileNetwork = int.TryParse(ConnectivityVM.MobileNetwork, out var connectionMaxMobileNetworkValue) ? connectionMaxMobileNetworkValue : 0,
                    connectionDualSim = (ConnectivityVM.DualSim == true ? 1 : 0),
                    connectionEsim = (ConnectivityVM.ESim == true ? 1 : 0),
                    connectionNfc = (ConnectivityVM.Nfc == true ? 1 : 0),
                    connectionJack = (ConnectivityVM.Jack == true ? 1 : 0),
                    connectionConnectionSpeed = decimal.TryParse(ConnectivityVM.ConnectionSpeed, out var connectionConnectionSpeedValue) ? connectionConnectionSpeedValue : 0,

                    //BATTERY
                    batteryCapacity = int.TryParse(BatteryChargingVM.BatteryCapacity, out var batteryCapacityValue) ? batteryCapacityValue : 0,

                    batteryMaxChargingWired = int.TryParse(BatteryChargingVM.MaxWiredCharging, out var batteryMaxChargingWiredValue) ? batteryMaxChargingWiredValue : 0,

                    batteryMaxChargingWireless = int.TryParse(BatteryChargingVM.MaxWirelessCharging, out var batteryMaxChargingWirelessValue) ? batteryMaxChargingWirelessValue : 0,

                    batteryType = (BatteryChargingVM.BatteryType != "" ? BatteryChargingVM.BatteryType : ""),
                    chargerType = (BatteryChargingVM.ChargerType != "" ? BatteryChargingVM.ChargerType : ""),


                    //BODY SPEAKER
                    caseHeight = decimal.TryParse(BodySpeakerVM.BodyHeight, out var caseHeightValue) ? caseHeightValue : 0,
                    caseWidth = decimal.TryParse(BodySpeakerVM.BodyWidth, out var caseWidthValue) ? caseWidthValue : 0,
                    caseThickness = decimal.TryParse(BodySpeakerVM.BodyThickness, out var caseThicknessValue) ? caseThicknessValue : 0,
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
                string url = $"http://localhost:5175/api/wpfPhone/phoneUpdate/{CurrentPhoneId}";

                string json = JsonSerializer.Serialize(newPhone);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Phone modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"An error occured while saving the phone!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // Upload images after saving phone
        private async Task UploadImages()
        {
            if (CurrentPhoneId == null)
                return;

            using HttpClient client = new HttpClient();

            bool uploadedNewImage = false;

            // ======================
            // 1️⃣ Upload new images
            // ======================
            foreach (var image in ImageVM.Images.Where(i => i.IsNew))
            {
                using var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(image.ImageData);

                var mimeType = Path.GetExtension(image.FileName).ToLower() switch
                {
                    ".png" => "image/png",
                    ".bmp" => "image/bmp",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    _ => "image/jpeg"
                };

                imageContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);

                content.Add(imageContent, "file", image.FileName);

                string url =
                    $"http://localhost:5175/api/blob/PostPicture/{CurrentPhoneId}/{image.IsIndex.ToString().ToLower()}";

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    uploadedNewImage = true;
                }
                else
                {
                    MessageBox.Show(await response.Content.ReadAsStringAsync());
                }
            }

            // 🔥 IMPORTANT: Reload images after upload to get real IDs
            if (uploadedNewImage)
            {
                await ImageVM.LoadImages(CurrentPhoneId.Value);
            }
            // ======================
            // 2️⃣ Update index
            // ======================

            var selectedIndex = ImageVM.Images
                .FirstOrDefault(i => i.IsIndex && i.Id.HasValue);

            if (selectedIndex == null)
            {
                MessageBox.Show($"Please select a valid index image!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string url1 =
                $"http://localhost:5175/api/blob/SetIndex/{CurrentPhoneId}/{selectedIndex.Id.Value}";


            var response1 = await client.PutAsync(url1, null);

            if (!response1.IsSuccessStatusCode)
            {
                var error = await response1.Content.ReadAsStringAsync();
                MessageBox.Show($"An error occured while saving the images!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                MessageBox.Show($"Images saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

    }
}
