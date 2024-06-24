using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using log4net;
using SWENTourPlanner.Models;

namespace SWENTourPlanner.ViewModels
{
    internal class EditTourLogViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private TourLog _selectedTourLog;
        public TourLog SelectedTourLog
        {
            get { return _selectedTourLog; }
            set { _selectedTourLog = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public EditTourLogViewModel()
        {
            log.Info("TourLog Edit initialized.");

            SaveCommand = new RelayCommand(param => Save());
        }

        private void Save()
        {
            log.Debug("TourLog Save Button Clicked.");

            CloseWindow?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> CloseWindow;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName == "TotalDistance")
                {
                    if (!double.TryParse(SelectedTourLog.TotalDistance.ToString(), out _))
                    {
                        return "Total Distance must be a numeric value.";
                    }
                }

                if (columnName == "TotalTime")
                {
                    if (!TimeSpan.TryParse(SelectedTourLog.TotalTime.ToString(), out _))
                    {
                        return "Total Time must be a numeric value.";
                    }
                }

                if (columnName == "Difficulty")
                {
                    if (!int.TryParse(SelectedTourLog.Difficulty.ToString(), out _))
                    {
                        return "Difficulty must be a numeric value.";
                    }
                }

                return null;
            }
        }
    }
}
