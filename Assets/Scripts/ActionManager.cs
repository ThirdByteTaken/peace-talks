using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{

    public Event CurrentEvent = new Event();

    public List<Action> actions = new List<Action>();

    public List<Focus> focuses = new List<Focus>();

    private Main main;

    private void Start()
    {
        main = GetComponent<Main>();
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
        if (main.actionTaken)
        {
            print("Action already made this turn. Cannot select another country.");
            return;
        }
        CountrySlot oldSelected = (CurrentEvent.receiver == null || CurrentEvent.receiver == main.cnt_Player) ? null : main.cs_NonPlayers[CurrentEvent.receiver.ID];
        bool diffAsLast = (oldSelected != CountrySlot); // if button clicked was NOT the same as previously selected country       

        if (oldSelected != null) oldSelected.SetButtonSelected(false);
        CountrySlot.SetButtonSelected(diffAsLast);
        CurrentEvent.receiver = (diffAsLast) ? main.cnt_NonPlayers[System.Array.IndexOf(main.cs_NonPlayers, CountrySlot)] : null; // sets receiver to corrsponsing country of clicked country slot
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

    public void DeselectCurrentCountrySlot()
    {
        if (CurrentEvent.receiver != null && CurrentEvent.receiver.ID != -1) SelectCountrySlot(main.cs_NonPlayers[CurrentEvent.receiver.ID]);
    }

    public void RunPlayerAction(Action action)
    {
        RunAction(new Event(action, main.cnt_Player, CurrentEvent.receiver, null));
        RunResponse(0);
        DeselectCurrentCountrySlot();
        SetCountrySlotButtonsUninteractable();
        main.actionTaken = true;
    }

    public void SetCountrySlotButtonsInteractable()
    {
        foreach (CountrySlot cs in main.cs_NonPlayers) cs.SetButtonInteractable(true);
    }
    public void SetCountrySlotButtonsUninteractable()
    {
        foreach (CountrySlot cs in main.cs_NonPlayers) cs.SetButtonInteractable(false);
    }
    public void RunResponse(int Index)
    {
        Response Response = CurrentEvent.action.Responses[Index];
        if (CurrentEvent.receiver.IsPlayerCountry) // Receiver is player
        {
            CurrentEvent.sender.PlayerRelations.Value += Response.SenderOpinion;
            CurrentEvent.receiver.Relations[CurrentEvent.sender.ID].Value += Response.ReceiverOpinion;
            foreach (Country country in main.cnt_NonPlayers)
            {
                country.PlayerRelations.Value += Response.WorldReceiverOpinion;
                if (country != CurrentEvent.sender)
                    country.Relations[CurrentEvent.sender.ID].Value += Response.WorldSenderOpinion;
            }
        }
        else if (CurrentEvent.sender.IsPlayerCountry) // Sender is player
        {
            CurrentEvent.sender.Relations[CurrentEvent.receiver.ID].Value += Response.SenderOpinion;
            CurrentEvent.receiver.PlayerRelations.Value += Response.ReceiverOpinion;
            foreach (Country country in main.cnt_NonPlayers)
            {
                country.PlayerRelations.Value += Response.WorldSenderOpinion;
                if (country != CurrentEvent.receiver)
                    country.Relations[CurrentEvent.receiver.ID].Value += Response.WorldReceiverOpinion;
            }
        }
        else // No player involvement
        {

            CurrentEvent.sender.Relations[CurrentEvent.receiver.ID].Value += Response.SenderOpinion;
            CurrentEvent.receiver.Relations[CurrentEvent.sender.ID].Value += Response.ReceiverOpinion;
            foreach (Country country in main.cnt_NonPlayers)
            {
                if (country != CurrentEvent.sender)
                    country.Relations[CurrentEvent.sender.ID].Value += Response.WorldSenderOpinion;
                if (country != CurrentEvent.receiver)
                    country.Relations[CurrentEvent.receiver.ID].Value += Response.WorldReceiverOpinion;
            }
        }

        CurrentEvent.sender.Money += Response.SenderMoney;
        CurrentEvent.sender.WarPower += Response.SenderWarPower;
        CurrentEvent.receiver.Money += Response.ReceiverMoney;
        CurrentEvent.receiver.WarPower += Response.ReceiverWarPower;

        if (Response.ChangeLeader)
        {
            CurrentEvent.receiver.LeaderName = "New Leader";
            CurrentEvent.receiver.LeaderPersonality = DevTools.RandomEnumValue<PersonalityTypes>();
            CurrentEvent.receiver.LeaderFocus = focuses[Random.Range(0, focuses.Count)];
        }

        main.UpdateCountrySlots();

        if (!CurrentEvent.receiver.IsPlayerCountry || true)
            main.ShowNotice(CurrentEvent, Response);
    }
}