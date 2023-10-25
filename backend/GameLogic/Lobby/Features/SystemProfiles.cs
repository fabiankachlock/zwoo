using ZwooGameLogic.Game.State;

namespace ZwooGameLogic.Lobby.Features;


public class SystemGameProfiles
{

    public static ExternalGameProfile FullFeatureSet = new ExternalGameProfile(
        "full", "All features", GameProfileGroup.System, new GameProfile(new Dictionary<string, int>()
        {
            {"deckChange", 1},
        }));

    public static List<ExternalGameProfile> All = new List<ExternalGameProfile>()
    {
        FullFeatureSet
    };

}