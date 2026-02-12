using System.ComponentModel;

namespace PhoneScoutAdmin.ViewModels
{
    public class CPUdetailsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _cpuName;
        public string CpuName
        {
            get => _cpuName;
            set
            {
                _cpuName = value;
                OnPropertyChanged(nameof(CpuName));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _antutuScore;
        public string AntutuScore
        {
            get => _antutuScore;
            set
            {
                _antutuScore = value;
                OnPropertyChanged(nameof(AntutuScore));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _clockSpeed;
        public string ClockSpeed
        {
            get => _clockSpeed;
            set
            {
                _clockSpeed = value;
                OnPropertyChanged(nameof(ClockSpeed));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _coreNumber;
        public string CoreNumber
        {
            get => _coreNumber;
            set
            {
                _coreNumber = value;
                OnPropertyChanged(nameof(CoreNumber));
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _manufacturingTech;
        public string ManufacturingTech
        {
            get => _manufacturingTech;
            set
            {
                _manufacturingTech = value;
                OnPropertyChanged(nameof(ManufacturingTech));
                OnPropertyChanged(nameof(Progress));
            }
        }

        public double Progress
        {
            get
            {
                int filled = 0;

                if (!string.IsNullOrWhiteSpace(CpuName)) filled++;
                if (!string.IsNullOrWhiteSpace(AntutuScore)) filled++;
                if (!string.IsNullOrWhiteSpace(ClockSpeed)) filled++;
                if (!string.IsNullOrWhiteSpace(CoreNumber)) filled++;
                if (!string.IsNullOrWhiteSpace(ManufacturingTech)) filled++;

                return filled / 8.0;
            }
        }
    }
}
