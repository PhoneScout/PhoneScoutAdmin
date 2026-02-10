using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
using PhoneScoutAdmin.ViewModels;

namespace PhoneScoutAdmin
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>

    
    



    
    public class ComboItemUsers
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }

        public override string? ToString()
        {
            return $"{Level} - {Name}";
        }
    }
    public class ComboItemOrderStorage
    {
        public int Id { get; set; }
        public int statusCode { get; set; }
        public string Name { get; set; }

        public override string? ToString()
        {
            return $"{Name}";
        }   
    }

    public partial class Home : Window
    {
        
        public Home()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        
        
    }
}
