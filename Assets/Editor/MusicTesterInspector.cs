using System;
using System.Collections.Generic;
using Music.Contex;
using Music.Support;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MusicTester))]
public class MusicTesterInspector : Editor
{
    private string[] _allModes = null;
    private string[] _allKeys = null;
    
    public override void OnInspectorGUI()
    {
        MusicTester tgt = target as MusicTester;

        const int captionIndent=85;
        
        bool changed = false;
        string oldTemperment = tgt.tempermentString;
        string oldMode = tgt.modeString;
        string oldKey = tgt.keyString;
        
        if (_allModes == null) 
        { RebuildAllModes(); }
        if (_allKeys == null && !string.IsNullOrWhiteSpace(oldTemperment))
        {RebuildAllKeys(new Temperament(tgt.tempermentString));}

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Temperment",GUILayout.Width(captionIndent));
        tgt.tempermentString = EditorGUILayout.TextField(oldTemperment);
        if (tgt.tempermentString != oldTemperment)
        {
            RebuildAllKeys(new Temperament(tgt.tempermentString));
            tgt.keyString = "";
            changed = true;
        }
        EditorGUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(tgt.tempermentString))
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mode", GUILayout.Width(captionIndent));
            int currentModeIndex = Array.IndexOf(_allModes, oldMode);
            int newModeIndex = EditorGUILayout.Popup(currentModeIndex, _allModes);
            if (currentModeIndex != newModeIndex)
            {
                tgt.modeString = (newModeIndex == -1) ? "" : _allModes[newModeIndex];
                changed = true;
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("Mode [?]",GUILayout.Width(captionIndent));
        }

        if (_allKeys != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key",GUILayout.Width(captionIndent));
            int currentKeyIndex = Array.IndexOf(_allKeys, oldKey);
            int newKeyIndex = EditorGUILayout.Popup(currentKeyIndex, _allKeys);
            if (currentKeyIndex != newKeyIndex)
            {
                tgt.keyString = (newKeyIndex == -1) ? "" : _allKeys[newKeyIndex];
                changed = true;
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("Key [?]",GUILayout.Width(captionIndent));
        }
        
        if(changed)
        {
            EditorUtility.SetDirty(tgt);
            tgt.Rebuild();
        }

        EditorGUILayout.Space(12);
        if (tgt.Context != null && Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            //string[] numerals = { "I", "bII", "II", "bIII", "III", "IV", "tt", "V", "bVI", "VI", "bVII", "VII" };
            for (int i = 0; i <= tgt.Context.Temperament.Notes.Length*2; ++i)
            {
                int semitone = i % tgt.Context.Temperament.Notes.Length;
                if (tgt.Context.NoteDictionary.ContainsKey(semitone))
                {
                    MusicalContext.ContextualNoteEntry note = tgt.Context.NoteDictionary[semitone];
                    EditorGUILayout.BeginHorizontal();

                    bool inScale = note.IsBaseScale; 
                    if (inScale)
                    {
                        GUILayout.Label($"{Music.Support.Const.GetAccidentalName(note.AccidentalRelativeToMajor)}{Temperament.INTERVAL_NUMERALS[note.Note.ScaleIndex]}",GUILayout.Width(30));
                    }
                    else
                    {
                        GUILayout.Label("", GUILayout.MaxWidth(80));
                    }

                    Color originalColor = GUI.color;
                    if (note.IsBlue)
                    {
                        GUI.color = Color.blue;
                    }
                    else if (note.IsAscendingMelodic)
                    {
                        GUI.color = new Color(1.0f, 0.5f, 0.0f);
                    }
                    else if (note.IsDescendingResolution)
                    {
                        GUI.color = new Color(0.0f, 0.5f, 1.0f);
                    }
                    else if (note.FromAltScale)
                    {
                        GUI.color = new Color(1.0f, 1.0f, 0.0f);
                    }

                    if(GUILayout.Button(TemperamentUtil.GetStringForNote(tgt.Context.Temperament, note.Note), GUILayout.Width(70)))
                    {
                        
                        tgt.PlaySemitone(note.Note.AbsoluteSemitoneInTemperment, i/tgt.Context.Temperament.Notes.Length);
                    }

                    GUI.color = originalColor;

                    if (inScale)
                    {
                        GUILayout.Label("");
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    void RebuildAllKeys(Temperament temperament)
    {
        List<string> keys = new List<string>();
        foreach (int index in temperament.AnchorNotes)
        {
            keys.Add(temperament.Notes[index].FormalName + "b");
            keys.Add(temperament.Notes[index].FormalName);
            keys.Add(temperament.Notes[index].FormalName + "#");
        }
        _allKeys = keys.ToArray();
    }

    void RebuildAllModes()
    {
        List<string> modes = new List<string>();
        modes.AddRange(Enum.GetNames(typeof(ModesHeptatonic)));
        modes.AddRange(Enum.GetNames(typeof(ModesPentatonic)));
        modes.AddRange(Enum.GetNames(typeof(ModesQuadrastep)));
        _allModes = modes.ToArray();
    }
}
