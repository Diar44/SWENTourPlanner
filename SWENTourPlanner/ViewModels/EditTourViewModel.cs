using log4net;
using Newtonsoft.Json;
using SWENTourPlanner.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SWENTourPlanner.ViewModels
{
    public class EditTourViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public ObservableCollection<string> AvailableLocations { get; } = new ObservableCollection<string>();

        public bool IsEditingEnabled { get; } = true;

        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get { return _selectedTour; }
            set { _selectedTour = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public EditTourViewModel()
        {
            log.Info("Tour Edit initialized.");

            LoadCitiesAsync();

            SaveCommand = new RelayCommand(param => Save(), param => CanSave());
        }

        private async void LoadCitiesAsync()
        {
            log.Debug("Cities Loaded from Api");

            try
            {
                using (var client = new HttpClient())
                {
                    string username = "if23b052";
                    string url = $"http://api.geonames.org/searchJSON?maxRows=1000&featureClass=P&username={username}";

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    AvailableLocations.Clear();
                    foreach (var city in data.geonames)
                    {
                        AvailableLocations.Add(city.name.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, z.B. Logging oder Benutzerbenachrichtigung
                Console.WriteLine($"Fehler beim Laden der Städte: {ex.Message}");
            }
        }

        private bool CanSave()
        {
            // Validate From and To fields
            return !string.IsNullOrWhiteSpace(SelectedTour?.From) && !string.IsNullOrWhiteSpace(SelectedTour?.To);
        }

        private void Save()
        {
            log.Debug("Save Edited Tour.");

            CloseWindow?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> CloseWindow;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // IDataErrorInfo implementation
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                switch (columnName)
                {
                    case "From":
                        if (string.IsNullOrWhiteSpace(SelectedTour?.From))
                            result = "From field is required.";
                        break;
                    case "To":
                        if (string.IsNullOrWhiteSpace(SelectedTour?.To))
                            result = "To field is required.";
                        break;
                }

                return result;
            }
        }
    }
}