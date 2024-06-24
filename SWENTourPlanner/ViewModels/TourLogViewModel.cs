using log4net;
using SWENTourPlanner.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SWENTourPlanner.ViewModels
{
    public class TourLogViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

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


        public TourLogViewModel()
        {
            log.Info("TourLog List initialized.");

            TourLogs = new ObservableCollection<TourLog>();
            AddTourLogCommand = new RelayCommand(param => AddTourLog());
            EditTourLogCommand = new RelayCommand(param => EditTourLog());
            DeleteTourLogCommand = new RelayCommand(param => DeleteTourLog());
            SaveTourLogCommand = new RelayCommand(param => SaveLogs());
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

            TourLogs.Remove(SelectedTourLog);
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
        }

        private void SaveLogs()
        {
            log.Debug("TourLog Save Button Clicked.");

            DataSubmitted?.Invoke(this, new ListTourLogEventArgs(TourLogs));
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