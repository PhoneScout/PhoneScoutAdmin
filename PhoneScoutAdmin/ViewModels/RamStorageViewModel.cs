using System.ComponentModel;

namespace PhoneScoutAdmin.ViewModels
{
    public class RamStorageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _ramAmount;
        public int RamAmount
        {
            get => _ramAmount;
            set
            {
                _ramAmount = value;
                OnPropertyChanged(nameof(RamAmount));
                OnPropertyChanged(nameof(RamStorageDisplay));
            }
        }

        private int _storageAmount;
        public int StorageAmount
        {
            get => _storageAmount;
            set
            {
                _storageAmount = value;
                OnPropertyChanged(nameof(StorageAmount));
                OnPropertyChanged(nameof(RamStorageDisplay));
            }
        }

        public string RamStorageDisplay => $"{RamAmount}/{StorageAmount}";
    }
}