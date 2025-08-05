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
						string script = $@"
							var contentDiv = document.getElementById('content-div');
							if (contentDiv) {{
								contentDiv.innerHTML = '{bodyHtml.Replace("'", "\\'").Replace(Environment.NewLine, "")}';
							}}";

						await webBrowser.EvaluateScriptAsync(script);
					}
				}
			});
        }
	}
	#endregion
}
