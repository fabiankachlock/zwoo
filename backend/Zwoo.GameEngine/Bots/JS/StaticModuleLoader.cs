using Microsoft.ClearScript;

namespace Zwoo.GameEngine.Bots.JS;

/// <summary>
/// A custom DocumentLoader that loads predefined modules. 
/// </summary>
internal class StaticModuleLoader : DocumentLoader
{
    private readonly Dictionary<string, string> _modules;

    public StaticModuleLoader(Dictionary<string, string> modules)
    {
        _modules = modules;
    }

    public override Task<Document> LoadDocumentAsync(DocumentSettings settings, DocumentInfo? sourceInfo, string specifier, DocumentCategory category, DocumentContextCallback contextCallback)
    {
        if (_modules.TryGetValue(specifier, out var content))
        {
            return Task.FromResult<Document>(
                new StringDocument(
                    new DocumentInfo(specifier)
                    {
                        Category = category,
                        ContextCallback = contextCallback
                    },
                    content
                )
            );
        }

        throw new ModuleNotResolvedException($"Module '{specifier}' not found in predefined modules.");
    }
}

/// <summary>
/// Exception thrown when a module is not found in the predefined modules.
/// </summary>
internal class ModuleNotResolvedException : Exception
{
    public ModuleNotResolvedException(string message) : base(message) { }
}