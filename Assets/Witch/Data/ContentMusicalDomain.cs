using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MusicalDomain", menuName = "ScriptableObjects/ContentMusicalDomain", order = 1)]
public class ContentMusicalDomain : ScriptableObject
{

    public MusicalTemperament[] temperaments;
    public MusicalMode[] modes;
    public string defaultMode;
}
