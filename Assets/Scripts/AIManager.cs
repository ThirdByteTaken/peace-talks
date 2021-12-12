using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIManager : MonoBehaviour
{

    // Determines the receivers response to actions
    public static Response BestResponse(Event ce, Country sender)
    {
        ResetResponseDictionary();
        var relation = sender.Relations[ce.receiver.ID].Value;

        List<Response> PossibleResponses = new List<Response>();
        foreach (Response Response in ce.action.Responses)
        {

            if (relation < Response.MinRelation || relation > Response.MaxRelation) continue;
            if (ce.receiver.Money < Response.MinMoney) continue; // If the person being asked for a loan doesn't have enough money
            if (ce.receiver.WarPower < Response.MinWarPower) continue;
            if (Response.RequireStrongerSender && ce.sender.WarPower < ce.receiver.WarPower) continue;

            PossibleResponses.Add(Response);
        }
        return DevTools.RandomListValue(PossibleResponses);
    }

    public static Action BestAction(Country sender)
    {
        ResetActionDictionary();
        Country receiver = new Country();
        var relation = sender.Relations[receiver.ID].Value;
        foreach (Action action in ActionManager.s_actions)
        {
            if (relation < action.MinRelation || relation > action.MaxRelation) continue;
            if (sender.Money < action.MinMoney) continue; // If the person being asked for a loan doesn't have enough money
            if (sender.WarPower < action.MinWarPower) continue;

            if (action.FittingFocuses.Contains(sender.LeaderFocus)) bestActions[action.FittingFocusChance].Add(action);
            else if (action.NonfittingFocuses.Contains(sender.LeaderFocus)) bestActions[action.NonfittingFocusChance].Add(action);
            else bestActions[action.FittingFocusChance].Add(action);
        }
        return null;
    }

    private static Dictionary<Likelihood, List<Action>> bestActions;
    static void ResetActionDictionary()
    {
        bestActions.Clear();
        bestActions.Add(Likelihood.Lowest, new List<Action>());
        bestActions.Add(Likelihood.Lower, new List<Action>());
        bestActions.Add(Likelihood.Low, new List<Action>());
        bestActions.Add(Likelihood.Middle, new List<Action>());
        bestActions.Add(Likelihood.High, new List<Action>());
        bestActions.Add(Likelihood.Highest, new List<Action>());
    }

    private static Dictionary<Likelihood, List<Response>> bestResponses;
    static void ResetResponseDictionary()
    {
        bestResponses.Clear();
        bestResponses.Add(Likelihood.Lowest, new List<Response>());
        bestResponses.Add(Likelihood.Lower, new List<Response>());
        bestResponses.Add(Likelihood.Low, new List<Response>());
        bestResponses.Add(Likelihood.Middle, new List<Response>());
        bestResponses.Add(Likelihood.High, new List<Response>());
        bestResponses.Add(Likelihood.Highest, new List<Response>());
    }

    readonly List<int> LikelihoodRatio = new List<int> { 32, 16, 8, 4, 2, 1 };
    public Likelihood WeightedRandomLikelihood()
    {
        var TotalRatio = LikelihoodRatio.Sum();
        int rand = Random.Range(0, TotalRatio + 1);
        int iteration = 0;
        foreach (int x in LikelihoodRatio)
        {
            if ((rand -= x) < 0) break;
            iteration++;
        }
        switch (iteration)
        {
            case 0: return Likelihood.Highest;
            case 1: return Likelihood.High;
            case 2: return Likelihood.Middle;
            case 3: return Likelihood.Low;
            case 4: return Likelihood.Lower;
            case 5: return Likelihood.Lowest;
            default: return Likelihood.Highest;
        }

    }

}
