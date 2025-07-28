using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MartinezAI.WPFApp.Converters;

internal class RatioToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double ratio = value as double? ?? 0;
        return ratio >= 0.6 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
