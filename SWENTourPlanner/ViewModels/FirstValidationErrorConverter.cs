using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace SWENTourPlanner.Converters
{
    public class FirstValidationErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errors = value as System.Collections.IList;
            if (errors == null || errors.Count == 0)
            {
                return null;
            }

            var firstError = errors[0] as ValidationError;
            return firstError?.ErrorContent?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
