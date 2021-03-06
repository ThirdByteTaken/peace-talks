using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Main : MonoBehaviour
{

    public static Main Instance;

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
    [Range(0, 1)]
    public float RelationChangeFromPersonalityDiffFactor;

    #region Variables

    [SerializeField]
    private List<CountrySlot> cs_PlayersList;
    public Dictionary<Country, CountrySlot> cs_Players = new Dictionary<Country, CountrySlot>();

    public List<Country> cnt_Players = new List<Country>();

    public Country cnt_Player;

    [SerializeField]
    private EventSlot eventSlot;

    public CountryView CountryView;

    [SerializeField]
    private Notice notice;

    public Camera mainCamera;

    [SerializeField]
    private List<GameObject> go_CurrentEventSlots = new List<GameObject>();

    [SerializeField]
    private List<GameObject> go_CurrentNotices = new List<GameObject>();

    private List<Event> ce_Player = new List<Event>();
    private List<Event> ce_NonPlayer = new List<Event>();

    #region Reference

    private ActionManager actionManager;
    private UIManager uIManager;

    #endregion

    #region Game Information



    public bool noDeath;



    //public Color co_csNormal, co_csHighlighted, co_csClicked, co_csSelected, co_csDisabled;
    public delegate void TurnAction();
    public static event TurnAction s_TurnActions;


    #endregion


    #endregion

    #region Initialization    

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

        // Reference Variables
        actionManager = ActionManager.Instance;
        uIManager = UIManager.Instance;

        // TurnActions Subscriptions
        s_TurnActions += UpdateCountryResources;
        s_TurnActions += DriftCountryRelations;
        s_TurnActions += UpdateInterCountryRelations;

        // Country initialization
        cnt_Player = cnt_Players.Find(x => x.IsPlayer);
        for (int i = 0; i < cnt_Players.Count; i++)
        {
            var rel_LeaderNew = new Dictionary<Country, Relation>(); // makes new list of relations
            for (int j = 0; j < cnt_Players.Count; j++) rel_LeaderNew.Add(cnt_Players[j], new Relation()); // initializes each one   

            var rel_New = new Dictionary<Country, Relation>(rel_LeaderNew);
            rel_New.Remove(cnt_Players[i]);

            cnt_Players[i].Relations = new Dictionary<Country, Relation>(rel_New); // Reset all relations            

            // AI-Player Setup            
            cnt_Players[i].LeaderRelations = new Relation(); // Reset leader relations              

            var newFocus = DevTools.RandomListValue<Focus>(actionManager.Focuses);
            cnt_Players[i].Leader = new Leader(TextGenerator.LeaderName(), rel_LeaderNew, DevTools.RandomListValue<PersonalityType>(actionManager.PersonalityTypes), newFocus);
            cnt_Players[i].Focus = newFocus;

            cnt_Players[i].FocusTendencies = new int[actionManager.Focuses.Count];
            cnt_Players[i].FocusTendencies[newFocus.ID] += 100;
            cnt_Players[i].UpdateFocusModifiers(cnt_Players[i].Focus);

            cs_Players.Add(cnt_Players[i], cs_PlayersList[i]);
            s_TurnActions += cnt_Players[i].CountryStatsDrift;
        }

        // UI Initializations
        foreach (CountrySlot cs in cs_Players.Values) // Country slot setup
        {
            cs.Init();
            if (!cs.Country.IsPlayer) cs.SetColorBlock(UIManager.Instance.cb_CountrySlotColors[0]);
        }
        eventSlot.Init();
        CountryView.Init();

        // Game Setup
        GameInfo.s_TurnCount = 0;
        UIManager.Instance.SetActionButtonsEnabled(false);
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
        if (newEvent.receiver.IsPlayer)
            ce_Player.Add((newEvent));
        else
            ce_NonPlayer.Add(newEvent);
    }

    private void SendAction(Event currentEvent)
    {
        if (currentEvent.receiver.IsPlayer)
        {
            ShowEventSlot(currentEvent);
            return;
        }
        actionManager.SetCurrentAction(currentEvent);

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
            actionManager.SetCurrentAction(ce_Player[0]); // Show next response
    }

    #endregion

    #region Miscellaneous 

    public void UpdateCountrySlots()
    {
        foreach (CountrySlot cs in cs_Players.Values)
            cs.UpdateSlot();
    }



    public void NextTurn()
    {
        GameInfo.s_TurnCount++;
        UpdateCountrySlots();
        UpdateActionCooldowns();

        UIManager.Instance.SetActionButtonsEnabled(false);

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
            actionManager.SetCurrentAction(ce_Player[0]); // Set up first (displayed) player event
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
            foreach (Relation relation in cnt.Relations.Values)
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
        // print("TURN " + GameInfo.s_TurnCount + ":");
        for (int i = 0; i < cnt_Players.Count; i++)
        {
            for (int j = 0; j < cnt_Players.Count; j++)
            {
                if (i == j) continue;
                // print("\tCOUNTRY " + i + " to COUNTRY " + j + ":");

                int focusTendencyDiff = Mathf.Abs(cnt_Players[i].FocusTendencies[cnt_Players[i].Focus.ID] - cnt_Players[j].FocusTendencies[cnt_Players[i].Focus.ID]);

                // print("\t\tFocusTendencyDiff: " + focusTendencyDiff + " (first country -> " + cnt_Players[i].FocusTendencies[cnt_Players[i].Focus.ID] + " second country -> " + cnt_Players[j].FocusTendencies[cnt_Players[i].Focus.ID] + ")");

                int compareFocusTendencyDiff = Mathf.Abs(cnt_Players[j].FocusTendencies[cnt_Players[j].Focus.ID] - cnt_Players[i].FocusTendencies[cnt_Players[j].Focus.ID]);

                // print("\t\tCompareFocusTendencyDiff: " + compareFocusTendencyDiff + " (first country -> " + cnt_Players[i].FocusTendencies[cnt_Players[j].Focus.ID] + " second country -> " + cnt_Players[j].FocusTendencies[cnt_Players[j].Focus.ID] + ")");

                int averageFocusTendencyDiff = ((focusTendencyDiff + compareFocusTendencyDiff) / 2);

                // print("\t\tAverage: " + averageFocusTendencyDiff);

                int focusTendencyDiffEffect = Mathf.RoundToInt(cnt_Players[i].FocusDifferenceHarshness * RelationChangeFromFocusDiffFactor * averageFocusTendencyDiff);

                // print("\t\tfocusTendencyDiffEffect: " + focusTendencyDiffEffect + " (harshness -> " + cnt_Players[i].FocusDifferenceHarshness + " factor -> " + RelationChangeFromFocusDiffFactor + ")");

                int personalityDiff = Mathf.Abs(cnt_Players[i].Leader.Personality.ID - cnt_Players[j].Leader.Personality.ID);

                // print("\t\tPersonalityDiff: " + personalityDiff + " (first country -> " + cnt_Players[i].Leader.Personality + " second country -> " + cnt_Players[j].Leader.Personality + ")");

                int personalityDiffEffect = Mathf.RoundToInt(cnt_Players[i].PersonalityDifferenceHarshness * RelationChangeFromPersonalityDiffFactor * personalityDiff);

                // print("\t\tPersonalityDiffEffect: " + personalityDiffEffect + " (harshness -> " + cnt_Players[i].PersonalityDifferenceHarshness + " factor -> " + RelationChangeFromPersonalityDiffFactor + ")");

                int ideologicalDifference = personalityDiffEffect + focusTendencyDiffEffect;

                // print("\t\tideologicalDifference: " + ideologicalDifference);

                int restingValue = (-ideologicalDifference + 15) * 2; // converts range of 0 - 30 to a range of -30 to 30

                // print("\t\trestingValue: " + restingValue);

                cnt_Players[i].Relations[cnt_Players[j]].RestingValue = restingValue;


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
