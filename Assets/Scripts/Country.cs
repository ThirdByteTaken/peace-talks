using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public string LeaderName;

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


    private Relation[] relations; // Relations with other countries
    public Relation[] Relations
    {
        get
        {
            return relations;
        }
        set
        {
            /*Debug.Log("FDF");
            if (relations != null)
                foreach (Relation relation in value)
                {
                    /*if (relation.RestingMin < relation.Value && relation.Value < relation.RestingMax) // If in resting range
                        relation.DriftSpeed = 0; // Stop drifting
                    else if (relation.GracePeriod == 0) // If drifting can occur from current position and the grace period is up
                        relation.DriftSpeed = (((relation.Value > relation.RestingValue) ? -1 : 1)) * Main.Default_Relation_Drift_Rate; // Set drifting value to proper sign based on current value
                        *//*
                    int changeValue = relation.Value - Relations[System.Array.IndexOf(value, relation)].Value;
                    Debug.Log("chng: " + changeValue);
                    if (!((relation.IsDrifting) && (changeValue > 0) == (relation.Value < relation.RestingMin))) // Unless it is currently drifting in the direction of the change
                    {
                        relation.GracePeriod = Main.Default_Relation_Grace_Period; // Reset the grace period
                        Debug.Log("Grace Period reset");
                    }
                    if (relation.Value <= -100)
                    {
                        DeathManager.GameOver("War", "War has broken out");
                    }
                }*/
            relations = value;
        }
    }

    public List<Country> cnt_RecentlyInteracted;

    public int MoneyGain = 10;
    public int WarPowerGain = 10;

    //public const int RelationDrift = 5;
    //public const int RelationGracePeriod = 5;
    //public const int RelationRestingValue = 0;
    //public const int RelationRestingRange = 20;

    public PersonalityTypes LeaderPersonality;
    [SerializeField]
    private Focus leaderFocus;
    public Focus LeaderFocus
    {
        get { return leaderFocus; }
        set
        {
            UpdateFocusModifiers(value);
            leaderFocus = value;
        }
    }
    public Sprite Flag;
    public Color textColor;

    public void UpdateFocusModifiers(Focus value)
    {
        Debug.Log(value.name + name);
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
}
public enum PersonalityTypes
{
    Angry,
    Neutral,
    Peaceful
}
