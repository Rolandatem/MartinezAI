using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MartinezAI.WPFApp.Tools;

public static class ServiceHelper
{
    public static IServiceProvider Services { get; set; } = null!;

    public static IMarkupToHtmlConverter? MarkdownToHtmlConverter
    {
        get
        {
            field ??= BaseViewModel.IsInDesignMode
                    ? new MarkupToHtmlConverter()
                    : ServiceHelper.Services.GetRequiredService<IMarkupToHtmlConverter>();

            return field;
        }
    }
}
