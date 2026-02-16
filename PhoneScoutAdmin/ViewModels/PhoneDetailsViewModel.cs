using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.ViewModels;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

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
            GeneralInfosVM.PhoneInStore = phone.phoneInStore;

            //CPU
            CpuVM.CpuName = phone.cpuName;
            CpuVM.AntutuScore = phone.phoneAntutu.ToString();
            CpuVM.ClockSpeed = phone.cpuClock.ToString();
            CpuVM.CoreNumber = phone.cpuCores.ToString();
            CpuVM.ManufacturingTech = phone.cpuTech.ToString();

            //SENSORS
            SensorsVM.FinSenPlace = phone.fingerprintPlace;
            SensorsVM.FinSenType = phone.fingerprintType;
            SensorsVM.Infrared = phone.sensorsInfrared;

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
            ConnectivityVM.DualSim = phone.connectionDualSim.ToString();
            ConnectivityVM.ESim = phone.connectionEsim.ToString();
            ConnectivityVM.Nfc = phone.connectionNfc.ToString();
            ConnectivityVM.Jack = phone.connectionJack.ToString();
            ConnectivityVM.ConnectionSpeed = phone.connectionConnectionSpeed.ToString();

            //BATTERY
            BatteryChargingVM.BatteryCapacity = phone.batteryCapacity.ToString();
            BatteryChargingVM.BatteryType = phone.batteryType;
            BatteryChargingVM.MaxWiredCharging = phone.batteryMaxChargingWired.ToString();
            BatteryChargingVM.MaxWirelessCharging = phone.batteryMaxChargingWireless.ToString();
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

            foreach (var color in phone.colors ?? new List<Color>())
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
                    CameraResolution = camera.cameraResolution.ToString(),
                    CameraAperture = camera.cameraAperture.ToString(),
                    CameraFocalLength = camera.cameraFocalLength,
                    CameraOIS = camera.cameraOis.ToString(),
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
                

                var newPhone = new FullPhone
                {
                    phoneName = GeneralInfosVM.PhoneName,
                    //phonePrice = price,
                    manufacturerName = GeneralInfosVM.ManufacturerName,
                    //phoneWeight = weight,
                    //phoneReleaseDate = releaseDate,
                    phoneInStore = GeneralInfosVM.PhoneInStore,

                    batteryCapacity = int.TryParse(BatteryChargingVM.BatteryCapacity, out var cap) ? cap : null,
                    batteryMaxChargingWired = int.TryParse(BatteryChargingVM.MaxWiredCharging, out var wired) ? wired : null,
                    batteryMaxChargingWireless = int.TryParse(BatteryChargingVM.MaxWirelessCharging, out var wireless) ? wireless : null,
                    batteryType = BatteryChargingVM.BatteryType,
                    chargerType = BatteryChargingVM.ChargerType,
                };

                using HttpClient client = new HttpClient();
                string url = "http://localhost:5175/api/phones";

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