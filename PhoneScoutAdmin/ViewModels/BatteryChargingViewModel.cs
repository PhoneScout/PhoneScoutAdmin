using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PhoneScoutAdmin.ViewModels
{
    public class BatteryChargingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _batteryCapacity;
        public string BatteryCapacity
        {
            get => _batteryCapacity;
            set
            {
                _batteryCapacity = value;
                OnPropertyChanged(nameof(BatteryCapacity));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressBorder));

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
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        private string _maxWiredCharging;
        public string MaxWiredCharging
        {
            get => _maxWiredCharging;
            set
            {
                _maxWiredCharging = value;
                OnPropertyChanged(nameof(MaxWiredCharging));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        private string _maxWirelessCharging;
        public string MaxWirelessCharging
        {
            get => _maxWirelessCharging;
            set
            {
                _maxWirelessCharging = value;
                OnPropertyChanged(nameof(MaxWirelessCharging));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressBorder));

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
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        public Brush ProgressBorder
        {
            get
            {
                if (Progress == 1)
                    return Brushes.Green;

                if (Progress >= 0.7)
                    return Brushes.Yellow;

                if (Progress >= 0.5)
                    return Brushes.Orange;

                if (Progress >= 0.2)
                    return Brushes.DarkOrange;

                return Brushes.DarkGray;
            }
        }

        public double Progress
        {
            get
            {
                int filled = 0;

                if (!string.IsNullOrWhiteSpace(BatteryCapacity)) filled++;
                if (!string.IsNullOrWhiteSpace(BatteryType)) filled++;
                if (!string.IsNullOrWhiteSpace(MaxWiredCharging)) filled++;
                if (!string.IsNullOrWhiteSpace(MaxWirelessCharging)) filled++;
                if (!string.IsNullOrWhiteSpace(ChargerType)) filled++;

                return filled / 5.0;
            }
        }
    }
}
