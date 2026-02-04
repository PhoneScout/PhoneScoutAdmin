using PhoneScoutAdmin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        // DATA
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        // SELECTION

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

        //  EDIT FIELDS

        private string _userFullName;
        public string UserFullName
        {
            get => _userFullName;
            set { _userFullName = value; OnPropertyChanged(nameof(UserFullName)); }
        }

        private string _userEmail;
        public string UserEmail
        {
            get => _userEmail;
            set { _userEmail = value; OnPropertyChanged(nameof(UserEmail)); }
        }

        private int _userPrivilegeLevel;
        public int UserPrivilegeLevel
        {
            get => _userPrivilegeLevel;
            set { _userPrivilegeLevel = value; OnPropertyChanged(nameof(UserPrivilegeLevel)); }
        }

        private string _userPrivilegeName;
        public string UserPrivilegeName
        {
            get => _userPrivilegeName;
            set { _userPrivilegeName = value; OnPropertyChanged(nameof(UserPrivilegeName)); }
        }

        private int _userActive;
        public int UserActive
        {
            get => _userActive;
            set { _userActive = value; OnPropertyChanged(nameof(UserActive)); }
        }

        //  COMMANDS

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




        //  LOGIC

        private void LoadSelectedUserIntoFields()
        {
            if (SelectedUser == null)
            {
                UserFullName = "";
                UserEmail = "";
                UserPrivilegeLevel = 0;
                UserPrivilegeName = "";
                UserActive = 0;
                return;
            }

            UserFullName = SelectedUser.fullName;
            UserEmail = SelectedUser.email;
            UserPrivilegeLevel = SelectedUser.privilegeLevel;
            UserPrivilegeName = SelectedUser.privilegeName;
            UserActive = SelectedUser.isActive;
        }

        private async Task LoadUsers()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/wpfUsers";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var phoneList = JsonSerializer.Deserialize<List<User>>(json);

            Users.Clear();
            foreach (var phone in phoneList)
                Users.Add(phone);
        }

        private async Task SaveUser()
        {
            if (SelectedUser == null) return;

            SelectedUser.fullName = UserFullName;
            SelectedUser.email = UserEmail;
            SelectedUser.privilegeLevel = UserPrivilegeLevel;
            SelectedUser.privilegeName = UserPrivilegeName;
            SelectedUser.isActive = UserActive;

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
            string url = $"http://localhost:5175/api/wpfPhone/{SelectedUser.userID}";

            await client.DeleteAsync(url);
            Users.Remove(SelectedUser);
            SelectedUser = null;
        }

    }
}
