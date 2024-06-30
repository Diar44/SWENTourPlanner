using log4net;
using SWENTourPlanner.Models;
using SWENTourPlanner.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SWENTourPlanner.ViewModels
{
    public class TourLogViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private int _currentTourId;

        private readonly TourDatabaseHelper _tourDatabaseHelper;

        private ObservableCollection<TourLog> _tourlogs;
        public ObservableCollection<TourLog> TourLogs
        {
            get { return _tourlogs; }
            set { _tourlogs = value; OnPropertyChanged(); }
        }

        private TourLog _selectedTourLogs;
        public TourLog SelectedTourLog
        {
            get { return _selectedTourLogs; }
            set { _selectedTourLogs = value; OnPropertyChanged(); }
        }

        AddTourLog addTourLog;
        AddTourLogViewModel addTourLogViewModel;

        EditTourLogViewModel editLogViewModel;
        EditTourLogView editLogWindow;

        public ICommand AddTourLogCommand { get; }
        public ICommand EditTourLogCommand { get; }
        public ICommand DeleteTourLogCommand { get; }
        public ICommand SaveTourLogCommand { get; }


        public TourLogViewModel(int tourId)
        {
            log.Info("TourLog List initialized.");

            _currentTourId = tourId;
            _tourDatabaseHelper = new TourDatabaseHelper();
            TourLogs = new ObservableCollection<TourLog>();
            AddTourLogCommand = new RelayCommand(param => AddTourLog());
            EditTourLogCommand = new RelayCommand(param => EditTourLog());
            DeleteTourLogCommand = new RelayCommand(param => DeleteTourLog());
            SaveTourLogCommand = new RelayCommand(param => SaveLogs()); 
            LoadTourLogs(); 
        }

        private void LoadTourLogs()
        {
            TourLogs.Clear();
            var loadedTourLogs = _tourDatabaseHelper.GetTourLogs(_currentTourId);
            foreach (var tourLog in loadedTourLogs)
            {
                tourLog.IsNew = false;
                TourLogs.Add(tourLog);
            }

            log.Info($"Loaded {TourLogs.Count} tour logs for current tour ID {_currentTourId}.");
        }
        private void AddTourLog()
        {
            log.Debug("TourLog Add Button Clicked.");

            addTourLog = new AddTourLog();
            addTourLogViewModel = new AddTourLogViewModel();
            addTourLogViewModel.DataSubmitted += TourLogViewModel_DataSubmitted;
            addTourLog.DataContext = addTourLogViewModel;
            addTourLog.ShowDialog();
        }

        private void TourLogViewModel_DataSubmitted(object? sender, TourLogEventArgs e)
        {
            TourLogs.Add(e.TourLog);

            SelectedTourLog = e.TourLog;

            addTourLog.Close();
        }

        private void DeleteTourLog()
        {
            log.Debug("TourLog Delete Button Clicked.");

            if (SelectedTourLog != null)
            {
                _tourDatabaseHelper.DeleteTourLog(SelectedTourLog.Id);
                TourLogs.Remove(SelectedTourLog);
            }
        }
        private void EditTourLog()
        {
            log.Debug("TourLog Edit Button Clicked.");

            editLogViewModel = new EditTourLogViewModel();
            editLogViewModel.SelectedTourLog = SelectedTourLog;

            editLogWindow = new EditTourLogView();
            editLogWindow.DataContext = editLogViewModel;

            editLogViewModel.CloseWindow += EditTourLog_CloseWindow;

            editLogWindow.ShowDialog();
        }

        private void EditTourLog_CloseWindow(object? sender, EventArgs e)
        {
            editLogWindow.Close();

            if (editLogViewModel.SelectedTourLog != null &&
                editLogViewModel.SelectedTourLog != SelectedTourLog)
            {
                SelectedTourLog = editLogViewModel.SelectedTourLog;

                _tourDatabaseHelper.UpdateTourLog(SelectedTourLog);

                log.Info($"TourLog with ID {SelectedTourLog.Id} updated in the database.");
            }
        }

        private void SaveLogs()
        {
            log.Debug("TourLog Save Button Clicked.");

            DataSubmitted?.Invoke(this, new ListTourLogEventArgs(TourLogs));

            // Collect only the new tour logs to save
            var newTourLogs = TourLogs.Where(tl => tl.Id == 0).ToList();

            foreach (var tourLog in newTourLogs)
            {
                if (tourLog.IsNew == true)
                {
                    tourLog.TourId = _currentTourId;
                    _tourDatabaseHelper.SaveTourLog(tourLog);
                }
                tourLog.IsNew = false;
            }

            log.Info($"{newTourLogs.Count} new tour logs saved for tour ID {_currentTourId}.");
        }

        public event EventHandler<ListTourLogEventArgs> DataSubmitted;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ListTourLogEventArgs : EventArgs
    {
        public ObservableCollection<TourLog> TourLog { get; }

        public ListTourLogEventArgs(ObservableCollection<TourLog> tourLog)
        {
            TourLog = tourLog;
        }
    }

}