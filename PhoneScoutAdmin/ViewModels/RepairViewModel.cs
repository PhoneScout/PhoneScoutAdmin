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
using System.Windows.Media;

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
        // ORIGINAL VALUES (Change Tracking)
        // ======================

        private int _originalPrice;
        private string _originalReapirDesc;
        private int _originalStatus;
        private List<string> _originalParts = new();

        // ======================
        // NOT EDITABLE FIELDS DESIGN FOR DISPLAY
        // ======================

        private int _isPriceAccepted;
        public int IsPriceAccepted
        {
            get => _isPriceAccepted;
            set
            {
                _isPriceAccepted = value;
                OnPropertyChanged(nameof(IsPriceAccepted));
                OnPropertyChanged(nameof(IsPriceAcceptedDisplay));
                OnPropertyChanged(nameof(IsPriceAcceptedBrush));
            }
        }

        public string IsPriceAcceptedDisplay
        {
            get => IsPriceAccepted==0 ? "Waiting for user." : _isPriceAccepted == 1?"Price accepted.":"Price not accepted.";
        }

        public Brush IsPriceAcceptedBrush =>
    IsPriceAccepted == 0 ? Brushes.Gray :
    IsPriceAccepted == 1 ? Brushes.Green :
    Brushes.Red;

        private int _phoneInspection;
        public int PhoneInspection
        {
            get => _phoneInspection;
            set
            {
                _phoneInspection = value;
                OnPropertyChanged(nameof(PhoneInspection));
                OnPropertyChanged(nameof(PhoneInspectionDisplay));
                OnPropertyChanged(nameof(PhoneInspectionBrush));
            }
        }

        public string PhoneInspectionDisplay
        {
            get => PhoneInspection == 1 ? "Phone inspection is required by user" : "Phone inspection is not required by user";
        }

        public Brush PhoneInspectionBrush =>
    PhoneInspection == 0 ? Brushes.Red :
    Brushes.Green;

        // ======================
        // EDIT FIELDS
        // ======================

        private int _repairPrice;
        public int RepairPrice
        {
            get => _repairPrice;
            set { _repairPrice = value; OnPropertyChanged(nameof(RepairPrice)); }
        }

        private string _repairDescription;
        public string RepairDescription
        {
            get => _repairDescription;
            set { _repairDescription = value; OnPropertyChanged(nameof(RepairDescription)); }
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
            Statuses.Add(new ComboItemOrderRepair { statusCode = 0, statusName = "Feldolgozás alatt" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 1, statusName = "Feldolgozva" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 2, statusName = "Átvételre kész" });
            Statuses.Add(new ComboItemOrderRepair { statusCode = 3, statusName = "Teljesítve" });

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
                RepairPrice = 0;
                return;
            }

            // Set editable fields
            RepairPrice = SelectedRepair.repairPrice;
            RepairDescription = SelectedRepair.repairDescription;

            IsPriceAccepted = SelectedRepair.isPriceAccepted;
            PhoneInspection = SelectedRepair.phoneInspection;

            if (SelectedRepair.parts != null)
            {
                foreach (var part in SelectedRepair.parts)
                    Parts.Add(part);
            }

            // ===== STORE ORIGINAL VALUES =====
            _originalPrice = SelectedRepair.repairPrice;
            _originalReapirDesc = SelectedRepair.repairDescription;
            _originalStatus = SelectedRepair.status;
            _originalParts = SelectedRepair.parts != null
                ? new List<string>(SelectedRepair.parts)
                : new List<string>();
        }


        public bool IsPriceChanged()
        { 
            return RepairPrice != _originalPrice;
        }

        private bool IsRepairDescriptionChanged()
        {
            return RepairDescription != _originalReapirDesc;
        }

        private bool IsStatusChanged()  
        {
            return SelectedRepair != null &&
                   SelectedRepair.status != _originalStatus;
        }

        public bool ArePartsChanged()
        {
            return !_originalParts.OrderBy(x => x)
                      .SequenceEqual(Parts.OrderBy(x => x));
        }

        private bool FilterRepairs(object obj)
        {
            if (obj is not Repair repair)
                return false;

            bool matchesRepairId = string.IsNullOrWhiteSpace(RepairIdFilter)
                || repair.repairID.ToString().Contains(RepairIdFilter, StringComparison.OrdinalIgnoreCase);

            bool matchesEmail = string.IsNullOrWhiteSpace(EmailFilter)
                || (!string.IsNullOrEmpty(repair.userEmail) &&
                    repair.userEmail.ToLower().Contains(EmailFilter, StringComparison.OrdinalIgnoreCase));

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
                basePrice = SelectedRepair.basePrice,
                repairPrice = RepairPrice,
                isPriceAccepted = (IsPriceChanged()?0:SelectedRepair.isPriceAccepted),
                status = SelectedRepair.status,
                manufacturerName = SelectedRepair.manufacturerName,
                phoneInspection = (sbyte)SelectedRepair.phoneInspection,
                problemDescription = SelectedRepair.problemDescription,
                repairDescription = RepairDescription,
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

                bool priceChanged = IsPriceChanged();
                bool repairDescChanged = IsRepairDescriptionChanged();
                bool statusChanged = IsStatusChanged();
                bool partsChanged = ArePartsChanged();

                if (!priceChanged && !statusChanged && !partsChanged && !repairDescChanged)
                {
                    MessageBox.Show("No changes detected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if(priceChanged && !repairDescChanged || !priceChanged && repairDescChanged)
                {
                    MessageBox.Show("Price and description should be changed at the same time.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (IsPriceAccepted == 1 && (priceChanged || repairDescChanged || partsChanged))
                {
                    MessageBox.Show("Once the price is accepted, the price, description or parts list cannot be modified.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (IsPriceAccepted == 2)
                {
                    MessageBox.Show("The price is not accepted. No further modifications allowed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Successfully updated.", "Update", MessageBoxButton.OK, MessageBoxImage.Information);

                string emailText = "";

                var changes = new List<string>();

                if (priceChanged)
                    changes.Add("ára");

                if (statusChanged)
                    changes.Add("státusza");

                if (partsChanged)
                    changes.Add("a javításhoz használt alkatrészlista");

                if (changes.Any())
                {
                    string changeText;

                    if (changes.Count == 1)
                    {
                        changeText = changes[0];
                    }
                    else
                    {
                        changeText = string.Join(", ", changes.Take(changes.Count - 1))
                                     + " és " + changes.Last();
                    }

                    emailText += $@"
        Tájékoztatjuk, hogy a 
        <i>{SelectedRepair.repairID}</i> azonosítójú javításának 
        {changeText} megváltozott!<br><br>";

                    if (statusChanged)
                    {
                        emailText += $"Új státusz: <b>{Statuses[SelectedRepair.status].statusName}</b><br><br>";
                    }
                    if (priceChanged)
                    {
                        emailText += $"Kérjük, tegye meg a szükséges intézkedéseket a profiljában.<br><br> A javítás leírása:<br> {RepairDescription}";
                    }
                }

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
{emailText}
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

            var result = MessageBox.Show(
                $"Are you sure you want to delete the selected repair: {SelectedRepair.repairID}?",
                "Delete repair",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using HttpClient client = new();
                string url = $"http://localhost:5175/api/Profile/deleteRepair/{SelectedRepair.repairID}";

                await client.DeleteAsync(url);
                Repairs.Remove(SelectedRepair);
                SelectedRepair = null;
            }

            
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
