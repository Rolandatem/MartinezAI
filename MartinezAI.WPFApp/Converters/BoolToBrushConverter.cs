using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MartinezAI.WPFApp.Converters;

internal class BoolToBrushConverter : IValueConverter
{
    public required Brush TrueBrush { get; set; }
    public required Brush FalseBrush { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool boolean && boolean)
            ? TrueBrush 
            : FalseBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
