using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader
{
    public string Name;

    public Dictionary<Country, Relation> Relations; // Relations with other countries    

    public PersonalityType Personality;
    public Focus Focus;

    public Leader(string name, Dictionary<Country, Relation> relations, PersonalityType personality, Focus focus)
    {
        Name = name;
        Relations = new Dictionary<Country, Relation>(relations);
        Personality = personality;
        Focus = focus;
    }

    public void UpdateFocusModifiers(Focus value)
    {

    }
}
