using CefSharp;
using CefSharp.Wpf;
using MartinezAI.WPFApp.Tools;
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

	private static readonly DependencyProperty IsContentLoadedProperty =
		DependencyProperty.RegisterAttached(
			"IsContentLoaded",
			typeof(bool),
			typeof(CefAttachedProperties),
			new FrameworkPropertyMetadata(false));

	public static bool GetIsContentLoaded(DependencyObject obj) => (bool)obj.GetValue(IsContentLoadedProperty);
	public static void SetIsContentLoaded(DependencyObject obj, bool value) => obj.SetValue(IsContentLoadedProperty, value);

	public static readonly DependencyProperty IsContentCompleteProperty =
		DependencyProperty.RegisterAttached(
			"IsContentComplete",
			typeof(bool?),
			typeof(CefAttachedProperties),
			new FrameworkPropertyMetadata(null, OnContentCompleteChangedAsync));

	public static bool? GetIsContentComplete(DependencyObject obj) => (bool?)obj.GetValue(IsContentCompleteProperty);
	public static void SetIsContentComplete(DependencyObject obj, bool? value) => obj.SetValue(IsContentCompleteProperty, value);
	#endregion

	#region "Events"
	private static void OnHtmlChangedAsync(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ChromiumWebBrowser webBrowser)
		{
			string markdown = (string)e.NewValue;

			webBrowser.Dispatcher.Invoke(async () =>
			{
				if (GetIsContentLoaded(webBrowser) == false)
				{
					string fullHtml = ServiceHelper.MarkdownToHtmlConverter!.ConvertAll(markdown);
					webBrowser.LoadHtml(fullHtml);
					SetIsContentLoaded(webBrowser, true);
				}
				else
				{
					if (webBrowser.CanExecuteJavascriptInMainFrame)
					{
						string bodyHtml = ServiceHelper.MarkdownToHtmlConverter!.ConvertBodyOnly(markdown);
						string script = $"updateContent(`{bodyHtml}`);";

						JavascriptResponse result = await webBrowser.EvaluateScriptAsync(script);
					}
				}
			});
        }
	}
	private static async void OnContentCompleteChangedAsync(DependencyObject d,  DependencyPropertyChangedEventArgs e)
	{
		if (d is ChromiumWebBrowser webBrowser)
		{
			bool? isContentComplete = (bool?)e.NewValue;

			if (isContentComplete == false)
			{
				webBrowser.Height = 500;
			}
			else if (isContentComplete == true)
			{
				while (webBrowser.CanExecuteJavascriptInMainFrame == false)
				{
					await Task.Delay(25);
				}

				JavascriptResponse r2 = await webBrowser.EvaluateScriptAsync("contentComplete();");
				JavascriptResponse response = await webBrowser.EvaluateScriptAsync("document.getElementsByTagName('body')[0].scrollHeight");
				if (response.Success)
				{
                    int docHeight = (int)response.Result;

                    webBrowser.Dispatcher.Invoke(() =>
                    {
                        webBrowser.Height = docHeight + 10;
                    });
                }
            }
		}
	}
	#endregion
}
