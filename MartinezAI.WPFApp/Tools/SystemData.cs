using MartinezAI.WPFApp.Interfaces;
using System.Windows;

namespace MartinezAI.WPFApp.Tools;

internal class SystemData : NotifyableClass, ISystemData
{
    public Window MainViewWindow { get; set; } = null!;

    public int MaxTPM
    {
        get => Properties.Settings.Default.MaxTMP;
        set
        {
            Properties.Settings.Default.MaxTMP = value;
            OnPropertyChanged();
        }
    }
}
