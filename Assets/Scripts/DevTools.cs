using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DevTools : MonoBehaviour
{
    public static T RandomEnumValue<T>()
    {
        var v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(Random.Range(0, v.Length));
    }

    public static T RandomListValue<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static string DecodeMessage(string message, Event currentEvent)
    {
        message = message.Replace("SenderLeader", currentEvent.sender.LeaderName);
        message = message.Replace("ReceiverLeader", currentEvent.receiver.LeaderName);
        message = message.Replace("Sender", currentEvent.sender.CountryName);
        message = message.Replace("Receiver", currentEvent.receiver.CountryName);
        if (currentEvent.affected != null)
        {
            message = message.Replace("AffectedLeader", currentEvent.affected.LeaderName);
            message = message.Replace("Affected", currentEvent.affected.CountryName);
        }
        return message;
    }

    public static string ColorCountryText(Country country)
    {
        return "<#" + ColorUtility.ToHtmlStringRGB(country.textColor) + ">";
    }


}

