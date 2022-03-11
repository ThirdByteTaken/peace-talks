using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIManager : MonoBehaviour
{

    // Determines the receivers response to actions
    static readonly List<int> LikelihoodRatio = new List<int> { 1, 2, 4, 8, 16, 32 };
    public static Response BestResponse(Event ce, Country sender)
    {
        ResetResponseDictionary();
        var relation = sender.Relations[ce.receiver].Value;

        List<Response> AllPossibleResponses = new List<Response>();
        foreach (Response response in ce.action.Responses)
        {
            if (relation < response.MinRelation || relation > response.MaxRelation) continue;
            if (ce.receiver.Money < response.MinMoney) continue; // If the person being asked for a loan doesn't have enough money
            if (ce.receiver.WarPower < response.MinWarPower) continue;
            if (response.RequireStrongerSender && ce.sender.WarPower < ce.receiver.WarPower) continue;


            if (response.FittingFocuses.Contains(ce.receiver.Focus))
                bestResponses[response.FittingFocusChance].Add(response);
            else
                bestResponses[Likelihood.Middle].Add(response); // Add response to be moderately likely

            AllPossibleResponses.Add(response);
        }

        Likelihood likelihood = (Likelihood)(DevTools.WeightedRandom(LikelihoodRatio));
        while (bestResponses[likelihood].Count == 0 && likelihood != Likelihood.Highest) likelihood = (Likelihood)((int)likelihood + 1);
        if (bestResponses[likelihood].Count == 0) return DevTools.RandomListValue(AllPossibleResponses); // Return a random response
        return DevTools.RandomListValue(bestResponses[likelihood]);
    }

    public static Action BestAction(Country sender, Country receiver)
    {
        ResetActionDictionary();
        var relation = sender.Relations[receiver].Value;


        foreach (Action action in ActionManager.Instance.actions)
        {
            if (relation < action.MinRelation || relation > action.MaxRelation) continue;
            if (sender.Money < action.MinMoney) continue; // If the person being asked for a loan doesn't have enough money
            if (sender.WarPower < action.MinWarPower) continue;
            if (sender.ActionCooldowns.ContainsKey(action)) continue;

            if (action.FittingFocuses.Contains(sender.Focus)) { bestActions[action.FittingFocusChance].Add(action); }
            else if (action.NonfittingFocuses.Contains(sender.Focus)) bestActions[action.NonfittingFocusChance].Add(action);
            else bestActions[action.FittingFocusChance].Add(action);

        }
        Likelihood likelihood = (Likelihood)(DevTools.WeightedRandom(LikelihoodRatio));
        while (bestActions[likelihood].Count == 0 && likelihood != Likelihood.Highest) likelihood = (Likelihood)((int)likelihood + 1);
        if (bestActions[likelihood].Count == 0) return null;
        return DevTools.RandomListValue(bestActions[likelihood]);
    }

    public static Country BestCountry(Country sender)
    {
        return (sender.cnt_RecentlyInteracted.Count == 0) ? null : DevTools.RandomListValue(sender.cnt_RecentlyInteracted);
    }

    private static Dictionary<Likelihood, List<Action>> bestActions = new Dictionary<Likelihood, List<Action>>();
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

    private static Dictionary<Likelihood, List<Response>> bestResponses = new Dictionary<Likelihood, List<Response>>();
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




}
