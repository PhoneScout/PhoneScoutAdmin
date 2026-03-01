using Microsoft.Win32;
using PhoneScoutAdmin.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace PhoneScoutAdmin.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<PhoneImage> Images { get; } = new ObservableCollection<PhoneImage>();

        public int? CurrentPhoneId { get; set; }

        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand SetIndexCommand { get; }

        public ImageViewModel()
        {
            AddImageCommand = new RelayCommand(AddImage);
            RemoveImageCommand = new RelayCommand<PhoneImage>(RemoveImage);
            SetIndexCommand = new RelayCommand<PhoneImage>(SetAsIndex);
        }

        private void SetAsIndex(PhoneImage selectedImage)
        {
            if (selectedImage == null)
                return;

            foreach (var img in Images)
                img.IsIndex = false;

            selectedImage.IsIndex = true;
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
                    bitmap.DecodePixelWidth = 300;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    Images.Add(new PhoneImage
                    {
                        FileName = Path.GetFileName(file),
                        ImageData = imageBytes,
                        Preview = bitmap,
                        IsIndex = false,
                        IsNew = true
                    });
                }

            }
        }

        public async Task LoadImages(int phoneId)
        {
            Images.Clear();

            using HttpClient client = new HttpClient();

            string url = $"http://localhost:5175/api/blob/GetAllPictures/{phoneId}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync();

            var pictures = JsonSerializer.Deserialize<List<BackendPictureDto>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (pictures == null)
                return;

            foreach (var pic in pictures)
            {
                var imageBytes = await client.GetByteArrayAsync(
                    $"http://localhost:5175/api/blob/GetPictureById/{pic.Id}");

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.EndInit();
                bitmap.Freeze();

                Images.Add(new PhoneImage
                {
                    Id = pic.Id,
                    FileName = $"image_{pic.Id}",
                    ImageData = imageBytes,
                    Preview = bitmap,
                    IsIndex = pic.IsIndex == 1,
                    IsNew = false
                });
            }


        }

        private async void RemoveImage(PhoneImage image)
        {
            if (image == null)
                return;

            if (!image.IsNew && image.Id != null)
            {
                var result = MessageBox.Show(
                $"Are you sure you want to delete the selected image?",
                "Delete image",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using HttpClient client = new HttpClient();
                    string url = $"http://localhost:5175/api/blob/DeletePicture/{image.Id}";

                    var response = await client.DeleteAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Failed to delete image from database.");
                        return;
                    }
                }


                
            }

            Images.Remove(image);

        }


    }

    // DTO for backend response
    public class BackendPictureDto
    {
        public int Id { get; set; }
        public int IsIndex { get; set; }
    }
}