using Microsoft.Win32;
using PhoneScoutAdmin.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhoneScoutAdmin.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<PhoneImage> Images { get; } = new ObservableCollection<PhoneImage>();

        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }

        public ImageViewModel()
        {
            AddImageCommand = new RelayCommand(AddImage);
            RemoveImageCommand = new RelayCommand<PhoneImage>(RemoveImage);
        }

        private void AddImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    byte[] imageBytes = File.ReadAllBytes(file);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new MemoryStream(imageBytes);
                    bitmap.DecodePixelWidth = 300; // prevent memory issues
                    bitmap.EndInit();
                    bitmap.Freeze();

                    Images.Add(new PhoneImage
                    {
                        FileName = Path.GetFileName(file),
                        ImageData = imageBytes,
                        Preview = bitmap,
                        IsIndex = Images.Count == 0 // first image as default index
                    });
                }

                OnPropertyChanged(nameof(Progress));
            }
        }

        private void RemoveImage(PhoneImage image)
        {
            if (image == null) return;
            Images.Remove(image);
            OnPropertyChanged(nameof(Progress));
        }

        public double Progress => Images.Count > 0 ? 1 : 0;
    }
}