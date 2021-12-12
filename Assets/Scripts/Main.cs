using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Main : MonoBehaviour
{
    public const int Max_Action_Reactions = 3; // should be updated for the largest number of responses for a single action
    public const int Default_Money_Gain = 50;
    public const int Default_WarPower_Gain = 50;
    public const int Default_Loan_Grace_Period = 5;
    public const int Default_Relation_Drift_Rate = 5;
    public const int Default_Relation_Grace_Period = 5; // turns after a relation-changing action before relations start to drift
    public const int Default_Relation_Resting_Value = 0; // where range is centered on
    public const int Default_Relation_Resting_Range = 20; // size of range in either direction (value of 20 means range is -20 to 20)

    #region Variables

    public CountrySlot[] cs_NonPlayers;

    public Country[] cnt_NonPlayers;

    public Country cnt_Player;

    [SerializeField]
    private CountrySlot cs_Player;

    [SerializeField]
    private EventSlot eventSlot;

    [SerializeField]
    private Notice notice;

    public GameObject go_Actions;

    [SerializeField]
    private List<GameObject> go_CurrentEventSlots = new List<GameObject>();

    [SerializeField]
    private List<GameObject> go_CurrentNotices = new List<GameObject>();

    private List<Event> ce_Player = new List<Event>();
    private List<Event> ce_NonPlayer = new List<Event>();

    #region Reference

    private ActionManager actionManager;

    #endregion

    #region Game Information
    [HideInInspector]
    public bool actionTaken; // if an action has already been taken by the player this turn

    public static bool s_noDeath;
    public bool noDeath;

    public List<ColorBlock> cb_CountrySlotColors;

    //public Color co_csNormal, co_csHighlighted, co_csClicked, co_csSelected, co_csDisabled;
    public delegate void TurnAction();
    public static event TurnAction s_TurnActions;


    #endregion


    #endregion

    #region Initialization

    private void Start()
    {
        // Reference Variables
        actionManager = GetComponent<ActionManager>();

        s_noDeath = noDeath;

        s_TurnActions += UpdateCountryResources;
        s_TurnActions += UpdateCountryRelations;

        SetActionButtonsEnabled(false);

        Relation[] rel_PlayerNew = new Relation[cnt_NonPlayers.Length]; // makes new list of relations
        for (int i = 0; i < rel_PlayerNew.Length; i++) rel_PlayerNew[i] = new Relation(); // initializes each one   

        cnt_Player.Relations = rel_PlayerNew;
        Relation[] rel_PlayerLeaderNew = (Relation[])rel_PlayerNew.Clone();
        cnt_Player.Leader = new Leader("john", new Relation(), rel_PlayerLeaderNew, DevTools.RandomEnumValue<PersonalityTypes>(), DevTools.RandomListValue<Focus>(actionManager.focuses));
        for (int i = 0; i < cnt_NonPlayers.Length; i++) // Country initialization
        {

            cnt_NonPlayers[i].ID = i; // Set all cnt_NonPlayers[i]; IDs

            Relation[] rel_New = new Relation[cnt_NonPlayers.Length]; // makes new list of relations
            for (int j = 0; j < rel_New.Length; j++) rel_New[j] = new Relation(); // initializes each one   

            cnt_NonPlayers[i].Relations = rel_New; // Reset all relations            


            // AI-Player Setup
            cnt_NonPlayers[i].PlayerRelations = new Relation(); // Reset player relations              
            cnt_NonPlayers[i].UpdateFocusModifiers(cnt_NonPlayers[i].Focus);

            Relation[] rel_LeaderNew = (Relation[])rel_New.Clone();
            cnt_NonPlayers[i].Leader = new Leader("john", new Relation(), rel_LeaderNew, DevTools.RandomEnumValue<PersonalityTypes>(), DevTools.RandomListValue<Focus>(actionManager.focuses));
        }

        foreach (CountrySlot cs in cs_NonPlayers) // Country slot setup
        {
            cs.Init();
            cs.SetColorBlock(cb_CountrySlotColors[0]);
        }

        cnt_NonPlayers[0].PlayerRelations.Value += 69;
        cs_Player.Init();
        // Game Setup
        GameInfo.s_TurnCount = 0;
        eventSlot.Init();
        UpdateCountrySlots();
    }

    #endregion

    #region Actions

    public void RunEvent(Country Sender)
    {
        Action newActionType = actionManager.actions[Random.Range(0, actionManager.actions.Count)];
        int newReceiverID = Random.Range(0, cnt_NonPlayers.Length);
        Country newReceiver = (newReceiverID == Sender.ID) ? cnt_Player : cnt_NonPlayers[newReceiverID];
        // if it is a disagreement, get another country to be the affected country
        Country newAffected = (newActionType.Name == "Disagreement") ? cnt_NonPlayers[Random.Range(0, cnt_NonPlayers.Length)] : null;
        // if the sender/receiver and affected is the same, keep randomizing affected till they're not
        while (Sender == newAffected || newReceiver == newAffected)
            newAffected = cnt_NonPlayers[Random.Range(0, cnt_NonPlayers.Length)];
        //if (newReceiver == cnt_Player)
        print(newActionType + " " + Sender + " " + newReceiver + " " + newAffected);
        Event newEvent = new Event(newActionType, Sender, newReceiver, newAffected);
        if (newEvent.receiver.IsPlayerCountry)
            ce_Player.Add((newEvent));
        else
            ce_NonPlayer.Add(newEvent);
    }

    private void SendAction(Event currentEvent)
    {
        if (currentEvent.receiver.IsPlayerCountry)
        {
            ShowEventSlot(currentEvent);
            return;
        }
        actionManager.RunAction(currentEvent);

        //TODO would decide receiver response and run it
        actionManager.RunResponse(AIManager.BestResponse(currentEvent, currentEvent.sender));
        UpdateCountrySlots();

    }

    private void ShowEventSlot(Event currentEvent)
    {
        EventSlot newEventSlot = GameObject.Instantiate(eventSlot, parent: eventSlot.transform.parent);
        newEventSlot.Init();
        newEventSlot.transform.SetAsFirstSibling();
        newEventSlot.SetCountryName(currentEvent.sender.CountryName);
        newEventSlot.SetLeaderName(currentEvent.sender.Leader.Name);
        newEventSlot.SetAction(currentEvent.action.Name);

        go_CurrentEventSlots.Add(newEventSlot.gameObject);

        string decodedMessage = currentEvent.action.PlayerDisplayMessage;
        decodedMessage = DevTools.DecodeMessage(currentEvent.action.PlayerDisplayMessage, currentEvent);

        newEventSlot.SetDescription(decodedMessage);

        for (int i = 0; i < Max_Action_Reactions; i++)
        {
            if (i >= currentEvent.action.Responses.Count)
            {
                newEventSlot.SetResponseActive(i, false);
                break;
            }

            string decodedResponse = currentEvent.action.Responses[i].DisplayMessage;
            decodedResponse = DevTools.DecodeMessage(currentEvent.action.Responses[i].DisplayMessage, currentEvent);

            newEventSlot.SetResponse(i, decodedResponse);
            newEventSlot.SetResponseActive(i, true);
        }

        newEventSlot.SetFlag(currentEvent.sender.Flag);
        newEventSlot.gameObject.SetActive(true);
        GameInfo.ShowNextTurnButton(false);
    }

    public void ShowNotice(Event currentEvent, Response response)
    {
        Notice newNotice = GameObject.Instantiate(notice, parent: notice.transform.parent);
        newNotice.Init();
        newNotice.transform.SetAsFirstSibling();
        go_CurrentNotices.Add(newNotice.gameObject);
        newNotice.transform.localPosition += new Vector3(0, (go_CurrentNotices.IndexOf(newNotice.gameObject) * 100), 0);
        newNotice.SetDescription(DevTools.DecodeMessage(response.NoticeDisplayMessage, currentEvent));//+ "\n" + CurrentSender.CountryName + "\t" + CurrentReceiver.CountryName + "\n" + response.DisplayMessage);
        newNotice.SetSymbol(currentEvent.action.NoticeSymbol);
        newNotice.gameObject.SetActive(true);


    }

    public void ButtonResponse(int index)
    {
        ce_Player.RemoveAt(0); // Remove latest event
        actionManager.RunResponse(index);
        UpdateCountrySlots();

        GameObject.Destroy(go_CurrentEventSlots[0]);
        go_CurrentEventSlots.RemoveAt(0); // Removes first event slot (the currently displayed one)        

        if (go_CurrentEventSlots.Count == 0)
            GameInfo.ShowNextTurnButton(true);
        else
            actionManager.RunAction(ce_Player[0]); // Show next response
    }

    #endregion

    #region Miscellaneous 

    public void UpdateCountrySlots()
    {
        cs_Player.SetCountryName(cnt_Player.CountryName);
        cs_Player.SetLeaderName(cnt_Player.Leader.Name);
        cs_Player.SetPersonality(cnt_Player.Leader.Personality);
        cs_Player.SetMoney(cnt_Player.Money);
        cs_Player.SetWarPower(cnt_Player.WarPower);
        for (int i = 0; i < cs_NonPlayers.Length; i++)
        {
            var cnt = cnt_NonPlayers[i];
            var cs = cs_NonPlayers[i];
            // CountrySlot.SetFlag(Country.Flag); Flags don't matter yet
            cs.SetCountryName(cnt.CountryName);
            cs.SetLeaderName(cnt.Leader.Name);
            cs.SetPersonality(cnt.Leader.Personality);
            cs.SetMoney(cnt.Money);
            cs.SetWarPower(cnt.WarPower);
            cs.SetRelation(cnt.PlayerRelations.Value); // Get relations toward the player country 
        }
    }

    public void SetActionButtonsEnabled(bool enabled)
    {
        go_Actions.GetComponentsInChildren<Button>().ToList().ForEach(x => x.interactable = enabled);
    }

    public void NextTurn()
    {
        GameInfo.s_TurnCount++;
        UpdateCountrySlots();

        SetActionButtonsEnabled(false);

        go_CurrentNotices.ForEach(x => GameObject.Destroy(x.gameObject));
        go_CurrentNotices.Clear();

        if (s_TurnActions != null)
            s_TurnActions();
        actionTaken = false;

        cnt_NonPlayers[0].Relations[0].Value = GameInfo.s_TurnCount;


        foreach (Country country in cnt_NonPlayers)
        {
            if (Random.Range(1, 100) > 0)// 25%
                RunEvent(country);
        }

        ce_NonPlayer.ForEach(x => SendAction(x)); // Run events in proper order so nonplayer events go first
        ce_NonPlayer.Clear();
        ce_Player.ForEach(x => ShowEventSlot(x)); // Player events go last

        if (ce_Player.Count > 0)
            actionManager.RunAction(ce_Player[0]); // Set up first (displayed) player event
    }

    private void UpdateCountryResources() // runs every turn
    {
        foreach (Country country in cnt_NonPlayers)
        {
            country.Money += Default_Money_Gain + country.Focus.MoneyModifier;
            country.WarPower += Default_WarPower_Gain + country.Focus.WarPowerModifier;
            /*foreach (Relation relation in country.Relations)
            {
                relation.Value += relation.DriftSpeed * ((relation.Value > relation.RestingValue) ? -1 : 1);
                relation.GracePeriod--;
            }*/
        }
        cnt_Player.Money += Default_Money_Gain + cnt_Player.Focus.MoneyModifier;
        cnt_Player.WarPower += Default_WarPower_Gain + cnt_Player.Focus.WarPowerModifier;
    }

    private void UpdateCountryRelations()
    {
        foreach (Country cnt in cnt_NonPlayers)
        {
            foreach (Relation relation in cnt.Relations)
            {
                relation.GracePeriod--;
                if (relation.IsDrifting)
                    relation.Value += (relation.Value < relation.RestingMin) ? 1 : -1 * relation.DriftSpeed; // If greater than resting range, decrease relations, otherwise increase them               


            }
            cnt.PlayerRelations.GracePeriod--;
            if (cnt.PlayerRelations.IsDrifting)
            {
                int driftDirection = (cnt.PlayerRelations.Value < cnt.PlayerRelations.RestingMin) ? 1 : -1;
                cnt.PlayerRelations.Value += driftDirection * cnt.PlayerRelations.DriftSpeed; // If greater than resting range, decrease cnt.PlayerRelationss, otherwise increase them               
                cnt.PlayerRelations.Value = (driftDirection == 1) ? Mathf.Min(cnt.PlayerRelations.Value, cnt.PlayerRelations.RestingMin) : Mathf.Max(cnt.PlayerRelations.Value, cnt.PlayerRelations.RestingMax);
            }
        }

    }

    #endregion

}

public struct Event
{
    public Event(Action _action, Country _sender, Country _receiver, Country _affected)
    {
        action = _action;
        sender = _sender;
        receiver = _receiver;
        affected = _affected;
    }
    public Action action;
    public Country sender;
    public Country receiver;
    public Country affected;
}
