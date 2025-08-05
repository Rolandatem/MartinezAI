namespace MartinezAI.WPFApp.Interfaces;

public interface IMarkdownToHtmlConverter
{
    string ConvertAll(string markup);
    string ConvertBodyOnly(string markup);
}
