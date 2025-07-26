using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MartinezAI.WPFApp.Converters;

internal class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = (bool)value;
        string? paramValue = parameter?.ToString();

        if (boolValue == true)
        {
            return Visibility.Visible;
        }
        else if (paramValue == null || paramValue.ToLower() == "c" || paramValue.ToLower() == "collapsed")
        {
            return Visibility.Collapsed;
        }
        else
        {
            return Visibility.Hidden;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = (Visibility)value;

        if (visibility == Visibility.Visible)
        {
            return true;
        }

        return false;
    }
}
