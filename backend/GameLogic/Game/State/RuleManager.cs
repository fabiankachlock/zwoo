using ZwooGameLogic.Game.Rules;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Game.State;

internal class RuleManager
{
    public static List<BaseRule> AllRules = new List<BaseRule>() {
        new BaseCardRule(),
        new BaseDrawRule(),
        new BaseWildCardRule(),
        new SkipCardRule(),
        new ReverseCardRule(),
        new AddUpDrawRule()
    };

    public readonly long GameId;

    private readonly ILoggerFactory _loggerFactory;
    private readonly GameSettings _settings;
    private List<BaseRule> _activeRules;

    public RuleManager(long gameId, GameSettings settings, ILoggerFactory loggerFactory)
    {
        GameId = gameId;
        _settings = settings;
        _activeRules = new List<BaseRule>();
        _loggerFactory = loggerFactory;
    }

    public void Configure()
    {
        _activeRules = AllRules
            .Where(rule =>
            {
                return rule.AssociatedOption == GameSettingsKey.DEFAULT_RULE_SET || _settings.Get(rule.AssociatedOption) > 0;
            })
            .ToList();

        foreach (var rule in _activeRules)
        {
            rule.SetLogger(_loggerFactory.CreateLogger($"Game-{GameId}"));
        }
    }

    public List<BaseRule> GetResponsibleRules(ClientEvent clientEvent, GameState state)
    {
        return _activeRules.Where(rule => rule.IsResponsible(clientEvent, state)).ToList();
    }

    public BaseRule GetPrioritizedRule(List<BaseRule> rules)
    {
        return rules.OrderByDescending(rule => rule.Priority).First();
    }

    public BaseRule? getRule(ClientEvent clientEvent, GameState state)
    {
        return GetPrioritizedRule(GetResponsibleRules(clientEvent, state));
    }
}
