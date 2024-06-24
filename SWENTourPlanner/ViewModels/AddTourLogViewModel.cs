using log4net;
using SWENTourPlanner.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SWENTourPlanner.ViewModels
{
    public class AddTourLogViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private DateTime _logDate;
        public DateTime LogDate
        {
            get { return _logDate; }
            set
            {
                _logDate = value;
                OnPropertyChanged(nameof(LogDate));
            }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
                RaiseSaveCanExecuteChanged();
            }
        }

        private int _difficulty;
        public int Difficulty
        {
            get { return _difficulty; }
            set
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    _difficulty = result;
                    OnPropertyChanged(nameof(Difficulty));
                    RaiseSaveCanExecuteChanged();
                }
            }
        }

        private double _totalDistance;
        public double TotalDistance
        {
            get { return _totalDistance; }
            set
            {
                if (double.TryParse(value.ToString(), out double result))
                {
                    _totalDistance = result;
                    OnPropertyChanged(nameof(TotalDistance));
                    RaiseSaveCanExecuteChanged();
                }
            }
        }

        private TimeSpan _totalTime;
        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set
            {
                if (TimeSpan.TryParse(value.ToString(), out TimeSpan result))
                {
                    _totalTime = result;
                    OnPropertyChanged(nameof(TotalTime));
                    RaiseSaveCanExecuteChanged();
                }
            }
        }

        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
                RaiseSaveCanExecuteChanged();
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public AddTourLogViewModel()
        {
            log.Info("TourLog Add initialized.");

            // Initialize LogDate to today's date
            LogDate = DateTime.Today;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSave(object obj)
        {
            return true;
        }


        private void Save(object obj)
        {
            log.Debug("TourLog Save Button Clicked.");

            TourLog newTourLog = new TourLog
            {
                Date = this.LogDate,
                Comment = this.Comment,
                Difficulty = this.Difficulty.ToString(),
                TotalDistance = this.TotalDistance,
                TotalTime = this.TotalTime,
                Rating = this.Rating
            };
            DataSubmitted?.Invoke(this, new TourLogEventArgs(newTourLog));
        }

        private void Cancel(object obj)
        {
            MessageBox.Show("Cancel command executed.");
        }

        public ICommand SubmitCommand { get; }

        public event EventHandler<TourLogEventArgs> DataSubmitted;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseSaveCanExecuteChanged()
        {
            ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged();
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName == "TotalDistance")
                {
                    if (!double.TryParse(TotalDistance.ToString(), out double result))
                    {
                        return "Total Distance must be a numeric value.";
                    }
                }

                if (columnName == "TotalTime")
                {
                    if (!TimeSpan.TryParse(TotalTime.ToString(), out TimeSpan result))
                    {
                        return "Total Time must be a numeric value.";
                    }
                }

                if (columnName == "Difficulty")
                {
                    if (!int.TryParse(Difficulty.ToString(), out int result))
                    {
                        return "Difficulty must be a numeric value.";
                    }
                }

                return null;
            }
        }
    }

    public class TourLogEventArgs : EventArgs
    {
        public TourLog TourLog { get; }

        public TourLogEventArgs(TourLog tourLog)
        {
            TourLog = tourLog;
        }
    }
}