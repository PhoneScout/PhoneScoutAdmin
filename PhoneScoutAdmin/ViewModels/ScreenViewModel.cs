using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
                OnPropertyChanged(nameof(ProgressBorder));

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
                OnPropertyChanged(nameof(ProgressBorder));

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
                OnPropertyChanged(nameof(ProgressBorder));

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
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        private string _screenSharpness;
        public string ScreenSharpness
        {
            get => _screenSharpness;
            set
            {
                _screenSharpness = value;
                OnPropertyChanged(nameof(ScreenSharpness));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressBorder));

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
                OnPropertyChanged(nameof(ProgressBorder));

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

                if (!string.IsNullOrWhiteSpace(ScreenType)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenResH)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenResW)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenSharpness)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenSize)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenRefreshRate)) filled++;
                if (!string.IsNullOrWhiteSpace(ScreenMaxBrightness)) filled++;

                return filled / 7.0;
            }
        }
    }
}
