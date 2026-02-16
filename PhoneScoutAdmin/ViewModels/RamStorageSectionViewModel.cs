using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class RamStorageSectionViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public ObservableCollection<RamStorageViewModel> RamStorages { get; }
            = new ObservableCollection<RamStorageViewModel>();

        private RamStorageViewModel _selectedRamStorage;
        public RamStorageViewModel SelectedRamStorage
        {
            get => _selectedRamStorage;
            set
            {
                _selectedRamStorage = value;
                OnPropertyChanged(nameof(SelectedRamStorage));

                // THIS is what you were missing
                (RemoveRamStorageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }


        public ICommand AddRamStorageCommand { get; }
        public ICommand RemoveRamStorageCommand { get; }

        public RamStorageSectionViewModel()
        {
            AddRamStorageCommand = new RelayCommand(AddRamStorage);
            RemoveRamStorageCommand = new RelayCommand(RemoveRamStorage, () => SelectedRamStorage != null);
        }

        private void AddRamStorage()
        {
            var cam = new RamStorageViewModel
            {
                RamAmount = SelectedRamStorage.RamAmount
            };

            RamStorages.Add(cam);
            SelectedRamStorage = cam;
        }

        private void RemoveRamStorage()
        {
            if (SelectedRamStorage != null)
                RamStorages.Remove(SelectedRamStorage);
        }


    }
}
