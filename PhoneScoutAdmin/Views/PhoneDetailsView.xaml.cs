using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PhoneScoutAdmin
{
    public partial class PhoneDetailsView : Window
    {
        public PhoneDetailsView()
        {
            InitializeComponent();
            DataContext = new PhoneDetailsViewModel();
        }
    }
}
