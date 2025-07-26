using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MartinezAI.WPFApp.Converters;

internal class MessageCountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //--If null or not a number, just hide.
        if (value == null || (value is int count) == false)
        {
            return Visibility.Collapsed;
        }

        //--Show only if there are no chat logs.
        return count > 0
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
