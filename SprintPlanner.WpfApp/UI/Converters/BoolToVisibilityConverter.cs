using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SprintPlanner.WpfApp.UI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = System.Convert.ToBoolean(value);
            return val ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TrueValue.Equals(value) ? true : false;
        }
    }
}
