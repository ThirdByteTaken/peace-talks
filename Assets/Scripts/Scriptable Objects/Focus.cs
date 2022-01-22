using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Focus : ScriptableObject
{
    public string Name;
    public int ID
    {
        get
        {
            return ActionManager.s_Focuses.IndexOf(this);
        }
    }
    public Sprite Sprite;
    [Header("Resource Modifiers")]
    public int MoneyModifier;
    public int WarPowerModifier;
    [Header("Relation Modifiers")]
    public int RelationDriftModifier;
    public int RelationGracePeriodModifier;
    public int RelationRestingValueModifier;
    public int RelationRestingRangeModifier;
}
