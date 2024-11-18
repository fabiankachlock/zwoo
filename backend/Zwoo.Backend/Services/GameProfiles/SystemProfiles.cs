namespace Zwoo.Backend.Services.GameProfiles;


public class SystemGameProfiles
{

    public static GameProfile DefaultFeatures = new (
        "default", "rules.profiles.default", GameProfileGroup.System, new Dictionary<string, int>());

    public static GameProfile FullFeatureSet = new (
        "full", "rules.profiles.full", GameProfileGroup.System, new Dictionary<string, int>()
        {
            {"deckChange", 1},
        });


    public static List<GameProfile> All =
    [
        DefaultFeatures,
        FullFeatureSet
    ];

}