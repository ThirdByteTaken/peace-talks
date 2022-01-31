using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{

    public static ActionManager Instance;
    public Event CurrentEvent = new Event();

    public List<Action> actions = new List<Action>();


    public List<Focus> Focuses;
    public List<PersonalityType> PersonalityTypes;

    private Main main;
    private UIManager uIManager;

    [HideInInspector]
    public bool actionTaken; // if an action has already been taken by the player this turn





    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        main = Main.Instance;
        uIManager = UIManager.Instance;
    }

    public void SetCurrentAction(Event currentEvent)
    {
        CurrentEvent = currentEvent;
    }

    public void RunPlayerAction(Action action)
    {
        var sender = CurrentEvent.sender;
        var receiver = CurrentEvent.receiver;
        if (main.cnt_Player.ActionCooldowns.Keys.Contains(action))
        {
            return;
        }
        SetCurrentAction(new Event(action, main.cnt_Player, CurrentEvent.receiver, null));
        RunResponse(AIManager.BestResponse(CurrentEvent, main.cnt_Player));
        uIManager.DeselectCurrentCountrySlot();
        uIManager.SetCountrySlotButtonsUninteractable();
        uIManager.ReturnToCategories();
        actionTaken = true;
    }

    public void RunResponse(int index)
    { RunResponse(CurrentEvent.action.Responses[index]); }

    public void RunResponse(Response Response)
    {
        CurrentEvent.receiver.cnt_RecentlyInteracted.Add(CurrentEvent.sender);

        CurrentEvent.sender.Relations[CurrentEvent.receiver].Value += Response.SenderOpinion;
        CurrentEvent.receiver.Relations[CurrentEvent.sender].Value += Response.ReceiverOpinion;
        foreach (Country country in main.cnt_Players)
        {
            if (country != CurrentEvent.sender)
                country.Relations[CurrentEvent.sender].Value += Response.WorldSenderOpinion;
            if (country != CurrentEvent.receiver)
                country.Relations[CurrentEvent.receiver].Value += Response.WorldReceiverOpinion;
        }

        CurrentEvent.sender.ActionCooldowns.Add(CurrentEvent.action, CurrentEvent.action.Cooldown);
        CurrentEvent.sender.Money += Response.SenderMoney;
        CurrentEvent.sender.WarPower += Response.SenderWarPower;
        CurrentEvent.receiver.Money += Response.ReceiverMoney;
        CurrentEvent.receiver.WarPower += Response.ReceiverWarPower;

        if (Response.ChangeLeader)
            CurrentEvent.receiver.ChangeLeader(CurrentEvent.sender); // replaces the recievers leader with one similar to the senders country


        main.UpdateCountrySlots();

        if (!CurrentEvent.receiver.IsPlayer || true)
            main.ShowNotice(CurrentEvent, Response);
    }
}