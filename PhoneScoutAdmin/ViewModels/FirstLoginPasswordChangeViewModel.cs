using PhoneScoutAdmin;
using PhoneScoutAdmin.Views;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public class FirstLoginPasswordChangeViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _email;

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private string _currentPassword;
    public string CurrentPassword
    {
        get => _currentPassword;
        set { _currentPassword = value; OnPropertyChanged(nameof(CurrentPassword)); }
    }

    private string _newPassword;
    public string NewPassword
    {
        get => _newPassword;
        set { _newPassword = value; OnPropertyChanged(nameof(NewPassword)); }
    }

    private string _confirmPassword;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); }
    }

    private string _statusMessage;
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
    }

    private bool _isError;
    public bool IsError
    {
        get => _isError;
        set { _isError = value; OnPropertyChanged(nameof(IsError)); }
    }

    public ICommand ChangePasswordCommand { get; }

    public FirstLoginPasswordChangeViewModel(string email)
    {
        _email = email;
        ChangePasswordCommand = new AsyncRelayCommand<object>(async _ => await ChangePassword());
    }

    private async Task ChangePassword()
    {
        MessageBox.Show(CurrentPassword);
        MessageBox.Show(NewPassword);
        MessageBox.Show(ConfirmPassword);

        StatusMessage = "";
        IsError = false;

        // 1️⃣ Validate new password confirmation
        if (NewPassword != ConfirmPassword)
        {
            StatusMessage = "Az új jelszó és a megerősítés nem egyezik!";
            IsError = true;
            return;
        }

        // 2️⃣ Validate password strength
        if (!IsStrongPassword(NewPassword))
        {
            StatusMessage = "A jelszó nem elég erős! Használjon kisbetűt, nagybetűt, számot és speciális karaktert!";
            IsError = true;
            return;
        }

        try
        {
            // 3️⃣ Get the current salt from the backend
            var saltResponse = await _httpClient.GetAsync($"http://localhost:5175/api/Login/GetSalt/{Uri.EscapeDataString(_email.Trim())}");
            if (!saltResponse.IsSuccessStatusCode) throw new Exception("Nem sikerült lekérni a saltot.");

            string currentDbSalt = (await saltResponse.Content.ReadAsStringAsync()).Trim('"');

            // 4️⃣ Hash the old password with the DB salt
            // Hash old password like React does
            string oldHashed = ComputeSHA256(CurrentPassword + currentDbSalt);

            // Generate new salt
            string newSalt = Guid.NewGuid().ToString();

            // Hash new password
            string newHashed = ComputeSHA256(NewPassword + newSalt);

            var dto = new
            {
                Email = _email.Trim().ToLower(),
                OldPassword = oldHashed,   // ✅ HASHED
                NewPassword = newHashed,   // ✅ HASHED
                Salt = newSalt
            };

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("http://localhost:5175/api/Registration/ChangePassword", content);


            if (response.IsSuccessStatusCode)
            {
                await _httpClient.PutAsync(
                    $"http://localhost:5175/api/Registration/ActivateAccountWPF?email={Uri.EscapeDataString(_email)}",
                    null
                );
               
                StatusMessage = "Sikeres jelszó módosítás!";

                // Open LoginView (Window) with fixed size
                var loginWindow = new LoginView
                {
                    Width = 275,
                    Height = 580,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };

                loginWindow.Show();

                // Close the window hosting this UserControl
                foreach (Window window in Application.Current.Windows)
                {
                    // Check if the window's DataContext is THIS ViewModel
                    if (window.DataContext == this)
                    {
                        window.Close(); // ✅ closes the window hosting this UserControl
                        break;
                    }
                }

               

            }
            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = await response.Content.ReadAsStringAsync();
                StatusMessage = "Hiba: " + errorMsg;
                IsError = true;
                return;
            }

            // ✅ Clear fields and notify success
            StatusMessage = "Sikeres jelszó módosítás!";
            IsError = false;

            CurrentPassword = "";
            NewPassword = "";
            ConfirmPassword = "";
        }
        catch (Exception ex)
        {
            StatusMessage = "Hiba: " + ex.Message;
            IsError = true;
        }
    }

    private string ComputeSHA256(string input)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    private bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else hasSpecial = true;
        }

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}