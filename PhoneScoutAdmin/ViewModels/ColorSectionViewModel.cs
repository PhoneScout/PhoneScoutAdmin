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
    public class ColorSectionViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public ObservableCollection<ColorViewModel> Colors { get; }
            = new ObservableCollection<ColorViewModel>();

        private ColorViewModel _selectedColor;
        public ColorViewModel SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));

                // THIS is what you were missing
                (RemoveColorCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }


        public ICommand AddColorCommand { get; }
        public ICommand RemoveColorCommand { get; }

        public ColorSectionViewModel()
        {
            AddColorCommand = new RelayCommand(AddCamera);
            RemoveColorCommand = new RelayCommand(RemoveCamera, () => SelectedColor != null);
        }

        private void AddCamera()
        {
            var cam = new ColorViewModel
            {
                ColorName = $"Camera {Colors.Count + 1}"
            };

            Colors.Add(cam);
            SelectedColor = cam;
        }

        private void RemoveCamera()
        {
            if (SelectedColor != null)
                Colors.Remove(SelectedColor);
        }


    }
}
