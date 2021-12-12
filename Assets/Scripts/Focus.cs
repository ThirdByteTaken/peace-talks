using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Focus : ScriptableObject
{
    public int ID;
    [Header("Resource Modifiers")]
    public int MoneyModifier;
    public int WarPowerModifier;
    [Header("Relation Modifiers")]
    public int RelationDriftModifier;
    public int RelationGracePeriodModifier;
    public int RelationRestingValueModifier;
    public int RelationRestingRangeModifier;
}
