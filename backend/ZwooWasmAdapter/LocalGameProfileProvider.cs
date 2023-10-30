using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Lobby.Features;

namespace ZwooWasm;

public class LocalGameProfile : IExternalGameProfile
{
    public string Id { get; set; }

    public string Name { get; set; }

    public GameProfile Settings { get; set; }

    public LocalGameProfile(string id, string name, GameProfile settings)
    {
        Id = id;
        Name = name;
        Settings = settings;
    }
}

[SupportedOSPlatform("browser")]
public partial class LocalGameProfileProvider : IExternalGameProfileProvider
{
    public static readonly LocalGameProfileProvider Instance = new LocalGameProfileProvider();


    #region Interface Implementation
    public void SaveConfig(long playerId, string name, GameProfile config)
    {
        _saveHandler(name, JsonSerializer.Serialize(config));
    }

    public void UpdateConfig(long playerId, string id, GameProfile config)
    {
        _updateHandler(id, JsonSerializer.Serialize(config));
    }

    public void DeleteConfig(long playerId, string id)
    {
        _deleteHandler(id);
    }

    public IEnumerable<IExternalGameProfile> GetConfigsOfPlayer(long playerId)
    {
        try
        {
            var rawData = _getProfiles();
            var profiles = JsonSerializer.Deserialize<List<LocalGameProfile>>(rawData);
            return profiles ?? new List<LocalGameProfile>();
        }
        catch
        {
            return new List<LocalGameProfile>();
        }
    }

    #endregion Interface Implementation

    #region Javascript Adaptation

    private Func<string> _getProfiles = () => { return "[]"; };

    [JSExport]
    public static void OnGetProfiles([JSMarshalAsAttribute<JSType.Function<JSType.String>>] Func<string> callback)
    {
        Instance._getProfiles = callback;
    }

    private Action<string, string> _saveHandler = (string name, string profile) => { };

    [JSExport]
    public static void OnSave([JSMarshalAsAttribute<JSType.Function<JSType.String, JSType.String>>] Action<string, string> callback)
    {
        Instance._saveHandler = callback;
    }

    private Action<string, string> _updateHandler = (string id, string profile) => { };

    [JSExport]
    public static void OnUpdate([JSMarshalAsAttribute<JSType.Function<JSType.String, JSType.String>>] Action<string, string> callback)
    {
        Instance._updateHandler = callback;
    }

    private Action<string> _deleteHandler = (string id) => { };

    [JSExport]
    public static void OnDelete([JSMarshalAsAttribute<JSType.Function<JSType.String>>] Action<string> callback)
    {
        Instance._deleteHandler = callback;
    }

    #endregion Javascript Adaptation
}