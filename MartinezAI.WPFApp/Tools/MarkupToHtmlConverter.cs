using Markdig;
using MartinezAI.WPFApp.Interfaces;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace MartinezAI.WPFApp.Tools;

internal class MarkupToHtmlConverter : IMarkupToHtmlConverter
{
    #region "Private Methods"
    private string BrushToHexString(SolidColorBrush brush)
    {
        Color color = brush.Color;
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
    private string StripOuterCodeFences(string text)
    {
        if (text.StartsWith("```") && text.TrimEnd().EndsWith("```"))
        {
            int firstNewline = text.IndexOf('\n');
            int lastFence = text.LastIndexOf("```");
            if (firstNewline >= 0 && lastFence > firstNewline)
            {
                return text.Substring(firstNewline + 1, lastFence - firstNewline - 1).Trim();
            }
        }
        return text;
    }
    private string AddLineNumbersClassToPre(string html)
    {
        // Adds line-numbers class to all <pre> tags (including ones that may have other classes)
        return Regex.Replace(
            html,
            @"<pre(\s+class=""[^""]*"")?",
            m =>
            {
                if (m.Groups[1].Success)
                    return m.Value.Insert(m.Value.IndexOf("class=\"") + 7, "line-numbers ");
                else
                    return "<pre class=\"line-numbers\"";
            },
            RegexOptions.IgnoreCase | RegexOptions.Multiline
        );
    }
    private string AddCopyButtons(string html)
    {
        // Add a copy button before every <pre ...><code ...>...</code></pre>
        return Regex.Replace(
            html,
            @"(<pre[^>]*>\s*<code[^>]*>)",
            m => $"<div class=\"code-container\"><button class=\"copy-btn\" onclick=\"copyCode(this)\">Copy</button>{m.Groups[1].Value}",
            RegexOptions.IgnoreCase);
    }
    #endregion

    #region "Private Methods"
    private string BuildWrapperHtml(string bodyHtml)
    {
        //--Wrap with HTML template
        string background = this.BrushToHexString((SolidColorBrush)
            Application.Current.Resources["DarkBrush"]);
        string foreground = this.BrushToHexString((SolidColorBrush)
            Application.Current.Resources["MainFontForegroundBrush"]);
        string htmlWrapper = $@"
        <!DOCTYPE html>
        <html>
            <head>
                <meta charset='utf-8'>
                <meta name=""viewport"" content=""width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"">
                <link href=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/themes/prism-tomorrow.min.css"" rel=""stylesheet""/>
                <link href=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/plugins/line-numbers/prism-line-numbers.min.css"" rel=""stylesheet""/>
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/components/prism-core.min.js""></script>
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/plugins/line-numbers/prism-line-numbers.min.js""></script>
                <!-- Add support for specific languages you want: -->
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/components/prism-markup.min.js""></script>
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/components/prism-clike.min.js""></script>
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/components/prism-javascript.min.js""></script>
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/components/prism-csharp.min.js""></script>
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/components/prism-python.min.js""></script>
                <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.30.0/components/prism-css.min.js""></script>
                <style>
                    html, body {{
                        overflow: hidden;
                    }}
                    body {{
                        color: {foreground};
                        background-color: {background};
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    }}
                    .prism-line-numbers .line-numbers-rows {{
                        background: #23243A;
                    }}
                    .code-container {{
                        position: relative;
                    }}
                    .copy-btn {{
                        position: absolute;
                        top: 5px;
                        right: 5px;
                        padding: 2px 8px;
                        font-size: 12px;
                        background: #444;
                        color: #fff;
                        border: none;
                        border-radius: 4px;
                        cursor: pointer;
                        opacity: 0.7;
                        z-index: 10;
                    }}
                    .copy-btn:hover {{
                        opacity: 1;
                        background: #222;
                    }}
                    #content-div {{
                        box-sizing: border-box;
                    }}
                </style>
            </head>
            <body>
                <div id='content-div'>{bodyHtml}</div>

                <script>
                    function copyCode(btn) {{
                        var code = btn.nextElementSibling.querySelector('code') || btn.nextElementSibling;
                        if (code) {{
                            var text = code.innerText || code.textContent;
                            var textarea = document.createElement('textarea');
                            textarea.value = text;
                            document.body.appendChild(textarea);
                            textarea.select();
                            try {{
                                document.execCommand('copy');
                                btn.textContent = 'Copied!';
                            }} catch (e) {{
                                btn.textContent = 'Error';
                            }}
                            setTimeout(() => btn.textContent = 'Copy', 1500);
                            document.body.removeChild(textarea);
                        }}
                    }}
                </script>
            </body>
        </html>";

        return htmlWrapper;
    }
    #endregion

    #region "Public Methods"
    public string ConvertAll(string markup)
    {
        string bodyHtml = ConvertBodyOnly(markup);
        return BuildWrapperHtml(bodyHtml);
    }
    public string ConvertBodyOnly(string markup)
    {
        //--Strip outer code fences from Open AI markdown if supplied.
        string html = StripOuterCodeFences(markup);

        //--Convert markdown to HTML using Markdig
        MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseEmojiAndSmiley()
            .Build();
        html = Markdown.ToHtml(markup, pipeline);

        //--Add line-numbers class to <pre>
        html = AddLineNumbersClassToPre(html);

        //--Add copy button to code blocks
        html = AddCopyButtons(html);

        return html;
    }
    #endregion
}
