using Zwoo.GameEngine.Game.State;

namespace Zwoo.GameEngine.Lobby.Features;


public class SystemGameProfiles
{

    public static ExternalGameProfile DefaultFeatures = new ExternalGameProfile(
        "default", "rules.profiles.default", GameProfileGroup.System, new GameProfile(new Dictionary<string, int>() { })
        );

    public static ExternalGameProfile FullFeatureSet = new ExternalGameProfile(
        "full", "rules.profiles.full", GameProfileGroup.System, new GameProfile(new Dictionary<string, int>()
        {
            {"deckChange", 1},
        }));


    public static List<ExternalGameProfile> All = new List<ExternalGameProfile>()
    {
        DefaultFeatures,
        FullFeatureSet
    };

}