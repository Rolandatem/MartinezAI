using CefSharp;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace MartinezAI.WPFApp.CefSharpTools;

public class ExternalLinkOpener : IRequestHandler
{
    public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
    {
        var url = request.Url;

        // Optionally, check if url is external
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            // Open in default browser
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            // Prevent navigation in CefSharp
            return true;
        }
        return false;
    }

    public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) => false;
    public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling) => null!;
    public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) => false;
    public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser) { }
    public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) => false;
    public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status, int errorCode, string errorMessage) { }
    public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser) { }
    public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) => false;
}
