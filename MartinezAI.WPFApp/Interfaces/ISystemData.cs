using System.Windows;

namespace MartinezAI.WPFApp.Interfaces;

public interface ISystemData
{
    Window MainViewWindow { get; set; }
    int MaxTPM { get; set; }
}
