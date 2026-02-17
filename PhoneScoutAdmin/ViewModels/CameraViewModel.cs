using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneScoutAdmin.ViewModels
{
    public class CameraViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        private string _cameraName;
        public string CameraName
        {
            get => _cameraName;
            set
            {
                _cameraName = value;
                OnPropertyChanged(nameof(CameraName));
            }
        }

        private int _cameraResolution;
        public int CameraResolution
        {
            get => _cameraResolution;
            set
            {
                _cameraResolution = value;
                OnPropertyChanged(nameof(CameraResolution));
            }
        }

        private string _cameraAperture;
        public string CameraAperture
        {
            get => _cameraAperture;
            set
            {
                _cameraAperture = value;
                OnPropertyChanged(nameof(CameraAperture));
            }
        }

        private int _cameraFocalLength;
        public int CameraFocalLength
        {
            get => _cameraFocalLength;
            set
            {
                _cameraFocalLength = value;
                OnPropertyChanged(nameof(CameraFocalLength));
            }
        }

        private bool _cameraOIS;
        public bool CameraOIS
        {
            get => _cameraOIS;
            set
            {
                _cameraOIS = value;
                OnPropertyChanged(nameof(CameraOIS));
            }
        }

        private string _cameraType;
        public string CameraType
        {
            get => _cameraType;
            set
            {
                _cameraType = value;
                OnPropertyChanged(nameof(CameraType));
            }
        }
    }
}
