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
            //GeneralInfosVM.PhoneAntutu = phone.phoneAntutu;
            //GeneralInfosVM.PhoneReleaseDate = phone.phoneReleaseDate;
            //GeneralInfosVM.PhonePrice = phone.phonePrice;

            // BATTERY
            BatteryChargingVM.BatteryCapacity = phone.batteryCapacity?.ToString();
            BatteryChargingVM.MaxWiredCharging = phone.batteryMaxChargingWired?.ToString();
            BatteryChargingVM.MaxWirelessCharging = phone.batteryMaxChargingWireless?.ToString();
            BatteryChargingVM.BatteryType = phone.batteryType;
            BatteryChargingVM.ChargerType = phone.chargerType;

            // CPU
            CpuVM.CpuName = phone.cpuName;
            /*CpuVM.CpuClock = phone.cpuClock;
            CpuVM.CpuCores = phone.cpuCores;
            CpuVM.CpuTech = phone.cpuTech;

            // SCREEN
            ScreenVM.ScreenSize = phone.screenSize;
            ScreenVM.ScreenRefreshRate = phone.screenRefreshRate;
            ScreenVM.ScreenMaxBrightness = phone.screenMaxBrightness;
            ScreenVM.ScreenSharpness = phone.screenSharpness;
            ScreenVM.ScreenType = phone.screenType;

            // CONNECTIVITY
            ConnectivityVM.MaxWifi = phone.connectionMaxWifi;
            ConnectivityVM.MaxBluetooth = phone.connectionMaxBluetooth;
            ConnectivityVM.MaxMobileNetwork = phone.connectionMaxMobileNetwork;
            ConnectivityVM.DualSim = phone.connectionDualSim;
            ConnectivityVM.Esim = phone.connectionEsim;
            ConnectivityVM.Nfc = phone.connectionNfc;
            ConnectivityVM.ConnectionSpeed = phone.connectionConnectionSpeed;
            ConnectivityVM.Jack = phone.connectionJack;

            // COLLECTIONS
            ColorSectionVM.Colors.Clear();
            foreach (var color in phone.colors ?? new List<Color>())
                ColorSectionVM.Colors.Add(color);

            CameraSectionVM.Cameras.Clear();
            foreach (var cam in phone.cameras ?? new List<Camera>())
                CameraSectionVM.Cameras.Add(cam);

            RamStorageSectionVM.RamStoragePairs.Clear();
            foreach (var pair in phone.ramStoragePairs ?? new List<RamStorage>())
                RamStorageSectionVM.RamStoragePairs.Add(pair);*/
        }


        private async Task CreatePhone()
        {
            var newPhone = new FullPhone
            {
                // GENERAL INFO
                phoneName = GeneralInfosVM.PhoneName,
                phonePrice = int.Parse(GeneralInfosVM.PhonePrice),
                manufacturerName = GeneralInfosVM.ManufacturerName,
                phoneWeight = decimal.Parse(GeneralInfosVM.PhoneWeight),
                phoneReleaseDate = DateOnly.Parse(GeneralInfosVM.ReleaseDate),
                phoneInStore = GeneralInfosVM.PhoneInStore,

                // BATTERY
                batteryCapacity = int.TryParse(BatteryChargingVM.BatteryCapacity, out var cap) ? cap : null,
                batteryMaxChargingWired = int.TryParse(BatteryChargingVM.MaxWiredCharging, out var wired) ? wired : null,
                batteryMaxChargingWireless = int.TryParse(BatteryChargingVM.MaxWirelessCharging, out var wireless) ? wireless : null,
                batteryType = BatteryChargingVM.BatteryType,
                chargerType = BatteryChargingVM.ChargerType,

                // CPU
                /*cpuName = CpuVM.CpuName,
                cpuClock = CpuVM.CpuClock,
                cpuCores = CpuVM.CpuCores,
                cpuTech = CpuVM.CpuTech,

                // SCREEN
                screenSize = ScreenVM.ScreenSize,
                screenRefreshRate = ScreenVM.ScreenRefreshRate,
                screenMaxBrightness = ScreenVM.ScreenMaxBrightness,
                screenSharpness = ScreenVM.ScreenSharpness,
                screenType = ScreenVM.ScreenType,

                // CONNECTIVITY
                connectionMaxWifi = ConnectivityVM.MaxWifi,
                connectionMaxBluetooth = ConnectivityVM.MaxBluetooth,
                connectionMaxMobileNetwork = ConnectivityVM.MaxMobileNetwork,
                connectionDualSim = ConnectivityVM.DualSim,
                connectionEsim = ConnectivityVM.Esim,
                connectionNfc = ConnectivityVM.Nfc,
                connectionConnectionSpeed = ConnectivityVM.ConnectionSpeed,
                connectionJack = ConnectivityVM.Jack,

                // COLLECTIONS
                colors = ColorSectionVM.Colors.ToList(),
                cameras = CameraSectionVM.Cameras.ToList(),
                ramStoragePairs = RamStorageSectionVM.RamStoragePairs.ToList()*/
            };

            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/phones";

            string json = JsonSerializer.Serialize(newPhone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // Optional success handling
            }
        }

    }
}