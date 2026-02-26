using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace PhoneScoutAdmin.Models
{
    public class PhoneImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int? Id { get; set; }

        public string FileName { get; set; }

        public byte[] ImageData { get; set; }

        public BitmapImage Preview { get; set; }

        private bool _isIndex;
        public bool IsIndex
        {
            get => _isIndex;
            set
            {
                if (_isIndex != value)
                {
                    _isIndex = value;
                    OnPropertyChanged(nameof(IsIndex));
                }
            }
        }

        public bool IsNew { get; set; }
    }
}