namespace Zwoo.Backend.Shared.Services;

/// <summary>
/// a service for parsing language query parameter
/// </summary>
public interface ILanguageService
{

    /// <summary>
    /// parse a language code from a query string
    /// </summary>
    /// <param name="query">the url query</param>
    /// <returns></returns>
    public LanguageCode ResolveFormQuery(string query);

    /// <summary>
    /// convert a language code into a string
    /// </summary>
    /// <param name="lng">the language code</param>
    /// <returns></returns>
    public string CodeToString(LanguageCode lng);
}

public class LanguageService : ILanguageService
{
    public LanguageCode ResolveFormQuery(string query)
    {
        string code = query.ToLower().PadLeft(2, ' ').Substring(0, 2);
        switch (code)
        {
            case "de": return LanguageCode.German;
            case "en": return LanguageCode.English;
            default: return LanguageCode.English;
        }
    }

    public string CodeToString(LanguageCode lng)
    {
        switch (lng)
        {
            case LanguageCode.German: return "de";
            case LanguageCode.English: return "en";
            default: return "en";
        }
    }
}


public enum LanguageCode
{
    English,
    German
}