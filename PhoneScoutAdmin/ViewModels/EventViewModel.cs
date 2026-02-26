using PhoneScoutAdmin.Models;
using PhoneScoutAdmin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PhoneScoutAdmin.ViewModels
{
    public class EventViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<Event> Events { get; }
            = new ObservableCollection<Event>();


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
                    EventDate = _selectedEvent.eventDate.Date;
                    EventTime = _selectedEvent.eventDate.ToString("HH:mm:ss");
                }
                LoadSelectedEventIntoFields();
                RaiseCommandStates();

            }
        }

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

        private DateTime _eventDate;
        public DateTime EventDate
        {
            get => _eventDate;
            set { _eventDate = value; OnPropertyChanged(nameof(EventDate)); }
        }

        private string _eventTime;
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

        public ICommand LoadEventsCommand { get; }
        public ICommand SaveEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICollectionView EventsView { get; }



        public EventViewModel()
        {
            LoadEventsCommand = new RelayCommand(async () => await LoadEvents());
            SaveEventCommand = new RelayCommand(async () => await SaveEvent(), () => SelectedEvent != null);
            DeleteEventCommand = new RelayCommand(async () => await DeleteEvent(), () => SelectedEvent != null);


            EventsView = CollectionViewSource.GetDefaultView(Events);
            EventsView.Filter = FilterEvents;
        }

        private void RaiseCommandStates()
        {
            (SaveEventCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteEventCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        // ======================
        // LOGIC
        // ======================

        private void LoadSelectedEventIntoFields()
        {
            if (SelectedEvent == null)
            {
                EventName = "";
                EventHostName = "";
                EventDate = DateTime.MinValue;
                EventURL = "";
                return;
            }

            EventName = SelectedEvent.eventName;
            EventHostName = SelectedEvent.eventHostName;
            EventDate = SelectedEvent.eventDate;
            EventURL = SelectedEvent.eventURL;
        }

        private async Task LoadEvents()
        {
            using HttpClient client = new HttpClient();
            string url = "http://localhost:5175/api/event";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            string json = await response.Content.ReadAsStringAsync();
            var eventsList = JsonSerializer.Deserialize<List<Event>>(json);

            Events.Clear();
            foreach (var anEvent in eventsList)
                Events.Add(anEvent);
        }

        private async Task SaveEvent()
        {
            if (SelectedEvent == null) return;

            SelectedEvent.eventName = EventName;
            SelectedEvent.eventHostName = EventHostName;
            SelectedEvent.eventURL = EventURL;

            if (TimeSpan.TryParse(EventTime, out TimeSpan time))
            {
                DateTime combined = EventDate.Date + time;
                SelectedEvent.eventDate = combined.ToUniversalTime();
            }
            else
            {
                MessageBox.Show("Invalid time format. Use HH:mm:ss");
                return;
            }



            using HttpClient client = new HttpClient();
            string url = $"http://localhost:5175/api/event/{SelectedEvent.eventID}";

            string json = JsonSerializer.Serialize(SelectedEvent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("An error occurred while saving the event!", "Error", MessageBoxButton.OK);
                return;
            }
            else
            {
                MessageBox.Show("Successfully updated.", "Update", MessageBoxButton.OK);
                EventsView.Refresh();
                return;
            }
        }

        private async Task DeleteEvent()
        {
            if (SelectedEvent == null) return;

            using HttpClient client = new();
            string url = $"http://localhost:5175/api/event/{SelectedEvent.eventID}";

            await client.DeleteAsync(url);
            Events.Remove(SelectedEvent);
            SelectedEvent = null;
        }

        private bool FilterEvents(object obj)
        {
            if (obj is not Event anEvent)
                return false;

            bool matcheseventName = string.IsNullOrWhiteSpace(EventHostNameFilter)
                || anEvent.eventHostName.ToString().Contains(EventHostNameFilter);



            return matcheseventName;
        }

    }
}
