using System;
using System.ComponentModel;
using System.Windows.Media;

namespace PhoneScoutAdmin.ViewModels
{
    public class GeneralInfosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
                OnPropertyChanged(name);
        }

        // ---------------------- Properties ----------------------

        private string _phoneName;
        public string PhoneName
        {
            get => _phoneName;
            set
            {
                _phoneName = value;
                NotifyPropertyChanged(nameof(PhoneName), nameof(Progress), nameof(ProgressBorder), nameof(PhoneNameDisplay));
            }
        }

        private string _manufacturerName;
        public string ManufacturerName
        {
            get => _manufacturerName;
            set
            {
                _manufacturerName = value;
                NotifyPropertyChanged(nameof(ManufacturerName), nameof(Progress), nameof(ProgressBorder));
            }
        }

        private DateOnly _releaseDate;
        public DateOnly ReleaseDate
        {
            get => _releaseDate;
            set
            {
                _releaseDate = value;
                NotifyPropertyChanged(nameof(ReleaseDate), nameof(ReleaseDateForBinding), nameof(Progress), nameof(ProgressBorder));
            }
        }

        public DateTime? ReleaseDateForBinding
        {
            get => _releaseDate.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (value.HasValue)
                {
                    ReleaseDate = DateOnly.FromDateTime(value.Value);
                    OnPropertyChanged(nameof(ReleaseDateForBinding));
                }
            }
        }

        private string _phoneWeight;
        public string PhoneWeight
        {
            get => _phoneWeight;
            set
            {
                _phoneWeight = value;
                NotifyPropertyChanged(nameof(PhoneWeight), nameof(Progress), nameof(ProgressBorder));
            }
        }

        // ---------------------- Computed Properties ----------------------

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

        public Brush ProgressBorder
        {
            get
            {
                if (Progress == 1)
                    return Brushes.Green;
                if (Progress >= 0.7)
                    return Brushes.Goldenrod; // better contrast than Yellow
                if (Progress >= 0.5)
                    return Brushes.Orange;
                if (Progress >= 0.2)
                    return Brushes.DarkOrange;
                return Brushes.DarkGray;
            }
        }

        public string PhoneNameDisplay => !string.IsNullOrWhiteSpace(PhoneName)
            ? $"Editing: {PhoneName}"
            : "Creating a new phone";
    }
}