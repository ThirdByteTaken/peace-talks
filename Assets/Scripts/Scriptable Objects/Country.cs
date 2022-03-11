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


    #endregion

    public void PopulationRevolt()
    {
        if (IsPlayer) DeathManager.GameOver("Revolt", "The people of your country disliked you so much that they revolted against you");
    }

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
        Debug.Log("\t-------------RELATIONS-------------");
        Relations.Values.ToList().ForEach(x => Debug.Log("\t\t" + x.Value));
        Debug.Log("\t-------------END RELATIONS-------------");
        Debug.Log("---------END NEW COUNTRY-----------------");
    }
    #endregion
}

