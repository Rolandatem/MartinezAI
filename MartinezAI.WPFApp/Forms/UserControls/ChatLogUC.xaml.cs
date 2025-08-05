using CefSharp;
using CefSharp.Wpf;
using MartinezAI.WPFApp.CefSharpTools;
using System.Windows;
using System.Windows.Controls;

namespace MartinezAI.WPFApp.Forms.UserControls;

/// <summary>
/// Interaction logic for ChatLogUC.xaml
/// </summary>
public partial class ChatLogUC : UserControl
{
    public ChatLogUC()
    {
        InitializeComponent();
    }

    private async void ChromiumWebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
    {
        if (e.IsLoading == false && sender is ChromiumWebBrowser webBrowser)
        {
            JavascriptResponse response = await webBrowser.EvaluateScriptAsync("document.documentElement.scrollHeight");
            int docHeight = (int)response.Result;

            Application.Current.Dispatcher.Invoke(() =>
            {
                webBrowser.Height = docHeight + 10;
            });
        }
    }

    private void ChromiumWebBrowser_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is ChromiumWebBrowser webBrowser)
        {
            webBrowser.RequestHandler = new ExternalLinkOpener();
        }
    }

    private void DevTools_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) { return; }

        //--Get parent stack panel
        Grid? grid = VisualTreeUtilities.FindAncestor<Grid>(button);
        if (grid == null) { return; }

        //--Find ChromiumWebBrowser child of stack panel.
        var webBrowser = VisualTreeUtilities.FindChild<ChromiumWebBrowser>(grid);
        if (webBrowser == null) { return; }

        webBrowser.GetBrowser().GetHost().ShowDevTools();
    }
}
