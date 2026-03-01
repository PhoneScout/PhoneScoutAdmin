using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhoneScoutAdmin.Models
{
    public class Event : INotifyPropertyChanged
    {
        public int eventID { get; set; }
        public string eventHostName { get; set; }
        public string eventHostURL { get; set; }
        public string eventName { get; set; }
        public DateTime eventDate { get; set; }
        public string eventURL { get; set; }

        private string _imageBase64;
        public string imageBase64
        {
            get => _imageBase64;
            set
            {
                _imageBase64 = value;
                OnPropertyChanged(nameof(imageBase64));
                OnPropertyChanged(nameof(EventImage)); // notify UI
            }
        }

        public string contentType { get; set; }

        public BitmapImage EventImage
        {
            get
            {
                if (string.IsNullOrEmpty(imageBase64)) return null;

                try
                {
                    byte[] imageBytes = Convert.FromBase64String(imageBase64);
                    BitmapImage bitmap = new BitmapImage();
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}