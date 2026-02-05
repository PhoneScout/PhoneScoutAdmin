using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // DATA
        // ======================

        public ObservableCollection<User> Users { get; }
            = new ObservableCollection<User>();

        public ObservableCollection<ComboItemUsers> Items { get; }
            = new ObservableCollection<ComboItemUsers>
            {
                new ComboItemUsers { Id = 1, Level = 1, Name = "User" },
                new ComboItemUsers { Id = 2, Level = 2, Name = "Manufacturer" },
                new ComboItemUsers { Id = 3, Level = 3, Name = "Admin" },
            };

        // ======================
        // SELECTION
        // ======================

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                LoadSelectedUserIntoFields();
                RaiseCommandStates();
            }
        }

        // ======================
        // EDIT FIELDS
        // ======================

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        private ComboItemUsers _selectedPrivilege;
        public ComboItemUsers SelectedPrivilege
        {
            get => _selectedPrivilege;
            set { _selectedPrivilege = value; OnPropertyChanged(nameof(SelectedPrivilege)); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(nameof(IsActive)); }
        }

        // ======================
        // COMMANDS
        // ======================

        public ICommand LoadUsersCommand { get; }
        public ICommand SaveUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public UserViewModel()
        {
            LoadUsersCommand = new RelayCommand(async () => await LoadUsers());
            SaveUserCommand = new RelayCommand(async () => await SaveUser(), () => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(async () => await DeleteUser(), () => SelectedUser != null);
        }

        private void RaiseCommandStates()
        {
            (SaveUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ======================
        // LOGIC
        // ======================

        private void LoadSelectedUserIntoFields()
        {
            if (SelectedUser == null)
            {
                FullName = "";
                Email = "";
                SelectedPrivilege = null;
                IsActive = false;
                return;
            }

            FullName = SelectedUser.fullName;
            Email = SelectedUser.email;
            IsActive = SelectedUser.isActive == 1;

            SelectedPrivilege = Items
                .FirstOrDefault(i => i.Level == SelectedUser.privilegeLevel);
        }

        private async Task LoadUsers()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/wpfUsers";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var userList = JsonSerializer.Deserialize<List<User>>(json);

            Users.Clear();
            foreach (var user in userList)
                Users.Add(user);
        }

        private async Task SaveUser()
        {
            if (SelectedUser == null) return;

            SelectedUser.fullName = FullName;
            SelectedUser.email = Email;
            SelectedUser.privilegeLevel = SelectedPrivilege.Level;
            SelectedUser.privilegeName = SelectedPrivilege.Name;
            SelectedUser.isActive = IsActive ? 1 : 0;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/wpfUsers/{SelectedUser.userID}";

            string json = JsonSerializer.Serialize(SelectedUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PutAsync(url, content);
        }

        private async Task DeleteUser()
        {
            if (SelectedUser == null) return;

            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/wpfUsers/{SelectedUser.userID}";

            await client.DeleteAsync(url);
            Users.Remove(SelectedUser);
            SelectedUser = null;
        }
    }
}
