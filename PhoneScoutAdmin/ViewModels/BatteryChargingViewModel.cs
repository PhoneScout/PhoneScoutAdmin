using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.ViewModels
{
    public class BatteryChargingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _batteryCapacity;
        public int BatteryCapacity
        {
            get => _batteryCapacity;
            set
            {
                _batteryCapacity = value;
                OnPropertyChanged(nameof(BatteryCapacity));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _batteryType;
        public string BatteryType
        {
            get => _batteryType;
            set
            {
                _batteryType = value;
                OnPropertyChanged(nameof(BatteryType));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private int _maxWiredCharging;
        public int MaxWiredCharging
        {
            get => _maxWiredCharging;
            set
            {
                _maxWiredCharging = value;
                OnPropertyChanged(nameof(MaxWiredCharging));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private int _maxWirelessCharging;
        public int MaxWirelessCharging
        {
            get => _maxWirelessCharging;
            set
            {
                _maxWirelessCharging = value;
                OnPropertyChanged(nameof(MaxWirelessCharging));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _chargerType;
        public string ChargerType
        {
            get => _chargerType;
            set
            {
                _chargerType = value;
                OnPropertyChanged(nameof(ChargerType));
                OnPropertyChanged(nameof(Progress));
            }
        }

        public double Progress
        {
            get
            {
                int filled = 0;

                //if (!string.IsNullOrWhiteSpace(BatteryCapacity)) filled++;
                if (!string.IsNullOrWhiteSpace(BatteryType)) filled++;
                //if (!string.IsNullOrWhiteSpace(MaxWiredCharging)) filled++;
                //if (!string.IsNullOrWhiteSpace(MaxWirelessCharging)) filled++;
                if (!string.IsNullOrWhiteSpace(ChargerType)) filled++;

                return filled / 8.0;
            }
        }
    }
}
