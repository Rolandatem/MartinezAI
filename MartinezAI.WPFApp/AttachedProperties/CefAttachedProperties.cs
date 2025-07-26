using CefSharp;
using CefSharp.Wpf;
using MartinezAI.WPFApp.Interfaces;
using MartinezAI.WPFApp.Tools;
using MartinezAI.WPFApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MartinezAI.WPFApp.AttachedProperties;

public static class CefAttachedProperties
{
	#region "Dependency Properties"
	public static readonly DependencyProperty HtmlProperty =
		DependencyProperty.RegisterAttached(
			"Html",
			typeof(string),
			typeof(CefAttachedProperties),
			new FrameworkPropertyMetadata(String.Empty, OnHtmlChangedAsync));

	public static string GetHtml(DependencyObject obj) => (string)obj.GetValue(HtmlProperty);
	public static void SetHtml(DependencyObject obj, string value) => obj.SetValue(HtmlProperty, value);
	#endregion

	#region "Events"
	private static void OnHtmlChangedAsync(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ChromiumWebBrowser webBrowser)
		{
			IMarkupToHtmlConverter converter = BaseViewModel.IsInDesignMode
				? new MarkupToHtmlConverter()
				: ServiceHelper.Services.GetRequiredService<IMarkupToHtmlConverter>();
			string markup = (string)e.NewValue;

            webBrowser.Dispatcher.Invoke(() =>
            {
                string fullHtml = converter.ConvertAll(markup);
                webBrowser.LoadHtml(fullHtml);
            });
        }
	}
	#endregion
}
