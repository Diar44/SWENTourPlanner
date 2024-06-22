using SWENTourPlanner.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SWENTourPlanner.ViewModels
{
    public class EditTourViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
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
            SaveCommand = new RelayCommand(param => Save(), param => CanSave());
        }

        private bool CanSave()
        {
            // Validate From and To fields
            return !string.IsNullOrWhiteSpace(SelectedTour?.From) && !string.IsNullOrWhiteSpace(SelectedTour?.To);
        }

        private void Save()
        {
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