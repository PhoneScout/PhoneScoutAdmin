using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
        }

        private string _infrared;
        public string Infrared
        {
            get => _infrared;
            set
            {
                _infrared = value;
                OnPropertyChanged(nameof(Infrared));
                OnPropertyChanged(nameof(Progress));
            }
        }

       

        public double Progress
        {
            get
            {
                int filled = 0;

                if (!string.IsNullOrWhiteSpace(FinSenPlace)) filled++;
                if (!string.IsNullOrWhiteSpace(FinSenType)) filled++;
                if (!string.IsNullOrWhiteSpace(Infrared)) filled++;

                return filled / 8.0;
            }
        }
    }
}
