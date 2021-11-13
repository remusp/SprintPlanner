using System;
using System.Globalization;
using System.Windows.Data;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class AvailabilityConverter : IMultiValueConverter
    {


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var availability = (decimal)values[0];
            var targetAvailability = (decimal)values[1];
            if (availability >= 0)
            {
                return $"{availability} h to {targetAvailability}%";
            }
            else
            {
                return $"{-1 * availability} h over {targetAvailability}%";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
