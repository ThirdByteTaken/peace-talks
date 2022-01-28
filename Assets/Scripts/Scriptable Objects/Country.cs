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

    [HideInInspector]
    public bool IsPlayer
    {
        get
        {
            return (this == Main.Instance.cnt_Player);
        }
    }

    public int ID
    {
        get
        {
            return (Main.Instance.cnt_Players.IndexOf(this));
        }
    }

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
            if (value <= 0 && IsPlayer) DeathManager.GameOver("Debt", "Your country ran out of money and simply couldn't exist anymore");
            money = value;
        }
    }

    [SerializeField]
    private int warPower;
    public int WarPower
    {
        get { return warPower; }
        set
        {
            if (value <= 0 && IsPlayer) DeathManager.GameOver("Anarchy", "Your war power was too low and your people resorted to anarchy.");
            warPower = value;
        }
    }

    public Dictionary<Country, Relation> Relations;

    private Relation leaderRelations;
    public Relation LeaderRelations
    {
        get
        {
            return leaderRelations;
        }
        set
        {
            if (value.Value <= -100) PopulationRevolt();
            leaderRelations = value;
        }
    }

    public List<Country> cnt_RecentlyInteracted;

    public int MoneyGain = 10;
    public int WarPowerGain = 10;
    public List<Territory> OwnedTerritories;

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

    public int[] FocusTendencies; // how much the country values each focus
    [Range(0, 2)]
    public float PersonalityDifferenceHarshness; // affects how much their relations decrease each turn because of a personality mismatch with the other country
    [Range(0, 2)]
    public float FocusDifferenceHarshness; // affects how much their relations decrease each turn because of focus mismatches with the other country

    public Sprite Flag;
    public Color textColor;

    public Dictionary<Action, int> ActionCooldowns = new Dictionary<Action, int>();

    #region Focuses

    public void UpdateFocusModifiers(Focus value)
    {
        MoneyGain = Main.Default_Money_Gain + value.MoneyModifier;
        WarPowerGain = Main.Default_WarPower_Gain + value.WarPowerModifier;
        foreach (Relation Relation in Relations.Values)
        {
            Relation.DriftSpeed = Main.Default_Relation_Drift_Rate + value.RelationDriftModifier;
            Relation.CurrentGracePeriod = Main.Default_Relation_Grace_Period + value.RelationGracePeriodModifier;
            Relation.RestingValue = Main.Default_Relation_Resting_Value + value.RelationRestingValueModifier;
            Relation.RestingRange = Main.Default_Relation_Resting_Range + value.RelationRestingRangeModifier;
        }
    }

    public void CountryStatsDrift() // Country focus shifts towards leader focus
    {
        List<float> FocusValues = new List<float>();
        for (int i = 0; i < FocusTendencies.Length; i++)
            FocusValues.Add(FocusTendencies[i]);


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
        Focus = ActionManager.s_Focuses[System.Array.IndexOf(FocusTendencies, (FocusTendencies.Max()))];

    }

    #endregion

    #region Leaders



    public void PopulationRevolt()
    {
        if (IsPlayer) DeathManager.GameOver("Revolt", "The people of your country disliked you so much that they revolted against you");
        ChangeLeader(this);
    }

    public void ChangeLeader(Country modelCountry = null) // modelCountry = the country the new leader is similar to - no value given = random leader
    {
        var rel_New = new Dictionary<Country, Relation>(); // makes new list of relations
        foreach (Country relationCountry in Main.s_cnt_Players)
        {
            rel_New.Add(relationCountry, new Relation()); // initializes each one   
            if (modelCountry != null)
            {
                var modelCountryRelationValue = (modelCountry.Relations.ContainsKey(relationCountry)) ? modelCountry.Relations[relationCountry].Value : 35; // Models new relation off model country (for leader relation of model country, set to arbitrary value of 35)
                var newRelationValue = Random.Range(modelCountryRelationValue - 20, modelCountryRelationValue + 20);
                rel_New[relationCountry].RestingValue = newRelationValue; // makes new leaders opinions of other countries similar to the populations opinions of them
                rel_New[relationCountry].Value = newRelationValue;
            }
        }
        // TODO Replace with new DevTools function 
        var TotalRatio = modelCountry.FocusTendencies.Sum();
        int rand = Random.Range(0, TotalRatio + 1);
        int iteration = 0;
        foreach (int x in modelCountry.FocusTendencies)
        {
            if ((rand -= x) < 0) break;
            iteration++;
        }
        Focus foc_New = (modelCountry != null) ? ActionManager.s_Focuses[iteration] : DevTools.RandomListValue<Focus>(ActionManager.s_Focuses);
        Debug.Log("New leader for country " + ID + "(model country: " + modelCountry + "):");
        Debug.Log("\tnewrelations:");
        rel_New.ToList().ForEach(x => Debug.Log("\t\t" + x.Value));
        Debug.Log("\tnewfocus: " + foc_New.name);
        previousLeader = Leader;
        Leader = new Leader(TextGenerator.LeaderName(), rel_New, DevTools.RandomListValue<PersonalityType>(ActionManager.s_PersonalityTypes), foc_New);
        modelCountry.leaderRelations.Value = modelCountry.leaderRelations.RestingValue;
    }
    #endregion

    #region Territories
    public void UpdateTerritoryBenefits()
    {
        MoneyGain = Focus.MoneyModifier;
        WarPowerGain = Focus.WarPowerModifier;
        foreach (Territory territory in OwnedTerritories)
        {
            MoneyGain += territory.Terrain.MoneyProduction;
            WarPowerGain += territory.Terrain.WarPowerProduction;
        }
    }

    #endregion

    #region Utility
    public void PrintInfo()
    {
        Debug.Log("---------NEW COUNTRY-----------------");
        Debug.Log("\t" + name);
        Debug.Log("\tID:\t " + ID);
        Debug.Log("\tPlayer Country?\t" + IsPlayer);
        Debug.Log("\t----------LEADER---------- ");
        Debug.Log("\t\tName:\t" + Leader.Name);
        Debug.Log("\t\tFocus:\t" + Leader.Focus.name);
        Debug.Log("\t\tPersonality:\t" + Leader.Personality);
        Debug.Log("\t\t--------LEADER RELATIONS--------");
        Leader.Relations.Values.ToList().ForEach(x => Debug.Log("\t\t\t" + x.Value));
        Debug.Log("\t\t--------END LEADER RELATIONS--------");
        Debug.Log("\t----------END LEADER---------- ");
        Debug.Log("\t-------------RELATIONS-------------");
        Relations.Values.ToList().ForEach(x => Debug.Log("\t\t" + x.Value));
        Debug.Log("\t-------------END RELATIONS-------------");
        Debug.Log("---------END NEW COUNTRY-----------------");
    }
    #endregion
}

