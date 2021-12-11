using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    // Determines the receivers response to actions
    public static Response BestResponse(Event ce, Country sender)
    {
        List<Response> PossibleResponses = new List<Response>();
        foreach (Response Response in ce.action.Responses)
        {
            var Relation = sender.Relations[ce.receiver.ID].Value;

            if (Relation <= Response.MinRelation || Relation >= Response.MaxRelation) continue;
            if (ce.receiver.Money < Response.MinMoney) continue; // If the person being asked for a loan doesn't have enough money
            if (ce.receiver.WarPower < Response.MinWarPower) continue;
            if (Response.RequireStrongerSender && ce.sender.WarPower < ce.receiver.WarPower) continue;

            PossibleResponses.Add(Response);
        }
        return DevTools.RandomListValue(PossibleResponses);
    }
}
