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
}

[System.Serializable]
public class Response
{
    public string DisplayMessage;
    public string NoticeDisplayMessage;


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
    public bool ChangeLeader;

    [Header("World Effects")]
    [Tooltip("Whole world's opinion change towards the sender")]
    public int WorldSenderOpinion;
    [Tooltip("Whole world's opinion change towards the receiver")]
    public int WorldReceiverOpinion;

}