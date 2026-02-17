using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.ViewModels
{
    public class GeneralInfosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _phoneName;
        public string PhoneName
        {
            get => _phoneName;
            set
            {
                _phoneName = value;
                OnPropertyChanged(nameof(PhoneName));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _phonePrice;
        public string PhonePrice
        {
            get => _phonePrice;
            set
            {
                _phonePrice = value;
                OnPropertyChanged(nameof(PhonePrice));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _manufacturerName;
        public string ManufacturerName
        {
            get => _manufacturerName;
            set
            {
                _manufacturerName = value;
                OnPropertyChanged(nameof(ManufacturerName));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _releaseDate;
        public string ReleaseDate
        {
            get => _releaseDate;
            set
            {
                _releaseDate = value;
                OnPropertyChanged(nameof(ReleaseDate));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _phoneWeight;
        public string PhoneWeight
        {
            get => _phoneWeight;
            set
            {
                _phoneWeight = value;
                OnPropertyChanged(nameof(PhoneWeight));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private bool _phoneInStore;
        public bool PhoneInStore
        {
            get => _phoneInStore;
            set
            {
                _phoneInStore = value;
                OnPropertyChanged(nameof(PhoneInStore));
                OnPropertyChanged(nameof(Progress));
            }
        }

        public double Progress
        {
            get
            {
                int filled = 0;

                if (!string.IsNullOrWhiteSpace(PhoneName)) filled++;
                if (!string.IsNullOrWhiteSpace(PhonePrice)) filled++;
                if (!string.IsNullOrWhiteSpace(ManufacturerName)) filled++;
                if (!string.IsNullOrWhiteSpace(ReleaseDate)) filled++;
                if (!string.IsNullOrWhiteSpace(PhoneWeight)) filled++;
                //if (!string.IsNullOrWhiteSpace(PhoneInStore)) filled++;

                return filled / 8.0;
            }
        }
    }
}
