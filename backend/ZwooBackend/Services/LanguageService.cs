namespace ZwooBackend.Services;

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
}


public enum LanguageCode
{
    English,
    German,
}