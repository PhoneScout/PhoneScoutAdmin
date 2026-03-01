using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PhoneScoutAdmin.ViewModels
{
    public class SensorsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _finSenPlace;
        public string FinSenPlace
        {
            get => _finSenPlace;
            set
            {
                _finSenPlace = value;
                OnPropertyChanged(nameof(FinSenPlace));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        private string _finSenType;
        public string FinSenType
        {
            get => _finSenType;
            set
            {
                _finSenType = value;
                OnPropertyChanged(nameof(FinSenType));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        private bool _infrared;
        public bool Infrared
        {
            get => _infrared;
            set
            {
                _infrared = value;
                OnPropertyChanged(nameof(Infrared));
                OnPropertyChanged(nameof(Progress));
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

                if (!string.IsNullOrWhiteSpace(FinSenPlace)) filled++;
                if (!string.IsNullOrWhiteSpace(FinSenType)) filled++;


                return filled / 2.0;
            }
        }
    }
}
