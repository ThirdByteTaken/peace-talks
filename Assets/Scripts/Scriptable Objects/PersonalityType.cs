using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PersonalityType : ScriptableObject
{
    public string Name;

    public Sprite Sprite;

    public int ID
    {
        get
        {
            return ActionManager.Instance.PersonalityTypes.IndexOf(this);
        }
    }
}
