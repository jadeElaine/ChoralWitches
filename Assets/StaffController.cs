using System.Collections.Generic;
using Music.Contex;
using Music.Support;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffController : MonoBehaviour
{
    [SerializeField] private Image notePrimitive;
    [SerializeField] private TMP_Text labelPrimitive;

    [SerializeField] private Image[] staff;
    [SerializeField] private int staffCentralTone = 3;

    private Dictionary<int, int> semitoneToStaffPosition = new Dictionary<int, int>();
    private Dictionary<int, int> staffPositionToSemitone = new Dictionary<int, int>();

    private List<TMP_Text> helperLabels = new List<TMP_Text>();

    private Vector3 staffOrigin;
    private float staffLineOffset;
    
    private MusicMechanic _mechanic = null;

    void Start()
    {
        _mechanic = FindObjectOfType<MusicMechanic>();
        UpdateHelpFromContext(_mechanic.Context);

        staffOrigin = staff[0].rectTransform.position + 
                      new Vector3(-staff[0].flexibleWidth / 2, 0, 0);
        staffLineOffset = (staff[1].rectTransform.position.y - staff[0].transform.position.y) / 2;
    }

    private void UpdateHelpFromContext(in MusicalContext context)
    {
        semitoneToStaffPosition.Clear();
        staffPositionToSemitone.Clear();
        int scaleWidth = context.Temperament.AnchorNotes.Length;
        int tempermentWidth = context.GetTempermentWidth();
        for (int i = 0; i < tempermentWidth; ++i)
        {
            int semitone = i + context.Key.AbsoluteSemitoneInTemperment;
            int dictIndex = i % context.GetTempermentWidth();
            if (context.NoteDictionary.ContainsKey(dictIndex))
            {
                MusicalContext.ContextualNoteEntry cne = context.NoteDictionary[dictIndex]; 
                semitoneToStaffPosition[semitone - tempermentWidth] =
                    cne.Note.ScaleIndex + context.Key.ScaleIndex - scaleWidth;
                semitoneToStaffPosition[semitone] =
                    cne.Note.ScaleIndex + context.Key.ScaleIndex;
                semitoneToStaffPosition[semitone + tempermentWidth] =
                    cne.Note.ScaleIndex + context.Key.ScaleIndex + scaleWidth;

                if (cne.IsBaseScale)
                {
                    staffPositionToSemitone[cne.Note.ScaleIndex + context.Key.ScaleIndex - scaleWidth] = semitone - tempermentWidth;
                    staffPositionToSemitone[cne.Note.ScaleIndex + context.Key.ScaleIndex] = semitone;
                    staffPositionToSemitone[cne.Note.ScaleIndex + context.Key.ScaleIndex + scaleWidth] = semitone + tempermentWidth;
                }
            }
        }
        
        
        for (int staffPosition = -4; staffPosition <= 12; ++staffPosition)
        {
            if (staffPositionToSemitone.ContainsKey(staffPosition))
            {

                TMP_Text obj = Instantiate(
                    labelPrimitive,
                    staffOrigin + new Vector3(0,staffLineOffset * staffPosition,0),
                    Quaternion.identity);
                obj.transform.SetParent(transform,true);
                
                int semitone = staffPositionToSemitone[staffPosition];
                int toneIndex = (staffPosition - staffCentralTone + scaleWidth*10) % scaleWidth;
                int octaveIndex = (staffPosition - staffCentralTone + scaleWidth*10) / scaleWidth - 10;
                int accidental = (semitone % tempermentWidth) - context.Temperament.AnchorNotes[toneIndex];
                //Debug.Log($"semitone: {semitone} ... tone/octave: {toneIndex} / {octaveIndex} ... with accidental {accidental}");
                NoteInstance semitoneNote = new NoteInstance(semitone, toneIndex, accidental);

                //obj.text = TemperamentUtil.GetStringForNote(context.Temperament, semitoneNote);
                
                helperLabels.Add(obj);
            }
        }
    }
}
