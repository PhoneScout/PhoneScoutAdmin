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

       
    }
}
