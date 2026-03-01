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

        public ObservableCollection<Event> Events { get; } = new ObservableCollection<Event>();
        public ICollectionView EventsView { get; }

        #region Selected Event

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
                    EventHostURL = _selectedEvent.eventHostURL;
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

        #endregion

        #region Form Fields

        private string _eventName;
        public string EventName
        {
            get => _eventName;
            set { _eventName = value; OnPropertyChanged(nameof(EventName)); }
        }

        private string _eventHostName;
        public string EventHostName
        {
            get => _eventHostName;
            set { _eventHostName = value; OnPropertyChanged(nameof(EventHostName)); }
        }

        private string _eventHostURL;
        public string EventHostURL
        {
            get => _eventHostURL;
            set { _eventHostURL = value; OnPropertyChanged(nameof(EventHostURL)); }
        }

        private DateTime _eventDate = DateTime.Now;
        public DateTime EventDate
        {
            get => _eventDate;
            set { _eventDate = value; OnPropertyChanged(nameof(EventDate)); }
        }

        private string _eventTime = DateTime.Now.ToString("HH:mm:ss");
        public string EventTime
        {
            get => _eventTime;
            set { _eventTime = value; OnPropertyChanged(nameof(EventTime)); }
        }

        private string _eventURL;
        public string EventURL
        {
            get => _eventURL;
            set { _eventURL = value; OnPropertyChanged(nameof(EventURL)); }
        }

        #endregion

        #region Image Handling (UI uses BitmapImage)

        private BitmapImage _eventImage;
        public BitmapImage EventImage
        {
            get => _eventImage;
            set
            {
                _eventImage = value;
                OnPropertyChanged(nameof(EventImage));
            }
        }

        private BitmapImage Base64ToBitmap(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(bytes))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        private string BitmapToBase64(BitmapImage bitmap)
        {
            if (bitmap == null) return null;

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        #endregion

        #region Mode Handling

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

        #endregion

        #region Filtering

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

            return string.IsNullOrWhiteSpace(EventHostNameFilter) ||
                   ev.eventHostName.Contains(EventHostNameFilter,
                       StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Commands

        public ICommand LoadEventsCommand { get; }
        public ICommand SaveEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand CreateEventCommand { get; }
        public ICommand SelectFileCommand { get; }

        public EventViewModel()
        {
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

        private void RaiseCommandStates()
        {
            (SaveEventCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteEventCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        #endregion

        #region CRUD

        public async Task LoadEvents()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/event";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var eventsList = JsonSerializer.Deserialize<List<Event>>(json);

            Events.Clear();
            foreach (var ev in eventsList)
                Events.Add(ev);
        }

        private async Task SaveEvent()
        {
            using HttpClient client = new HttpClient();

            if (!TimeSpan.TryParse(EventTime, out TimeSpan time))
            {
                MessageBox.Show("Invalid time format. Use HH:mm:ss");
                return;
            }

            DateTime combined = EventDate.Date + time;
            DateTime utcDate = combined.ToUniversalTime();

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(EventName), "eventName");
            form.Add(new StringContent(EventHostName), "eventHostName");
            form.Add(new StringContent(utcDate.ToString("o")), "eventDate");
            form.Add(new StringContent(EventURL ?? ""), "eventURL");

            string imageBase64 = BitmapToBase64(EventImage);

            if (!string.IsNullOrEmpty(imageBase64))
            {
                byte[] fileBytes = Convert.FromBase64String(imageBase64);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue("image/png");

                form.Add(fileContent, "file", "image.png");
            }

            HttpResponseMessage response;

            if (IsCreate)
            {
                response = await client.PostAsync(
                    "http://localhost:5175/api/event", form);


            }
            else
            {
                string url =
                    $"http://localhost:5175/api/event/{SelectedEvent.eventID}";

                response = await client.PutAsync(url, form);
            }

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();

                MessageBox.Show(
                    $"Status: {response.StatusCode}\n\n" +
                    $"Reason: {response.ReasonPhrase}\n\n" +
                    $"Server Response:\n{errorContent}"
                );

                return;
            }
            MessageBox.Show(IsCreate
                ? "Event created."
                : "Event updated.");

            IsCreate = false;
            await LoadEvents();
        }

        private async Task DeleteEvent()
        {
            if (SelectedEvent == null) return;

            using HttpClient client = new();
            string url =
                $"http://localhost:5175/api/event/{SelectedEvent.eventID}";

            await client.DeleteAsync(url);
            Events.Remove(SelectedEvent);
            SelectedEvent = null;
        }

        private async Task CreateEvent()
        {
            SelectedEvent = null;

            EventName = "";
            EventHostName = "";
            EventURL = "";
            EventDate = DateTime.Now;
            EventTime = DateTime.Now.ToString("HH:mm:ss");
            EventImage = null;

            IsCreate = true;
        }

        #endregion

        #region File Picker

        private void SelectFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dlg.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                EventImage = bitmap;
            }
        }

        #endregion
    }
}