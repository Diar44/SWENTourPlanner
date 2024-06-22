using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace SWENTourPlanner.Converters
{
    public class CommandToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICommand command)
            {
                return command.CanExecute(null);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
