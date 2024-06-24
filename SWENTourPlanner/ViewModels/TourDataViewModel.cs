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


namespace SWENTourPlanner.ViewModels
{
    public class TourDataViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name;
        private string _from;
        private string _to;
        private string _description;

        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

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

        public ICommand SubmitCommand { get; }

        public TourDataViewModel()
        {
            SubmitCommand = new RelayCommand(param => Submit(), param => CanSubmit());

            log.Info("Tour Add initialized.");

            LoadCitiesAsync();
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
                Description = this.Description
            };
            DataSubmitted?.Invoke(this, new TourDataEventArgs(newTour));
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