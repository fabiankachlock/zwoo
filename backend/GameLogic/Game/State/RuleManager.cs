using ZwooGameLogic.Game.Rules;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using ZwooGameLogic.Logging;

namespace ZwooGameLogic.Game.State;

internal class RuleManager
{
    public static List<BaseRule> AllRules() => new List<BaseRule>() {
        new BaseCardRule(),
        new BaseDrawRule(),
        new BaseWildCardRule(),
        new SkipCardRule(),
        new ReverseCardRule(),
        new AddUpDrawRule(),
        new LastCardRule()
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

    public void Configure(Action<GameInterrupt> interruptHandler)
    {
        _activeRules = AllRules()
            .Where(rule =>
            {
                if (rule.Setting == null) return true;
                return _settings.Get(rule.Setting.Value.SettingsKey) > 0;
            })
            .ToList();

        foreach (var rule in _activeRules)
        {
            rule.SetupRule(interruptHandler, _loggerFactory.CreateLogger($"Game-{GameId}"));
        }
    }

    public void OnGameUpdate(GameState state, List<GameEvent> outgoingEvents)
    {
        _activeRules.ForEach(r => r.OnGameEvent(state, outgoingEvents));
    }

    public List<BaseRule> GetResponsibleRules(ClientEvent clientEvent, GameState state)
    {
        return _activeRules.Where(rule => rule.IsResponsible(clientEvent, state)).ToList();
    }

    public List<BaseRule> GetResponsibleRulesForInterrupt(GameInterrupt interrupt, GameState state)
    {
        return _activeRules.Where(rule => rule.IsResponsibleForInterrupt(interrupt, state)).ToList();
    }

    public BaseRule? GetPrioritizedRule(List<BaseRule> rules)
    {
        return rules.OrderByDescending(rule => rule.Priority).FirstOrDefault();
    }

    public BaseRule? GetRule(ClientEvent clientEvent, GameState state)
    {
        return GetPrioritizedRule(GetResponsibleRules(clientEvent, state));
    }

    public BaseRule? GetRuleForInterrupt(GameInterrupt interrupt, GameState state)
    {
        return GetPrioritizedRule(GetResponsibleRulesForInterrupt(interrupt, state));
    }
}
