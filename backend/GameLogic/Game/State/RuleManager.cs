using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwooGameLogic.Game.Rules;
using ZwooGameLogic.Game.State;
using ZwooGameLogic.Game.Events;
using ZwooGameLogic.Game.Settings;
using log4net;

namespace ZwooGameLogic.Game.State;

internal class RuleManager
{
    public static List<BaseRule> AllRules = new List<BaseRule>() { new BaseCardRule(), new BaseDrawRule(), new BaseWildCardRule(), new SkipCardRule() };

    public readonly long GameId;

    private readonly GameSettings _settings;
    private List<BaseRule> _activeRules;

    public RuleManager(long gameId, GameSettings settings)
    {
        GameId = gameId;
        _settings = settings;
        _activeRules = new List<BaseRule>();
    }

    public void Configure()
    {
        _activeRules = AllRules
            .Where(rule =>
            {
                Console.WriteLine(rule.AssociatedOption == GameSettingsKey.DEFAULT_RULE_SET);
                return rule.AssociatedOption == GameSettingsKey.DEFAULT_RULE_SET || _settings.Get(rule.AssociatedOption) > 0;
            })
            .ToList();

        foreach (var rule in _activeRules)
        {
            // TODO: Fix log management
            rule.SetLogger(LogManager.GetLogger($"[Game-{GameId}]"));
        }
    }

    public List<BaseRule> GetResponsibleRules(ClientEvent clientEvent, GameState state)
    {
        return _activeRules.Where(rule => rule.IsResponsible(clientEvent, state)).ToList();
    }

    public BaseRule GetPrioritiesedRule(List<BaseRule> rules)
    {
        return rules.OrderByDescending(rule => rule.Priority).First();
    }

    public BaseRule? getRule(ClientEvent clientEvent, GameState state)
    {
        return GetPrioritiesedRule(GetResponsibleRules(clientEvent, state));
    }
}
