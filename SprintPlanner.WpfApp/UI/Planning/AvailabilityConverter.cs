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
            var scaledCapacity = (decimal)values[1] * 100;
            if (availability >= 0)
            {
                return $"{availability} h to {scaledCapacity}%";
            }
            else
            {
                return $"{-1 * availability} h over {scaledCapacity}%";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
