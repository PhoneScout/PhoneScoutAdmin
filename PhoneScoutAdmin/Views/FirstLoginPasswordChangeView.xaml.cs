using System.Windows;
using System.Windows.Controls;

namespace PhoneScoutAdmin.Views
{   public partial class FirstLoginPasswordChangeView : UserControl
    {
        public FirstLoginPasswordChangeView()
        {
            InitializeComponent();
        }

        public FirstLoginPasswordChangeView(string email)
        {
            InitializeComponent();
            DataContext = new FirstLoginPasswordChangeViewModel(email);
        }

        private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is FirstLoginPasswordChangeViewModel vm)
                vm.CurrentPassword = ((PasswordBox)sender).Password;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is FirstLoginPasswordChangeViewModel vm)
                vm.NewPassword = ((PasswordBox)sender).Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is FirstLoginPasswordChangeViewModel vm)
                vm.ConfirmPassword = ((PasswordBox)sender).Password;
        }
    }
}