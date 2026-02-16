using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
        }

        private string _ramSpeed;
        public string RamSpeed
        {
            get => _ramSpeed;
            set
            {
                _ramSpeed = value;
                OnPropertyChanged(nameof(RamSpeed));
            }
        }

        private string _storageSpeed;
        public string StorageSpeed
        {
            get => _storageSpeed;
            set
            {
                _storageSpeed = value;
                OnPropertyChanged(nameof(_storageSpeed));
            }
        }


    }
}
