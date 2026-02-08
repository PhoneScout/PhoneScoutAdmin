using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhoneScoutAdmin
{
    /// <summary>
    /// Interaction logic for ManufacturerHome.xaml
    /// </summary>
    public partial class ManufacturerHome : Window
    {
        public ManufacturerHome()
        {
            InitializeComponent();
            DataContext = new ManufacturerHomeViewModel();

        }

    }
}
