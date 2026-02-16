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
    public class CameraSectionViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public ObservableCollection<CameraViewModel> Cameras { get; }
            = new ObservableCollection<CameraViewModel>();

        private CameraViewModel _selectedCamera;
        public CameraViewModel SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                _selectedCamera = value;
                OnPropertyChanged(nameof(SelectedCamera));

                // THIS is what you were missing
                (RemoveCameraCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }


        public ICommand AddCameraCommand { get; }
        public ICommand RemoveCameraCommand { get; }

        public CameraSectionViewModel()
        {
            AddCameraCommand = new RelayCommand(AddCamera);
            RemoveCameraCommand = new RelayCommand(RemoveCamera, () => SelectedCamera != null);
        }

        private void AddCamera()
        {
            var cam = new CameraViewModel
            {
                CameraName = $"Camera {Cameras.Count + 1}"
            };

            Cameras.Add(cam);
            SelectedCamera = cam;
        }

        private void RemoveCamera()
        {
            if (SelectedCamera != null)
                Cameras.Remove(SelectedCamera);
        }

        
    }
}
