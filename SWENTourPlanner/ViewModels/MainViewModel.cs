using log4net;
using SWENTourPlanner.Models;
using SWENTourPlanner.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SWENTourPlanner.Repository;

namespace SWENTourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private readonly TourDatabaseHelper _tourDatabaseHelper;

        private ObservableCollection<Tour> _tours;
        public ObservableCollection<Tour> Tours
        {
            get { return _tours; }
            set { _tours = value; OnPropertyChanged(); }
        }

        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get { return _selectedTour; }
            set { _selectedTour = value; OnPropertyChanged(); }
        }

        private Dictionary<string, ObservableCollection<TourLog>> _logs;

        //Create TourData
        public TourData inputWindow;
        public TourDataViewModel inputViewModel;

        //Create EditTour
        public EditTour editWindow;
        public EditTourViewModel editViewModel;

        //Create TourLogView
        TourLogView tourLogView;
        public TourLogViewModel tourLogViewModel;

        public ICommand AddTourCommand { get; }
        public ICommand EditTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand ShowTourLog { get; }
        public ICommand OpenFileCommand { get; }

        public MainViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            AddTourCommand = new RelayCommand(param => AddTour());
            EditTourCommand = new RelayCommand(param => EditTour());
            DeleteTourCommand = new RelayCommand(param => DeleteTour());
            ShowTourLog = new RelayCommand(param => OpenTourLogs());
            OpenFileCommand = new RelayCommand(param => OpenFileDialogMethod());
            _logs = new Dictionary<string, ObservableCollection<TourLog>>();
            _tourDatabaseHelper = new TourDatabaseHelper();

            log.Info("MainWindow initialized.");

            RefreshTours();
        }

        private void RefreshTours()
        {
            try
            {
                var toursFromDb = _tourDatabaseHelper.GetTours();
                Tours.Clear();
                foreach (var tour in toursFromDb)
                {
                    Tours.Add(tour);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error fetching tours from database", ex);
                MessageBox.Show("Error fetching tours from database. See log for details.");
            }
        }
        private void OpenFileDialogMethod()
        {
            log.Debug("OpenFileDialog.");

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                MessageBox.Show(filename);
            }
        }

        private void AddTour()
        {
            log.Debug("AddTour Button clicked");

            inputWindow = new TourData();
            inputViewModel = new TourDataViewModel();
            inputViewModel.DataSubmitted += TourDataViewModel_DataSubmitted;
            inputWindow.DataContext = inputViewModel;
            inputWindow.ShowDialog();
        }

        private void TourDataViewModel_DataSubmitted(object? sender, TourDataEventArgs e)
        {
            Tours.Add(e.Tour);

            SelectedTour = e.Tour;

            inputWindow.Close();
        }

        private void DeleteTour()
        {
            log.Debug("DeleteTour Button clicked");

            if (SelectedTour != null)
            {
                try
                {
                    _tourDatabaseHelper.DeleteTour(SelectedTour.Id);
                    log.Info($"Deleted tour with ID {SelectedTour.Id}.");
                    Tours.Remove(SelectedTour);
                }
                catch (Exception ex)
                {
                    log.Error("Error deleting tour from database", ex);
                    MessageBox.Show("Error deleting tour from database. See log for details.");
                }
            }
        }
        private void EditTour()
        {
            log.Debug("EditTour Button clicked");


            if (SelectedTour != null)
            {
                editViewModel = new EditTourViewModel();
                editViewModel.SelectedTour = SelectedTour;

                editWindow = new EditTour();
                editWindow.DataContext = editViewModel;

                editViewModel.CloseWindow += EditTour_CloseWindow;

                editWindow.ShowDialog();

                _tourDatabaseHelper.UpdateTour(SelectedTour);
                log.Info($"Updated tour with ID {SelectedTour.Id}.");
            }

        }
        private void EditTour_CloseWindow(object? sender, EventArgs e)
        {
            log.Debug("EditTour CloseWindow Button clicked");

            editWindow.Close();
        }


        public void OpenTourLogs()
        {
            log.Debug("OpenTourLogs Button clicked");

            if (SelectedTour != null)
            {
                tourLogViewModel = new TourLogViewModel(_selectedTour.Id);
                if (_logs.ContainsKey(SelectedTour.Name))
                {
                    tourLogViewModel.TourLogs = _logs[SelectedTour.Name];
                }
                tourLogView = new TourLogView();
                tourLogView.DataContext = tourLogViewModel;
                tourLogViewModel.DataSubmitted += TourLogViewModel_DataSubmitted;
                tourLogView.ShowDialog();
            }
        }

        private void TourLogViewModel_DataSubmitted(object? sender, ListTourLogEventArgs e)
        {

            if (!_logs.ContainsKey(SelectedTour.Name))
            {
                _logs.Add(SelectedTour.Name, e.TourLog);
            }
            else
            {
                _logs[SelectedTour.Name] = e.TourLog;
            }
            tourLogView.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
