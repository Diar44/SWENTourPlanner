using SWENTourPlanner.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SWENTourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
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
        }

        private void OpenFileDialogMethod()
        {
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
            Tours.Remove(SelectedTour);
        }
        private void EditTour()
        {

            if (SelectedTour != null)
            {
                editViewModel = new EditTourViewModel();
                editViewModel.SelectedTour = SelectedTour;

                editWindow = new EditTour();
                editWindow.DataContext = editViewModel;

                editViewModel.CloseWindow += EditTour_CloseWindow;

                editWindow.ShowDialog();
            }

        }
        private void EditTour_CloseWindow(object? sender, EventArgs e)
        {
            editWindow.Close();
        }


        public void OpenTourLogs()
        {
            if (SelectedTour != null)
            {
                tourLogViewModel = new TourLogViewModel();
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
