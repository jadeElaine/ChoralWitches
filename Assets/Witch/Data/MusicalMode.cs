using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MusicalChordDefinition
{
    [System.Serializable]
    public enum ChordComposition
    {
        None=0,

        AutoDetectTriad,
        AutoDetectSeventh,
        AutoDetectFifth,
        
        AutoDetectAddNine,
        AutoDetectNinth,
        AutoDetectThirteenth,
        AutoDetectSixth,
        
        AutoDetectSusTwo,
        AutoDetectSusTwoSeventh,
        AutoDetectSusFour,
        AutoDetectSusFourSeventh,
        
        Diminished,
        DiminishedSeventh,
        HalfDiminishedSeventh,
        HalfDiminishedSeventhFlatNine,
        
        SusTwo,
        SusTwoMinorSeventh,
        SusTwoMajorSeventh,
        SusFour,
        SusFourMinorSeventh,
        SusFourMajorSeventh,
        SusTwoSusFour,
        
        Minor,
        MinorMinorSeventh,
        MinorMajorSeventh,
        MinorAddNine,
        MinorMinorSeventhNinth,
        MinorMajorSeventhNinth,
        MinorMinorSeventhThirteenth,
        MinorMajorSeventhThirteenth,
        MinorFlatSix,
        MinorSixth,

        Major,
        MajorMinorSeventh,
        MajorMajorSeventh,
        MajorAddNine,
        MajorMinorSeventhNinth,
        MajorMajorSeventhNinth,
        MajorMinorSeventhThirteenth,
        MajorMajorSeventhThirteenth,
        MajorFlat6,
        MajorSixth,
        
        Augmented,
        AugmentedMajorSeventh,
        
        AugmentedSixthItalian,
        AugmentedSixthGerman,
        AugmentedSixthFrench,
    }

    [System.Serializable]
    public enum ModifierState
    {
        Off=0,
        On,
        OnRevokable,
        Double,
    }
    
    public static readonly string[] CHORD_COMPOSITION_AUTOS =
    {
        "", "1|3|5", "1|3|5|7", "1|5",
        "1|3|5|9", "1|3|5|7|9", "1|3|5|7|13", "1|3|5|6",
        "1|2|5", "1|2|5|7", "1|4|5", "1|4|5|7"
    };

    public static readonly string[] CHORD_COMPOSITION_SEMITONES =
    {
        "", "", "", "",
        "", "", "", "",
        "", "", "", "", 
        "0|3|6", "0|3|6|9", "0|3|6|10", "0|3|6|10|13",
        "0|2|7", "0|2|7|10", "0|2|7|11", "0|5|7", "0|5|7|10", "0|5|7|11", "0|2|5|7",
        
        "0|3|7","0|3|7|10","0|3|7|11",
        "0|3|7|14","0|3|7|10|14","0|3|7|11|14",
        "0|3|7|10|14|21","0|3|7|11|14|21",
        "0|3|7|8","0|3|7|9",
        
        "0|4|7","0|4|7|10","0|4|7|11",
        "0|4|7|14","0|4|7|10|14","0|4|7|11|14",
        "0|4|7|10|14|21","0|4|7|11|14|21",
        "0|4|7|8","0|4|7|9",
        
        "0|4|8","0|4|8|11",
        
        "0|4|10","0|4|7|10","0|4|6|10"
    };

    public ChordComposition composition;
    public int semitoneOffset;

    public ModifierState Lower; 
    public ModifierState Raise; 
    public ModifierState Aux; 
    public ModifierState Seventh;
    public ModifierState Modulate;
    public bool inStateTransferable;
    public int[] transitionWeights;
}

[System.Serializable]
public struct MusicalModalChordSetPlaySpace
{
    public MusicalChordDefinition[] chords;
}

[System.Serializable]
public struct MusicalMode
{
    public string name;
    public int tempermentIndex;
    public string tempermentSpacing;
    public Vector2Int uiPlacement;
    public MusicalModalChordSetPlaySpace[] chordGroups;
}
