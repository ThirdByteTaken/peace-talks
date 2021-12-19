using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu]
public class Country : ScriptableObject
{

    [SerializeField]
    private string countryName;
    [HideInInspector]
    public string CountryName
    {
        get
        {
            return "<material=\"LiberationSans SDF - Outline\">" + DevTools.ColorCountryText(this) + countryName + "</color>" + "</material>";
        }
    }
    public Leader previousLeader;

    public Leader Leader;

    public bool IsPlayerCountry;

    public int ID; // Used to access it within cnt_Countries array

    [SerializeField]
    private int money;
    public int Money
    {
        get
        {
            return money;
        }
        set
        {
            if (value <= 0 && IsPlayerCountry) DeathManager.GameOver("Debt", "Your country ran out of money and simply couldn't exist anymore");
            money = value;
        }
    }

    [SerializeField]
    private int warPower;
    public int WarPower
    {
        get
        {
            return warPower;
        }
        set
        {
            if (value <= 0 && IsPlayerCountry) DeathManager.GameOver("Anarchy", "Your war power was too low and your people resorted to anarchy.");
            warPower = value;
        }
    }

    private Relation playerRelations;
    public Relation PlayerRelations
    {
        get
        {
            return playerRelations;
        }
        set
        {
            if (value.Value <= -100) DeathManager.GameOver("War", "War has broken out");
            playerRelations = value;
        }
    }


    public Relation[] Relations;

    private Relation leaderRelations;
    public Relation LeaderRelations
    {
        get
        {
            return leaderRelations;
        }
        set
        {
            //if (value.Value <= -100) PopulationRevolt();
            leaderRelations = value;
        }
    }

    public List<Country> cnt_RecentlyInteracted;

    public int MoneyGain = 10;
    public int WarPowerGain = 10;


    [SerializeField]
    private Focus focus;
    public Focus Focus
    {
        get { return focus; }
        set
        {
            UpdateFocusModifiers(value);
            focus = value;
        }
    }

    public List<int> FocusTendencies; // how much the country values each focus
    [Range(0, 2)]
    public float PersonalityDifferenceHarshness; // affects how much their relations decrease each turn because of a personality mismatch with the other country
    [Range(0, 2)]
    public float FocusDifferenceHarshness; // affects how much their relations decrease each turn because of focus mismatches with the other country

    public Sprite Flag;
    public Color textColor;

    public void UpdateFocusModifiers(Focus value)
    {
        MoneyGain = Main.Default_Money_Gain + value.MoneyModifier;
        WarPowerGain = Main.Default_WarPower_Gain + value.WarPowerModifier;
        foreach (Relation Relation in Relations)
        {
            Relation.DriftSpeed = Main.Default_Relation_Drift_Rate + value.RelationDriftModifier;
            Relation.CurrentGracePeriod = Main.Default_Relation_Grace_Period + value.RelationGracePeriodModifier;
            Relation.RestingValue = Main.Default_Relation_Resting_Value + value.RelationRestingValueModifier;
            Relation.RestingRange = Main.Default_Relation_Resting_Range + value.RelationRestingRangeModifier;
        }
        if (ID != -1)
        {
            PlayerRelations.DriftSpeed = Main.Default_Relation_Drift_Rate + value.RelationDriftModifier;
            PlayerRelations.CurrentGracePeriod = Main.Default_Relation_Grace_Period + value.RelationGracePeriodModifier;
            PlayerRelations.RestingValue = Main.Default_Relation_Resting_Value + value.RelationRestingValueModifier;
            PlayerRelations.RestingRange = Main.Default_Relation_Resting_Range + value.RelationRestingRangeModifier;
        }
    }

    public void CountryStatsDrift() // Country focus shifts towards leader focus
    {
        List<float> FocusValues = FocusTendencies.ConvertAll(x => (float)x);
        FocusValues.RemoveAt(Leader.Focus.ID);
        int oldWeight = 100 - FocusTendencies[Leader.Focus.ID];
        int newWeight = 100 - (FocusTendencies[Leader.Focus.ID] += 5);

        for (int i = 0; i < FocusValues.Count; i++)
        {
            FocusValues[i] = (newWeight / (float)oldWeight);
            FocusValues[i] = Mathf.Clamp(FocusValues[i], 0, 100);
        }

        List<float> FocusValueRemainders = FocusValues.ConvertAll(x => x %= 1);

        for (int i = 0; i < FocusValues.Count; i++)
            FocusValues[i] = Mathf.Floor(FocusValues[i]);

        //int x = 0;
        while (FocusValues.Sum() < newWeight)
        {
            int maxIndex = FocusValueRemainders.IndexOf(FocusValueRemainders.Max());
            FocusValues[maxIndex]++;
            FocusValueRemainders[maxIndex] = 0; // So it wont be accessed as max value
            /*if (x > 10)
            {
                Debug.Log("bruh it got stuck");
                FocusValues.ForEach(x => Debug.Log("STUCK VAL \t" + x));
                break;
            }
            x++;
            */
        }

        for (int i = 0; i < FocusValues.Count; i++)
        {
            FocusTendencies[i + ((i >= Leader.Focus.ID) ? 1 : 0)] = (int)FocusValues[i];
        }
        /*
        Debug.Log("Focus Values: (should be same)");

        FocusTendencies.ForEach(x => Debug.Log("focus value \t" + x));*/
        Focus = ActionManager.focuses[FocusTendencies.IndexOf(FocusTendencies.Max())];

    }

    public void PopulationRevolt()
    {
        if (IsPlayerCountry) DeathManager.GameOver("Revolt", "The people of your country disliked you so much that they revolted against you");
        ChangeLeader(this);
    }

    public void ChangeLeader(Country modelCountry = null) // modelCountry = the country the new leader is similar to - no value given = random leader
    {
        Relation[] rel_New = new Relation[Relations.Length]; // makes new list of relations
        for (int j = 0; j < rel_New.Length; j++)
        {
            rel_New[j] = new Relation(); // initializes each one   
            if (modelCountry != null)
                rel_New[j].Value = Random.Range(modelCountry.Relations[j].Value - 20, modelCountry.Relations[j].Value + 20); // makes new leaders opinions of other countries similar to the populations opinions of them
        }
        Relation rel_newPlayer = new Relation();
        if (modelCountry == null)
            rel_newPlayer.Value = Random.Range(modelCountry.PlayerRelations.Value - 20, modelCountry.PlayerRelations.Value + 20);
        // TODO Replace with new DevTools function 
        var TotalRatio = modelCountry.FocusTendencies.Sum();
        int rand = Random.Range(0, TotalRatio + 1);
        int iteration = 0;
        foreach (int x in modelCountry.FocusTendencies)
        {
            if ((rand -= x) < 0) break;
            iteration++;
        }
        Focus foc_New = (modelCountry != null) ? ActionManager.focuses[iteration] : DevTools.RandomListValue<Focus>(ActionManager.focuses);
        Debug.Log("New leader for country " + ID + "(model country: " + modelCountry + "):");
        Debug.Log("\tnewplayerrelation: " + rel_newPlayer.Value);
        Debug.Log("\tnewrelations:");
        rel_New.ToList().ForEach(x => Debug.Log("\t\t" + x.Value));
        Debug.Log("\tnewfocus: " + foc_New.name);
        previousLeader = Leader;
        Leader = new Leader(TextGenerator.LeaderName(), rel_newPlayer, rel_New, DevTools.RandomEnumValue<PersonalityTypes>(), foc_New);
        modelCountry.leaderRelations.Value = modelCountry.leaderRelations.RestingValue;
    }



    public Dictionary<Action, int> ActionCooldowns = new Dictionary<Action, int>();
}

