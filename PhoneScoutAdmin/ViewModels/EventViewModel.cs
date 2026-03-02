using Microsoft.Win32;
using PhoneScoutAdmin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhoneScoutAdmin.ViewModels
{
    public class EventViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ===============================
        // ROLE CONTEXT
        // ===============================

        private readonly bool _isAdmin;
        private readonly string? _manufacturerName;

        public EventViewModel(bool isAdmin, string? manufacturerName = null)
        {
            _isAdmin = isAdmin;
            _manufacturerName = manufacturerName;

            LoadEventsCommand = new RelayCommand(async () => await LoadEvents());

            SaveEventCommand = new RelayCommand(
                async () => await SaveEvent(),
                () => SelectedEvent != null || IsCreate);

            DeleteEventCommand = new RelayCommand(
                async () => await DeleteEvent(),
                () => SelectedEvent != null);

            CreateEventCommand = new RelayCommand(async () => await CreateEvent());

            SelectFileCommand = new RelayCommand(SelectFile);

            EventsView = CollectionViewSource.GetDefaultView(Events);
            EventsView.Filter = FilterEvents;
        }

        // ===============================
        // DATA
        // ===============================

        public ObservableCollection<Event> Events { get; } = new();
        public ICollectionView EventsView { get; }

        // ===============================
        // FILTERING (ROLE + SEARCH)
        // ===============================

        private string _eventHostNameFilter;
        public string EventHostNameFilter
        {
            get => _eventHostNameFilter;
            set
            {
                _eventHostNameFilter = value;
                OnPropertyChanged(nameof(EventHostNameFilter));
                EventsView.Refresh();
            }
        }

        private bool FilterEvents(object obj)
        {
            if (obj is not Event ev) return false;

            // Manufacturer restriction
            if (!_isAdmin && ev.eventHostName != _manufacturerName)
                return false;

            // Search filter
            return string.IsNullOrWhiteSpace(EventHostNameFilter) ||
                   ev.eventHostName.Contains(EventHostNameFilter,
                       StringComparison.OrdinalIgnoreCase);
        }

        // ===============================
        // SELECTED EVENT
        // ===============================

        private Event _selectedEvent;
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));

                if (_selectedEvent != null)
                {
                    EventName = _selectedEvent.eventName;
                    EventHostName = _selectedEvent.eventHostName;
                    EventURL = _selectedEvent.eventURL;

                    EventDate = _selectedEvent.eventDate.Date;
                    EventTime = _selectedEvent.eventDate.ToString("HH:mm:ss");

                    if (!string.IsNullOrEmpty(_selectedEvent.imageBase64))
                        EventImage = Base64ToBitmap(_selectedEvent.imageBase64);
                    else
                        EventImage = null;

                    IsCreate = false;
                }

                RaiseCommandStates();
            }
        }

        // ===============================
        // FORM FIELDS
        // ===============================

        public string EventName { get; set; }
        public string EventHostName { get; set; }
        public DateTime EventDate { get; set; } = DateTime.Now;
        public string EventTime { get; set; } = DateTime.Now.ToString("HH:mm:ss");
        public string EventURL { get; set; }
        public BitmapImage EventImage { get; set; }

        // ===============================
        // MODE
        // ===============================

        private bool _isCreate;
        public bool IsCreate
        {
            get => _isCreate;
            set
            {
                _isCreate = value;
                OnPropertyChanged(nameof(IsCreate));
                OnPropertyChanged(nameof(ButtonTextDisplay));
                RaiseCommandStates();
            }
        }

        public string ButtonTextDisplay =>
            IsCreate ? "Create Event" : "Save Changes";

        // ===============================
        // COMMANDS
        // ===============================

        public ICommand LoadEventsCommand { get; }
        public ICommand SaveEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand CreateEventCommand { get; }
        public ICommand SelectFileCommand { get; }

        private void RaiseCommandStates()
        {
            (SaveEventCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteEventCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ===============================
        // LOAD EVENTS
        // ===============================

        public async Task LoadEvents()
        {
            using HttpClient client = new();
            var response = await client.GetAsync("http://localhost:5175/api/event");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<Event>>(json);

            Events.Clear();
            foreach (var ev in list)
                Events.Add(ev);

            EventsView.Refresh();
        }

        // ===============================
        // SAVE / DELETE
        // ===============================

        private async Task SaveEvent()
        {
            using HttpClient client = new();

            if (!TimeSpan.TryParse(EventTime, out TimeSpan time))
            {
                MessageBox.Show("Invalid time format. Use HH:mm:ss");
                return;
            }

            DateTime combined = EventDate.Date + time;
            DateTime utcDate = combined.ToUniversalTime();

            MultipartFormDataContent form = new();
            form.Add(new StringContent(EventName), "eventName");
            form.Add(new StringContent(EventHostName), "eventHostName");
            form.Add(new StringContent(utcDate.ToString("o")), "eventDate");
            form.Add(new StringContent(EventURL ?? ""), "eventURL");

            if (EventImage != null)
            {
                var base64 = BitmapToBase64(EventImage);
                byte[] fileBytes = Convert.FromBase64String(base64);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue("image/png");

                form.Add(fileContent, "file", "image.png");
            }

            HttpResponseMessage response;

            if (IsCreate)
                response = await client.PostAsync("http://localhost:5175/api/event", form);
            else
                response = await client.PutAsync(
                    $"http://localhost:5175/api/event/{SelectedEvent.eventID}", form);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(await response.Content.ReadAsStringAsync());
                return;
            }

            MessageBox.Show(IsCreate ? "Event created." : "Event updated.");

            IsCreate = false;
            await LoadEvents();
        }

        private async Task DeleteEvent()
        {
            if (SelectedEvent == null) return;

            using HttpClient client = new();
            await client.DeleteAsync(
                $"http://localhost:5175/api/event/{SelectedEvent.eventID}");

            Events.Remove(SelectedEvent);
            SelectedEvent = null;
        }

        private async Task CreateEvent()
        {
            SelectedEvent = null;
            EventName = "";
            EventHostName = _manufacturerName ?? "";
            EventURL = "";
            EventDate = DateTime.Now;
            EventTime = DateTime.Now.ToString("HH:mm:ss");
            EventImage = null;
            IsCreate = true;
        }

        // ===============================
        // IMAGE
        // ===============================

        private BitmapImage Base64ToBitmap(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            using MemoryStream ms = new(bytes);
            BitmapImage image = new();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private string BitmapToBase64(BitmapImage bitmap)
        {
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using MemoryStream ms = new();
            encoder.Save(ms);
            return Convert.ToBase64String(ms.ToArray());
        }

        private void SelectFile()
        {
            OpenFileDialog dlg = new();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dlg.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                EventImage = bitmap;
                OnPropertyChanged(nameof(EventImage));
            }
        }
    }
}