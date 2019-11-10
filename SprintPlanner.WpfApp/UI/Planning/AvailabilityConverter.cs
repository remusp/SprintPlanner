using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class AvailabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var availability = (decimal)value;
            if (availability >= 0)
            {
                return $"{availability} h to 80%"; // TODO: do not hardcode
            }
            else 
            {
                return $"{-1 * availability} h over 80%"; // TODO: do not hardcode
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
