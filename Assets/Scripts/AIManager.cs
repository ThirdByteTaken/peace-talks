using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    // Determines the receivers response to actions
    public static Response BestResponse(Event ce, Country sender)
    {
        ResetResponseDictionary();
        var Relation = sender.Relations[ce.receiver.ID].Value;

        List<Response> PossibleResponses = new List<Response>();
        foreach (Response Response in ce.action.Responses)
        {

            if (Relation < Response.MinRelation || Relation > Response.MaxRelation) continue;
            if (ce.receiver.Money < Response.MinMoney) continue; // If the person being asked for a loan doesn't have enough money
            if (ce.receiver.WarPower < Response.MinWarPower) continue;
            if (Response.RequireStrongerSender && ce.sender.WarPower < ce.receiver.WarPower) continue;

            PossibleResponses.Add(Response);
        }
        return DevTools.RandomListValue(PossibleResponses);
    }

    public static Action BestAction(Country country)
    {
        ResetActionDictionary();
        foreach (Action action in ActionManager.s_actions)
        {
            if (action.FittingFocuses.Contains(country.LeaderFocus)) bestActions[action.FittingFocusChance].Add(action);
            else if (action.NonfittingFocuses.Contains(country.LeaderFocus)) bestActions[action.NonfittingFocusChance].Add(action);
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

}
