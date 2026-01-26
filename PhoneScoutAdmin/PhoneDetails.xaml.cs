using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PhoneScoutAdmin
{
    public partial class PhoneDetails : Window
    {
        // Array to store 4 images
        private BitmapImage[] Images = new BitmapImage[4];
        public string selectedMenu = "generalInfos";



        public PhoneDetails(int id)
        {
            InitializeComponent();
        }

        public PhoneDetails()
        {
            InitializeComponent();
        }

        // Select an image
        private BitmapImage SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            return null;
        }

        // Upload image for a slot
        private void UploadImage(int index)
        {
            var image = SelectImage();
            if (image != null)
            {
                Images[index] = image;
                UpdateImageControls();
            }
        }

        // Delete image from a slot
        private void DeleteImage(int index)
        {
            Images[index] = null;
            UpdateImageControls();
        }

        // Refresh UI
        private void UpdateImageControls()
        {
            ImageControl1.Source = Images[0];
            ImageControl2.Source = Images[1];
            ImageControl3.Source = Images[2];
            ImageControl4.Source = Images[3];
        }

        // Button click handlers
        private void UploadImage1_Click(object sender, RoutedEventArgs e) => UploadImage(0);
        private void UploadImage2_Click(object sender, RoutedEventArgs e) => UploadImage(1);
        private void UploadImage3_Click(object sender, RoutedEventArgs e) => UploadImage(2);
        private void UploadImage4_Click(object sender, RoutedEventArgs e) => UploadImage(3);

        private void DeleteImage1_Click(object sender, RoutedEventArgs e) => DeleteImage(0);
        private void DeleteImage2_Click(object sender, RoutedEventArgs e) => DeleteImage(1);
        private void DeleteImage3_Click(object sender, RoutedEventArgs e) => DeleteImage(2);
        private void DeleteImage4_Click(object sender, RoutedEventArgs e) => DeleteImage(3);

        private void showGenInfos(object sender, RoutedEventArgs e)
        {
            selectedMenu = "generalInfos";
            generalInfos.Visibility = Visibility.Visible;
        }

        private void showCPU(object sender, RoutedEventArgs e)
        {
            generalInfos.Visibility = Visibility.Collapsed;
        }
    }
}
