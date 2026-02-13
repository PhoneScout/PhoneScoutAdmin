using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.ViewModels
{
    public class ScreenViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _screenType;
        public string ScreenType
        {
            get => _screenType;
            set
            {
                _screenType = value;
                OnPropertyChanged(nameof(ScreenType));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _screenResH;
        public string ScreenResH
        {
            get => _screenResH;
            set
            {
                _screenResH = value;
                OnPropertyChanged(nameof(ScreenResH));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _screenResW;
        public string ScreenResW
        {
            get => _screenResW;
            set
            {
                _screenResW = value;
                OnPropertyChanged(nameof(ScreenResW));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _screenSize;
        public string ScreenSize
        {
            get => _screenSize;
            set
            {
                _screenSize = value;
                OnPropertyChanged(nameof(ScreenSize));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _screenRefreshRate;
        public string ScreenRefreshRate
        {
            get => _screenRefreshRate;
            set
            {
                _screenRefreshRate = value;
                OnPropertyChanged(nameof(ScreenRefreshRate));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _screenMaxBrightness;
        public string ScreenMaxBrightness
        {
            get => _screenMaxBrightness;
            set
            {
                _screenMaxBrightness = value;
                OnPropertyChanged(nameof(ScreenMaxBrightness));
                OnPropertyChanged(nameof(Progress));
            }
        }

       

        public double Progress
        {
            get
            {
                int filled = 0;

                if (!string.IsNullOrWhiteSpace(ScreenType)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenResH)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenResW)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenSize)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenRefreshRate)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenMaxBrightness)) filled++;

                return filled / 8.0;
            }
        }
    }
}
