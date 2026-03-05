using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
                OnPropertyChanged(nameof(ProgressBorder));

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
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        private DateOnly _releaseDate;
        public DateOnly ReleaseDate
        {
            get => _releaseDate;
            set
            {
                _releaseDate = value;
                OnPropertyChanged(nameof(ReleaseDate));
                OnPropertyChanged(nameof(ReleaseDateForBinding));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(ProgressBorder));

            }
        }

        public DateTime? ReleaseDateForBinding
        {
            get => _releaseDate.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (value.HasValue)
                    ReleaseDate = DateOnly.FromDateTime(value.Value);
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

                if (!string.IsNullOrWhiteSpace(PhoneName)) filled++;                
                if (!string.IsNullOrWhiteSpace(ManufacturerName)) filled++;
                if (!string.IsNullOrWhiteSpace(PhoneWeight)) filled++;


                return filled / 3.0;
            }
        }

        public string PhoneNameDisplay => (PhoneName != "") ? $"Editing: {PhoneName}" : "Creating a new phone";

    }
}
