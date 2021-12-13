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
            if (value.Value <= -100) PopulationRevolt();
            leaderRelations = value;
        }
    }

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

    public Sprite Flag;
    public Color textColor;

    public void UpdateFocusModifiers(Focus value)
    {
        MoneyGain = Main.Default_Money_Gain + value.MoneyModifier;
        WarPowerGain = Main.Default_WarPower_Gain + value.WarPowerModifier;
        foreach (Relation Relation in Relations)
        {
            Relation.DriftSpeed = Main.Default_Relation_Drift_Rate + value.RelationDriftModifier;
            Relation.GracePeriod = Main.Default_Relation_Grace_Period + value.RelationGracePeriodModifier;
            Relation.RestingValue = Main.Default_Relation_Resting_Value + value.RelationRestingValueModifier;
            Relation.RestingRange = Main.Default_Relation_Resting_Range + value.RelationRestingRangeModifier;
        }
        if (ID != -1)
        {
            PlayerRelations.DriftSpeed = Main.Default_Relation_Drift_Rate + value.RelationDriftModifier;
            PlayerRelations.GracePeriod = Main.Default_Relation_Grace_Period + value.RelationGracePeriodModifier;
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
            FocusValues[i] *= (newWeight / (float)oldWeight);


        List<float> FocusValueRemainders = FocusValues.ConvertAll(x => x %= 1);
        for (int i = 0; i < FocusValues.Count; i++)
            FocusValues[i] = Mathf.Floor(FocusValues[i]);

        while (FocusValues.Sum() < newWeight)
        {
            int maxIndex = FocusValueRemainders.IndexOf(FocusValueRemainders.Max());
            FocusValues[maxIndex]++;
            FocusValueRemainders[maxIndex] = 0; // So it wont be accessed as max value
        }

        for (int i = 0; i < FocusValues.Count; i++)
        {
            FocusTendencies[i + ((i >= Leader.Focus.ID) ? 1 : 0)] = (int)FocusValues[i];
        }

        if (FocusTendencies.IndexOf(FocusTendencies.Max()) == Leader.Focus.ID) Focus = Leader.Focus;
    }

    public void PopulationRevolt()
    {

    }
}

