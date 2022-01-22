using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Relation
{
    private int _value = 0;
    public int Value
    {
        get { return _value; }
        set
        {
            value = Mathf.Clamp(value, -100, 100);
            int changeValue = value - Value;

            if (restingValue != 0)
                if (!((IsDrifting) && (changeValue > 0) == (Value < RestingMin))) // Unless it is currently drifting in the direction of the change                
                    CurrentGracePeriod = ResetGracePeriod; // Reset the grace period                                    

            if (value <= -100)
                DeathManager.GameOver("War", "War has broken out");

            _value = value;
        }
    }
    public int DriftSpeed = Main.Default_Relation_Drift_Rate;
    public int ResetGracePeriod = Main.Default_Relation_Grace_Period;
    public int CurrentGracePeriod = Main.Default_Relation_Grace_Period;
    private int restingValue = Main.Default_Relation_Resting_Value;
    public int RestingValue
    {
        get { return restingValue; }
        set
        {
            RestingMax = value + restingRange;
            RestingMin = value - restingRange;
            restingValue = value;
        }
    }
    private int restingRange = Main.Default_Relation_Resting_Range;
    public int RestingRange
    {
        get { return restingRange; }
        set
        {
            RestingMax = restingValue + value;
            RestingMin = restingValue - value;
            restingRange = value;
        }
    }
    public int RestingMin = Main.Default_Relation_Resting_Value - Main.Default_Relation_Resting_Range;
    public int RestingMax = Main.Default_Relation_Resting_Value + Main.Default_Relation_Resting_Range;



    public bool IsDrifting
    {
        get
        {
            return (!(Value >= RestingMin && Value <= RestingMax) && CurrentGracePeriod <= 0);
        }
    }
}


