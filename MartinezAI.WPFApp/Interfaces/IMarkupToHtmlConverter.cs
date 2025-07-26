namespace MartinezAI.WPFApp.Interfaces;

public interface IMarkupToHtmlConverter
{
    string ConvertAll(string markup);
    string ConvertBodyOnly(string markup);
}
