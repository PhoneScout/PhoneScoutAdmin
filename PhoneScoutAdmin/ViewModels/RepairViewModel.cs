using PhoneScoutAdmin.Commands;
using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class RepairViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ======================
        // DATA
        // ======================

        public ObservableCollection<Repair> Repairs { get; } = new();
        public ObservableCollection<ComboItemOrderRepair> Statuses { get; } = new();
        public ObservableCollection<string> Parts { get; } = new();

        // ======================
        // SELECTION
        // ======================

        private Repair _selectedRepair;
        public Repair SelectedRepair
        {
            get => _selectedRepair;
            set
            {
                _selectedRepair = value;
                OnPropertyChanged(nameof(SelectedRepair));
                LoadSelectedRepairIntoFields();
                RaiseCommandStates();
            }
        }

        // ======================
        // EDIT FIELDS
        // ======================

        private int _price;
        public int Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        private string _newPart;
        public string NewPart
        {
            get => _newPart;
            set { _newPart = value; OnPropertyChanged(nameof(NewPart)); }
        }

        private string _repairIdFilter;
        public string RepairIdFilter
        {
            get => _repairIdFilter;
            set
            {
                _repairIdFilter = value;
                OnPropertyChanged(nameof(RepairIdFilter));
                RepairsView.Refresh();
            }
        }

        private string _emailFilter;
        public string EmailFilter
        {
            get => _emailFilter;
            set
            {
                _emailFilter = value;
                OnPropertyChanged(nameof(EmailFilter));
                RepairsView.Refresh();
            }
        }

        // ======================
        // COMMANDS
        // ======================

        public ICommand LoadRepairsCommand { get; }
        public ICommand SaveRepairCommand { get; }
        public ICommand DeleteRepairCommand { get; }
        public ICommand AddPartCommand { get; }
        public ICommand RemovePartCommand { get; }
        public ICollectionView RepairsView { get; }

        public RepairViewModel()
        {
            Statuses.Add(new ComboItemOrderRepair { statusCode = 0, statusName = "Pending" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 1, statusName = "Shipped" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 2, statusName = "Delivered" });

            LoadRepairsCommand = new RelayCommand(async () => await LoadRepairs());
            SaveRepairCommand = new RelayCommand(async () => await SaveRepair(), () => SelectedRepair != null);
            DeleteRepairCommand = new RelayCommand(async () => await DeleteRepair(), () => SelectedRepair != null);
            RepairsView = CollectionViewSource.GetDefaultView(Repairs);
            RepairsView.Filter = FilterRepairs;

            AddPartCommand = new RelayCommand(AddPart);
            RemovePartCommand = new RelayCommand<string>(RemovePart);

        }

        private void RaiseCommandStates()
        {
            (SaveRepairCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteRepairCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ======================
        // LOAD SELECTED REPAIR
        // ======================

        private void LoadSelectedRepairIntoFields()
        {
            Parts.Clear();

            if (SelectedRepair == null)
            {
                Price = 0;
                return;
            }

            Price = SelectedRepair.price;

            if (SelectedRepair.parts != null)
            {
                foreach (var part in SelectedRepair.parts)
                    Parts.Add(part);
            }
        }


        private bool FilterRepairs(object obj)
        {
            if (obj is not Repair repair)
                return false;

            bool matchesRepairId = string.IsNullOrWhiteSpace(RepairIdFilter)
                || repair.repairID.ToString().Contains(RepairIdFilter);

            bool matchesEmail = string.IsNullOrWhiteSpace(EmailFilter)
                || (!string.IsNullOrEmpty(repair.userEmail) &&
                    repair.userEmail.ToLower().Contains(EmailFilter.ToLower()));

            return matchesRepairId && matchesEmail;
        }



        // ======================
        // API
        // ======================

        private async Task LoadRepairs()
        {
            using HttpClient client = new();
            string url = "http://localhost:5175/api/Profile/GetAllRepair";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<Repair>>(json);

            Repairs.Clear();
            foreach (var repair in list)
                Repairs.Add(repair);


        }

        private async Task SaveRepair()
        {
            if (SelectedRepair == null) return;

            var dto = new Repair
            {
                repairID = SelectedRepair.repairID,
                userID = SelectedRepair.userID,
                billingPostalCode = SelectedRepair.billingPostalCode,
                billingCity = SelectedRepair.billingCity,
                billingAddress = SelectedRepair.billingAddress,
                billingPhoneNumber = SelectedRepair.billingPhoneNumber,
                deliveryPostalCode = SelectedRepair.deliveryPostalCode,
                deliveryCity = SelectedRepair.deliveryCity,
                deliveryAddress = SelectedRepair.deliveryAddress,
                deliveryPhoneNumber = SelectedRepair.deliveryPhoneNumber,
                phoneName = SelectedRepair.phoneName,
                price = Price,
                status = SelectedRepair.status,
                manufacturerName = SelectedRepair.manufacturerName,
                phoneInspection = (sbyte)SelectedRepair.phoneInspection,
                problemDescription = SelectedRepair.problemDescription,
                parts = Parts.ToList()
            };

            using HttpClient client = new();
            string url = $"http://localhost:5175/api/Profile/updateRepair/{SelectedRepair.repairID}";

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("An error occurred while saving the repair!", "Error", MessageBoxButton.OK);
                return;
            }
            else
            {
                MessageBox.Show("Successfully updated.", "Update", MessageBoxButton.OK);


                string emailTargy = "Javitásának állapota megváltozott!";
                string emailTorzs = $@"
                    <div style=""background: linear-gradient(135deg, #2300B3 0%, #68F145 100%); margin: 0; padding: 0; min-height: 100vh;"">
                        <!-- Külső táblázat a teljes magasság és a vertikális középre igazítás miatt -->
                        <table width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""min-height: 100vh; width: 100%;"">
                            <tr>
                                <td align=""center"" valign=""middle"" style=""padding: 20px;"">
                                    
                                    <!-- Fehér kártya (formContainer stílus) -->
                                    <div style=""max-width: 600px; width: 100%; background-color: #ffffff; border-radius: 12px; box-shadow: 0 0 20px rgba(0, 0, 0, 0.25); overflow: hidden; display: inline-block; text-align: left;"">
                                        <div style=""padding: 40px 30px; text-align: center;"">
                                            
                                            <h2 style=""color: #333333; font-size: 28px; margin-bottom: 30px; margin-top: 0; font-family: Arial, sans-serif;"">Rendelésének státusza megváltozott.</h2>
                                            
                                            <h3 style=""color: #333333; font-size: 18px; margin-bottom: 20px; font-family: Arial, sans-serif;"">Kedves {SelectedRepair.userName}!</h3>
                                            
                                            <p style=""color: #555555; font-size: 16px; line-height: 1.5; margin-bottom: 35px; font-family: Arial, sans-serif;"">
Tájékoztatjuk, hogy a <i>{SelectedRepair.repairID}</i> azonosítójú rendelésének állapota megváltozott. Kérjük, tegye meg a szükséges beavatkozásokat a profiljában.<br><br>
                                                Rendelését továbbra is nyomon követheti a fiókjában.: <br><br>
                                            </p>

                                           

                                            <!-- Lábjegyzet -->
                                            <div style=""margin-top: 45px; border-top: 1px solid #eeeeee; padding-top: 20px; text-align: left;"">
                                                <p style=""color: #333333; font-size: 14px; margin: 0; font-family: Arial, sans-serif;"">Üdvözlettel,<br><strong>PhoneScout Team</strong></p>
                                                <p style=""font-size: 12px; color: #777777; margin-top: 15px; line-height: 1.4; font-family: Arial, sans-serif;"">Ez egy automatikusan generált üzenet, kérjük ne válaszoljon rá.</p>
                                            </div>
                                        </div>
                                    </div>

                                </td>
                            </tr>
                        </table>
                    </div>";



                // Email küldése (Program.cs-ben lévő metódussal)
                await EmailSending.SendEmail(SelectedRepair.userEmail, emailTargy, emailTorzs);


                RepairsView.Refresh();
            }
        }

        private async Task DeleteRepair()
        {
            if (SelectedRepair == null) return;

            using HttpClient client = new();
            string url = $"http://localhost:5175/api/Profile/deleteRepair/{SelectedRepair.repairID}";

            await client.DeleteAsync(url);
            Repairs.Remove(SelectedRepair);
            SelectedRepair = null;
        }

        // ======================
        // PARTS
        // ======================

        private void AddPart()
        {
            if (!string.IsNullOrWhiteSpace(NewPart))
            {
                Parts.Add(NewPart);
                NewPart = string.Empty;
            }
        }

        private void RemovePart(string part)
        {
            if (!string.IsNullOrEmpty(part))
                Parts.Remove(part);
        }
    }
}
