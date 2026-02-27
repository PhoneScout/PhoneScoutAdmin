using PhoneScoutAdmin;
using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.ViewModels;
using PhoneScoutAdmin.Views;
using System.ComponentModel;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _httpClient = new HttpClient();

    public event PropertyChangedEventHandler PropertyChanged;

    private string _email;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(nameof(Email)); }
    }

    private string _message;
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(nameof(Message)); }
    }

    private Brush _messageColor = Brushes.Red;
    public Brush MessageColor
    {
        get => _messageColor;
        set { _messageColor = value; OnPropertyChanged(nameof(MessageColor)); }
    }

    public AsyncRelayCommand<object> LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand<object>(Login);
    }

    private async Task Login(object param)
    {
        MessageBox.Show("login");

        var passwordBox = param as PasswordBox;
        string password = passwordBox?.Password;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
        {
            MessageColor = Brushes.Red;
            Message = "Hiba: Minden mezőt töltsön ki!";
            return;
        }

        try
        {
            // ✅ Get user info
            var userInfoResponse = await _httpClient.GetAsync(
                $"http://localhost:5175/api/Login/GetUserInfo/{Uri.EscapeDataString(Email)}");

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                MessageColor = Brushes.Red;
                Message = await userInfoResponse.Content.ReadAsStringAsync();
                return;
            }

            var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfo>(userInfoJson);

            MessageBox.Show($"NAme={userInfo.name}, Privilege={userInfo.privilege}, Active={userInfo.active}");

            if (userInfo == null)
            {
                MessageColor = Brushes.Red;
                Message = "Hiba: Nem sikerült lekérni a felhasználót!";
                return;
            }

            // Manufacturer first login -> force password change
            if (userInfo.privilege == 6 && userInfo.active == 0)
            {
                MessageBox.Show("firstlogin");
                OpenFirstPasswordChange(userInfo.email);
                CloseLoginWindow();
                return;
            }

            // Normal login
            var saltResponse = await _httpClient.GetAsync(
                $"http://localhost:5175/api/Login/GetSalt/{Uri.EscapeDataString(Email)}");

            if (!saltResponse.IsSuccessStatusCode)
                throw new Exception("A felhasználó nem található.");

            var salt = (await saltResponse.Content.ReadAsStringAsync()).Trim('"');
            string combined = password + salt;
            string tmpHash = ComputeSHA256(combined);

            var loginData = new { Email = Email, TmpHash = tmpHash };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5175/api/Login", content);

            if (!response.IsSuccessStatusCode)
            {
                MessageColor = Brushes.Red;
                Message = await response.Content.ReadAsStringAsync();
                return;
            }
            

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(resultJson);

            

            Application.Current.Properties["Token"] = result.Token;
            Application.Current.Properties["FullName"] = result.FullName;
            Application.Current.Properties["Email"] = result.Email;


            if (userInfo.privilege == 7) // Admin
            {
                MessageBox.Show("adminlogin");

                OpenAdminWindow();
                CloseLoginWindow();
            }
            else if (userInfo.privilege == 6) // Manufacturer
            {
                MessageBox.Show("manulogin"+userInfo.name);



                OpenManufacturerHome(userInfo.name);
                CloseLoginWindow();
            }

        }
        catch (Exception ex)
        {
            MessageColor = Brushes.Red;
            Message = "Hiba: " + ex.Message;
        }
    }

    private string ComputeSHA256(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    private void CloseLoginWindow()
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window.DataContext == this)
            {
                window.Close();
                break;
            }
        }
    }

    private void OpenManufacturerHome(string loggedInManufacturer)
    {
        var window = new Window
        {
            Content = new ManufacturerHome
            {
                DataContext = new SingleManufacturerViewModel(loggedInManufacturer)
            }
        };

        window.Show();
    }

    private void OpenAdminWindow()
    {
        var window = new Home();
        window.DataContext = new MainViewModel();
        window.Show();
    }

    private void OpenFirstPasswordChange(string email)
    {
        var window = new Window
        {
            Width = 275,
            Height = 580,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            Content = new FirstLoginPasswordChangeView()
        };

        // Set DataContext of the hosting window
        window.DataContext = new FirstLoginPasswordChangeViewModel(email);

        window.Show();
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}