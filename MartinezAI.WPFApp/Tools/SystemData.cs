using MartinezAI.WPFApp.Interfaces;
using System.Windows;

namespace MartinezAI.WPFApp.Tools;

internal class SystemData : ISystemData
{
    public Window MainViewWindow { get; set; } = null!;
}
