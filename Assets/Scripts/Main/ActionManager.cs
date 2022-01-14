using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{

    public Event CurrentEvent = new Event();

    [SerializeField]
    private List<Action> actions = new List<Action>();
    public static List<Action> s_actions = new List<Action>();

    public static Action s_hoveredAction;

    public static List<Focus> s_Focuses = new List<Focus>();
    public List<Focus> Focuses;

    public static List<PersonalityType> s_PersonalityTypes;
    public List<PersonalityType> PersonalityTypes;

    private Main main;

    [HideInInspector]
    public bool actionTaken; // if an action has already been taken by the player this turn

    [SerializeField]
    private GameObject go_CategoryButtons;

    [SerializeField]
    private GameObject[] go_Categories;

    void Awake()
    {
        s_Focuses = Focuses;
        s_PersonalityTypes = PersonalityTypes;
    }

    private void Start()
    {
        main = GetComponent<Main>();
        s_actions = new List<Action>(actions);
        Main.s_TurnActions += SetCountrySlotButtonsInteractable;
        Main.s_TurnActions += DeselectCurrentCountrySlot;
    }

    public void RunAction(Event currentEvent)
    {
        CurrentEvent = currentEvent;
        //currentResponses = CurrentAction.Responses.OfType<Response>().ToList(); // Convert response array to list    

    }

    public void SelectCountrySlot(CountrySlot CountrySlot)
    {
        if (actionTaken)
        {
            print("Action already made this turn. Cannot select another country.");
            return;
        }
        CountrySlot oldSelected = (CurrentEvent.receiver == null || CurrentEvent.receiver.IsPlayerCountry) ? null : main.cs_NonPlayers[CurrentEvent.receiver.ID];
        bool diffAsLast = (oldSelected != CountrySlot); // if button clicked was NOT the same as previously selected country       

        if (oldSelected != null) oldSelected.SetButtonSelected(false);
        CountrySlot.SetButtonSelected(diffAsLast);
        CurrentEvent.receiver = (diffAsLast) ? main.cnt_Players[System.Array.IndexOf(main.cs_NonPlayers, CountrySlot)] : null; // sets receiver to corresponsing country of clicked country slot
        main.SetActionButtonsEnabled(diffAsLast);
    }

    public void CountrySlotUnhover(CountrySlot CountrySlot)
    {
        CountrySlot.SetCurrentlyHovered(false);
        CountrySlot.ResetButtonHighlightedColor();
    }
    public void CountrySlotHover(CountrySlot CountrySlot)
    {
        CountrySlot.SetCurrentlyHovered(true);
    }

    public void ActionHover(Action action)
    {
        s_hoveredAction = action;
    }

    public void ReturnToCategories()
    {
        go_CategoryButtons.SetActive(true);
        foreach (GameObject obj in go_Categories) obj.SetActive(false);
    }

    public void DeselectCurrentCountrySlot()
    {
        if (CurrentEvent.receiver != null && CurrentEvent.receiver.IsPlayerCountry) SelectCountrySlot(main.cs_NonPlayers[CurrentEvent.receiver.ID]);
    }

    public void RunPlayerAction(Action action)
    {
        var sender = CurrentEvent.sender;
        var receiver = CurrentEvent.receiver;
        if (main.cnt_Player.ActionCooldowns.Keys.Contains(action))
        {
            return;
        }
        RunAction(new Event(action, main.cnt_Player, CurrentEvent.receiver, null));
        RunResponse(AIManager.BestResponse(CurrentEvent, main.cnt_Player));
        DeselectCurrentCountrySlot();
        SetCountrySlotButtonsUninteractable();
        ReturnToCategories();
        actionTaken = true;
    }

    public void SetCountrySlotButtonsInteractable()
    {
        foreach (CountrySlot cs in main.cs_NonPlayers) cs.SetButtonInteractable(true);
    }
    public void SetCountrySlotButtonsUninteractable()
    {
        foreach (CountrySlot cs in main.cs_NonPlayers) cs.SetButtonInteractable(false);
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

        if (!CurrentEvent.receiver.IsPlayerCountry || true)
            main.ShowNotice(CurrentEvent, Response);
    }
}