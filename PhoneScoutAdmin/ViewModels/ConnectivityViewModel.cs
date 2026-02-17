using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.ViewModels
{
    public class ConnectivityViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _wifi;
        public string Wifi
        {
            get => _wifi;
            set
            {
                _wifi = value;
                OnPropertyChanged(nameof(Wifi));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _bluetooth;
        public string Bluetooth
        {
            get => _bluetooth;
            set
            {
                _bluetooth = value;
                OnPropertyChanged(nameof(Bluetooth));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _mobileNetwork;
        public string MobileNetwork
        {
            get => _mobileNetwork;
            set
            {
                _mobileNetwork = value;
                OnPropertyChanged(nameof(MobileNetwork));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private bool _dualSim;
        public bool DualSim
        {
            get => _dualSim;
            set
            {
                _dualSim = value;
                OnPropertyChanged(nameof(DualSim));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private bool _eSim;
        public bool ESim
        {
            get => _eSim;
            set
            {
                _eSim = value;
                OnPropertyChanged(nameof(ESim));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private bool _nfc;
        public bool Nfc
        {
            get => _nfc;
            set
            {
                _nfc = value;
                OnPropertyChanged(nameof(Nfc));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private bool _jack;
        public bool Jack
        {
            get => _jack;
            set
            {
                _jack = value;
                OnPropertyChanged(nameof(Jack));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _connectionSpeed;
        public string ConnectionSpeed
        {
            get => _connectionSpeed;
            set
            {
                _connectionSpeed = value;
                OnPropertyChanged(nameof(ConnectionSpeed));
                OnPropertyChanged(nameof(Progress));
            }
        }        

        public double Progress
        {
            get
            {
                int filled = 0;

                if (!string.IsNullOrWhiteSpace(Wifi)) filled++;
                if (!string.IsNullOrWhiteSpace(Bluetooth)) filled++;
                if (!string.IsNullOrWhiteSpace(MobileNetwork)) filled++;
                if (!string.IsNullOrWhiteSpace(ConnectionSpeed)) filled++;                

                return filled / 8.0;
            }
        }
    }
}
