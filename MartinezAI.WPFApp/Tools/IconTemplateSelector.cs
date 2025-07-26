using MartinezAI.WPFApp.Enums;
using System.Windows;
using System.Windows.Controls;

namespace MartinezAI.WPFApp.Tools;

internal class IconTemplateSelector : DataTemplateSelector
{
    public DataTemplate? InfoTemplate { get; set; }
    public DataTemplate? WarningTemplate { get; set; }
    public DataTemplate? ErrorTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is MessageBoxIconFlag flag)
        {
            return flag switch
            {
                MessageBoxIconFlag.Warning => WarningTemplate,
                MessageBoxIconFlag.Error => ErrorTemplate,
                _ => InfoTemplate
            };
        }

        return InfoTemplate;
    }
}
