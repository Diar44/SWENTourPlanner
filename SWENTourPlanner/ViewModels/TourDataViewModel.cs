using SWENTourPlanner.Models;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using log4net;
using System.Configuration;
using System.IO;
using System.Windows;
using SWENTourPlanner.Repository;


namespace SWENTourPlanner.ViewModels
{
    public class TourDataViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name;
        private string _from;
        private string _to;
        private string _description;

        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private readonly TourDatabaseHelper _tourDatabaseHelper;

        public ObservableCollection<string> AvailableLocations { get; } = new ObservableCollection<string>();


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }
        public string From
        {
            get { return _from; }
            set
            {
                _from = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }
        public string To
        {
            get { return _to; }
            set
            {
                _to = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            }
        }

        private Uri _webViewSource;
        public Uri WebViewSource
        {
            get { return _webViewSource; }
            set
            {
                _webViewSource = value;
                OnPropertyChanged(nameof(WebViewSource));
            }
        }

        public ICommand SubmitCommand { get; }

        public ICommand LoadMapCommand { get; }

        public TourDataViewModel()
        {
            SubmitCommand = new RelayCommand(param => Submit(), param => CanSubmit());

            log.Info("Tour Add initialized.");

            LoadMapCommand = new RelayCommand(param => LoadMap());

            LoadCitiesAsync();

            _tourDatabaseHelper = new TourDatabaseHelper();
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

        private void Submit()
        {
            log.Debug("TourData Submit Button clicked");

            Tour newTour = new Tour
            {
                Name = this.Name,
                From = this.From,
                To = this.To,
                Description = this.Description,
                Distance = 0,
                EstimatedTime = TimeSpan.Zero,
                RouteInformation = string.Empty, 
                TransportType = string.Empty,
            };

            try
            {
                _tourDatabaseHelper.SaveTour(newTour);
                DataSubmitted?.Invoke(this, new TourDataEventArgs(newTour));
            }
            catch (Exception ex)
            {
                log.Error("Error saving tour to database", ex);
                MessageBox.Show($"Error saving tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void LoadMap()
        {
            log.Debug("Map is Loading.");

            var apiKey = ConfigurationManager.AppSettings["apiKey"];

            string fromCoordinates, toCoordinates;

            fromCoordinates = await GeocodeOfCity(From);
            toCoordinates = await GeocodeOfCity(To);

            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri("https://api.openrouteservice.org/v2/directions/driving-car?api_key=" + apiKey + "&start=" + $"{fromCoordinates}&end={toCoordinates}".Replace("\r\n", "").Replace("[", "").Replace("]", ""));
                    //car,bycicle,foot

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    try
                    {
                        dynamic data = JsonConvert.DeserializeObject(json);

                        log.Debug(data);
                        if (data == null) log.Debug("test");

                        if (data != null && data.properties != null && data.properties.segments != null)
                        {
                            var segments = data.properties.segments;
                            if (segments.Count > 0)
                            {
                                var firstSegment = segments[0];

                                // Example: Accessing distance and duration
                                double distance = firstSegment.distance;
                                double duration = firstSegment.duration;

                                log.Debug($"Distance: {distance}");
                                log.Debug($"Duration: {duration}");
                            }
                            else
                            {
                                log.Debug("No segments found in the JSON data.");
                            }
                        }
                        else
                        {
                            log.Debug("JSON data structure is not as expected. Missing properties.");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Debug($"Error deserializing JSON data: {ex.Message}");
                    }


                    string filePath = "../../../Resources/directions.js";

                    // Define the content to write
                    string contentDirections = "var directions =" + json;

                    // Write the content to the file
                    File.WriteAllText(filePath, contentDirections);
                }
                
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, z.B. Logging oder Benutzerbenachrichtigung
                Console.WriteLine($"Fehler beim Laden der Städte: {ex.Message}");
            }

            WebViewSource = new Uri("https://www.example.com");
            WebViewSource = new Uri(AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\net8.0-windows7.0\\", "Resources/leaflet.html"));
        }

        public async Task<string> GeocodeOfCity(string city)
        {
            log.Debug("Searching geocode of "+ city + ".");

            string geocode = "";

            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"https://api.openrouteservice.org/geocode/search?api_key=5b3ce3597851110001cf6248010905636ee344a2beadd8e315e995d9&text={city}&size=1";

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    geocode = $"{data.features[0].geometry.coordinates}";
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, z.B. Logging oder Benutzerbenachrichtigung
                Console.WriteLine($"Fehler beim Laden der Städte: {ex.Message}");
            }

            return geocode;
        }


        private bool CanSubmit()
        {
            return string.IsNullOrEmpty(this["Name"]) &&
                   string.IsNullOrEmpty(this["From"]) &&
                   string.IsNullOrEmpty(this["To"]);
        }

        public event EventHandler<TourDataEventArgs> DataSubmitted;

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
                    case "Name":
                        if (string.IsNullOrWhiteSpace(Name))
                            result = "Name is required.";
                        break;
                    case "From":
                        if (string.IsNullOrWhiteSpace(From))
                            result = "From location is required.";
                        break;
                    case "To":
                        if (string.IsNullOrWhiteSpace(To))
                            result = "To location is required.";
                        break;
                }

                return result;
            }
        }

    }

    public class TourDataEventArgs : EventArgs
    {
        public Tour Tour { get; }

        public TourDataEventArgs(Tour tour)
        {
            Tour = tour;
        }
    }
}