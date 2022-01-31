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
        if (currentEvent.sender.previousLeader != null) message = message.Replace("SenderOldLeader", currentEvent.sender.previousLeader.Name);
        message = message.Replace("SenderLeader", currentEvent.sender.Leader.Name);
        if (currentEvent.receiver.previousLeader != null) message = message.Replace("ReceiverOldLeader", currentEvent.receiver.previousLeader.Name);
        message = message.Replace("ReceiverLeader", currentEvent.receiver.Leader.Name);
        message = message.Replace("Sender", currentEvent.sender.CountryName);
        message = message.Replace("Receiver", currentEvent.receiver.CountryName);
        if (currentEvent.affected != null)
        {
            message = message.Replace("AffectedLeader", currentEvent.affected.Leader.Name);
            message = message.Replace("Affected", currentEvent.affected.CountryName);
        }
        return message;
    }

    public static string ColorCountryText(Country country)
    {
        return "<#" + ColorUtility.ToHtmlStringRGB(country.textColor) + ">";
    }
    public static Vector3 RoundVector3(Vector3 vector3, float roundFactor)
    {
        vector3 /= roundFactor;
        vector3 = new Vector3(Mathf.Round(vector3.x), Mathf.Round(vector3.y), Mathf.Round(vector3.z));
        vector3 *= roundFactor;
        return vector3;
    }
    public static int WeightedRandom(List<int> ratio)
    {
        var RatioTotal = ratio.Sum();
        int random = Random.Range(0, RatioTotal + 1);
        // ratio.ForEach(x => print(x));
        // print("rand - " + random);
        int index = 0;
        foreach (int ratioElement in ratio)
        {
            if ((random -= ratioElement) <= 0) break; // Determines if the random falls in the range corresponding to ratio[index] 
            index++;
        }
        //print("index " + index);
        return index;/*
        switch (index)
        {
            case 0: return Likelihood.Highest;
            case 1: return Likelihood.High;
            case 2: return Likelihood.Middle;
            case 3: return Likelihood.Low;
            case 4: return Likelihood.Lower;
            case 5: return Likelihood.Lowest;
            default: return Likelihood.Highest;
        }
*/
    }
}

