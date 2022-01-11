using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader
{
    public string Name;

    public List<Relation> Relations; // Relations with other countries    

    public PersonalityTypes Personality;
    public Focus Focus;

    public Leader(string name, List<Relation> relations, PersonalityTypes personality, Focus focus)
    {
        Name = name;
        Relations = new List<Relation>(relations);
        Personality = personality;
        Focus = focus;
    }

    public void UpdateFocusModifiers(Focus value)
    {

    }
}
public enum PersonalityTypes
{
    Angry,
    Neutral,
    Peaceful
}
