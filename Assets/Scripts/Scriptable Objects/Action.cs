using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Action : ScriptableObject
{
    public string Name;
    [Tooltip("For when action is shown in an event slot against player")]
    public string PlayerDisplayMessage;
    public List<Response> Responses;
    public Sprite NoticeSymbol;
    public int Cooldown;

    [Header("Requirements")]
    public int MinRelation;
    public int MaxRelation;
    public int MinMoney;
    public int MinWarPower;

    [Header("AI Settings")]
    public List<Focus> FittingFocuses;
    public List<Focus> NonfittingFocuses;
    public Likelihood FittingFocusChance;
    public Likelihood NeutralChance;
    public Likelihood NonfittingFocusChance;
}

[System.Serializable]
public class Response
{
    public string DisplayMessage;
    public string NoticeDisplayMessage;

    [Header("Requirements")]
    public int MinRelation;
    public int MaxRelation;
    public int MinMoney;
    public int MinWarPower;

    [Header("AI Settings")]
    public Focus[] FittingFocuses;
    public Likelihood FittingFocusChance;

    public bool RequireStrongerSender;

    [Header("Sender Effects")]
    [Tooltip("The sender's opinion change towards the receiver")]
    public int SenderOpinion;
    public int SenderMoney;
    public int SenderWarPower;

    [Header("Receiver Effects")]
    [Tooltip("The receiver's opinion change towards the sender")]
    public int ReceiverOpinion;
    public int ReceiverMoney;
    public int ReceiverWarPower;

    [Header("World Effects")]
    [Tooltip("Whole world's opinion change towards the sender")]
    public int WorldSenderOpinion;
    [Tooltip("Whole world's opinion change towards the receiver")]
    public int WorldReceiverOpinion;

}

public enum Likelihood
{
    Lowest,
    Lower,
    Low,
    Middle,
    High,
    Highest
}