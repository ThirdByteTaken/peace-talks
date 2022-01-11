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
    [Range(0, 1)]
    public float RelationChangeFromFocusDiffFactor;
    [Range(0, 20)]
    public float RelationChangeFromPersonalityDiffFactor;

    #region Variables

    public CountrySlot[] cs_NonPlayers;

    public List<Country> cnt_Players = new List<Country>();

    [HideInInspector]
    public Country cnt_Player;

    [SerializeField]
    private CountrySlot cs_Player;

    [SerializeField]
    private EventSlot eventSlot;

    [SerializeField]
    private Notice notice;

    public GameObject go_Actions;

    public Camera mainCamera;

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
        s_TurnActions += DriftCountryRelations;
        s_TurnActions += UpdateInterCountryRelations;
        SetActionButtonsEnabled(false);
        cnt_Player = cnt_Players.Find(x => x.IsPlayerCountry);
        for (int i = 0; i < cnt_Players.Count; i++) // Country initialization
        {

            cnt_Players[i].ID = i; // Set all cnt_NonPlayers[i]; IDs

            Relation[] rel_New = new Relation[cnt_Players.Count - 1]; // makes new list of relations
            for (int j = 0; j < rel_New.Length; j++) rel_New[j] = new Relation(); // initializes each one   

            cnt_Players[i].Relations = rel_New; // Reset all relations            


            // AI-Player Setup            
            cnt_Players[i].LeaderRelations = new Relation(); // Reset leader relations              

            var newFocus = DevTools.RandomListValue<Focus>(ActionManager.focuses);

            cnt_Players[i].Leader = new Leader(TextGenerator.LeaderName(), (Relation[])rel_New.Clone(), DevTools.RandomEnumValue<PersonalityTypes>(), newFocus);
            cnt_Players[i].Focus = newFocus;


            cnt_Players[i].FocusTendencies = new int[ActionManager.focuses.Count];
            cnt_Players[i].FocusTendencies[newFocus.ID] += 100;
            cnt_Players[i].UpdateFocusModifiers(cnt_Players[i].Focus);

            s_TurnActions += cnt_Players[i].CountryStatsDrift;
        }

        foreach (CountrySlot cs in cs_NonPlayers) // Country slot setup
        {
            cs.Init();
            cs.SetColorBlock(cb_CountrySlotColors[0]);
        }

        cs_Player.Init();
        // Game Setup
        GameInfo.s_TurnCount = 0;
        eventSlot.Init();
        UpdateCountrySlots();
    }

    #endregion

    #region Actions

    public void RunEvent(Country sender)
    {
        Country receiver = AIManager.BestCountry(sender);
        if (receiver == null)
        {
            int rand = Random.Range(0, cnt_Players.Count);
            while ((rand == sender.ID))
                rand = Random.Range(0, cnt_Players.Count);
            receiver = cnt_Players[rand];
        }

        Action nextAction = AIManager.BestAction(sender, receiver);
        if (nextAction == null) return;

        sender.cnt_RecentlyInteracted.Remove(receiver);

        // If it is a disagreement, get another country to be the affected country
        Country newAffected = (nextAction.Name == "Disagreement") ? cnt_Players[Random.Range(0, cnt_Players.Count)] : null;

        // If the sender/receiver and affected is the same, keep randomizing affected till they're not
        while (sender == newAffected || receiver == newAffected)
            newAffected = cnt_Players[Random.Range(0, cnt_Players.Count)];
        Event newEvent = new Event(nextAction, sender, receiver, newAffected);
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
            var cnt = cnt_Players[i + 1]; // +1 to skip over player
            var cs = cs_NonPlayers[i];
            // CountrySlot.SetFlag(Country.Flag); Flags don't matter yet
            cs.SetCountryName(cnt.CountryName);
            cs.SetLeaderName(cnt.Leader.Name);
            cs.SetPersonality(cnt.Leader.Personality);
            cs.SetMoney(cnt.Money);
            cs.SetWarPower(cnt.WarPower);
            cs.SetRelation(cnt.Relations[cnt_Player.ID].Value); // Get relations toward the player country 
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
        UpdateActionCooldowns();

        SetActionButtonsEnabled(false);

        go_CurrentNotices.ForEach(x => GameObject.Destroy(x.gameObject));
        go_CurrentNotices.Clear();

        if (s_TurnActions != null)
            s_TurnActions();
        actionManager.actionTaken = false;




        foreach (Country country in cnt_Players)
        {
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
        foreach (Country country in cnt_Players)
        {
            country.Money += Default_Money_Gain + country.Focus.MoneyModifier;
            country.WarPower += Default_WarPower_Gain + country.Focus.WarPowerModifier;
            /*foreach (Relation relation in country.Relations)
            {
                relation.Value += relation.DriftSpeed * ((relation.Value > relation.RestingValue) ? -1 : 1);
                relation.GracePeriod--;
            }*/
        }
    }

    private void UpdateActionCooldowns()
    {
        foreach (Country country in cnt_Players)
        {
            foreach (Action action in country.ActionCooldowns.Keys.ToList())
            {
                country.ActionCooldowns[action]--;
                if (country.ActionCooldowns[action] <= 0) country.ActionCooldowns.Remove(action);
            }
        }

    }

    private void DriftCountryRelations()
    {
        foreach (Country cnt in cnt_Players)
        {
            foreach (Relation relation in cnt.Relations)
            {
                relation.CurrentGracePeriod--;
                if (relation.IsDrifting)
                {
                    int driftDirection = (relation.Value < relation.RestingMin) ? 1 : -1;
                    relation.Value += driftDirection * relation.DriftSpeed; // If greater than resting range, decrease relations, otherwise increase them               
                    relation.Value = (driftDirection == 1) ? Mathf.Min(relation.Value, relation.RestingMin) : Mathf.Max(relation.Value, relation.RestingMax);
                }


            }
            cnt.LeaderRelations.Value += Mathf.RoundToInt(cnt.LeaderRelations.DriftSpeed * (cnt.FocusTendencies[cnt.Leader.Focus.ID] - (float)cnt.FocusTendencies.Average()));
        }

    }

    private void UpdateInterCountryRelations()
    {
        print("TURN " + GameInfo.s_TurnCount + ":");
        for (int i = 0; i < cnt_Players.Count; i++)
        {
            for (int j = 0; j < cnt_Players.Count; j++)
            {
                if (i == j) continue;
                print("\tCOUNTRY " + i + " to COUNTRY " + j + ":");

                int focusTendencyDiff = Mathf.Abs(cnt_Players[i].FocusTendencies[cnt_Players[i].Focus.ID] - cnt_Players[j].FocusTendencies[cnt_Players[i].Focus.ID]);

                print("\t\tFocusTendencyDiff: " + focusTendencyDiff + " (first country -> " + cnt_Players[i].FocusTendencies[cnt_Players[i].Focus.ID] + " second country -> " + cnt_Players[j].FocusTendencies[cnt_Players[i].Focus.ID] + ")");

                int compareFocusTendencyDiff = Mathf.Abs(cnt_Players[j].FocusTendencies[cnt_Players[j].Focus.ID] - cnt_Players[i].FocusTendencies[cnt_Players[j].Focus.ID]);

                print("\t\tCompareFocusTendencyDiff: " + compareFocusTendencyDiff + " (first country -> " + cnt_Players[i].FocusTendencies[cnt_Players[j].Focus.ID] + " second country -> " + cnt_Players[j].FocusTendencies[cnt_Players[j].Focus.ID] + ")");

                int averageFocusTendencyDiff = ((focusTendencyDiff + compareFocusTendencyDiff) / 2);

                print("\t\tAverage: " + averageFocusTendencyDiff);

                int focusTendencyDiffEffect = Mathf.RoundToInt(cnt_Players[i].FocusDifferenceHarshness * RelationChangeFromFocusDiffFactor * averageFocusTendencyDiff);

                print("\t\tfocusTendencyDiffEffect: " + focusTendencyDiffEffect + " (harshness -> " + cnt_Players[i].FocusDifferenceHarshness + " factor -> " + RelationChangeFromFocusDiffFactor + ")");

                int personalityDiff = Mathf.Abs((int)cnt_Players[i].Leader.Personality - (int)cnt_Players[j].Leader.Personality);

                print("\t\tPersonalityDiff: " + personalityDiff + " (first country -> " + cnt_Players[i].Leader.Personality + " second country -> " + cnt_Players[j].Leader.Personality + ")");

                int personalityDiffEffect = Mathf.RoundToInt(cnt_Players[i].PersonalityDifferenceHarshness * RelationChangeFromPersonalityDiffFactor * personalityDiff);

                print("\t\tPersonalityDiffEffect: " + personalityDiffEffect + " (harshness -> " + cnt_Players[i].PersonalityDifferenceHarshness + " factor -> " + RelationChangeFromPersonalityDiffFactor + ")");

                int ideologicalDifference = personalityDiffEffect + focusTendencyDiffEffect;

                print("\t\tideologicalDifference: " + ideologicalDifference);

                int restingValue = (-ideologicalDifference + 15) * 2; // converts range of 0 - 30 to a range of -30 to 30

                print("\t\trestingValue: " + restingValue);

                cnt_Players[i].Relations[j].RestingValue = restingValue;


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
