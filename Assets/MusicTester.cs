using System.Collections.Generic;
using Music.Contex;
using Music.Support;
using UnityEngine;

[RequireComponent(typeof(AudioPlay))]
public class MusicTester : MonoBehaviour
{
    public string tempermentString;
    public string modeString;
    public string keyString;

    private MusicalContext context;
    public MusicalContext Context => context;
    private AudioPlay _audioPlay = null;
    
    void Start()
    {
        _audioPlay = GetComponent<AudioPlay>();
        
        Rebuild();
    }

    public void Rebuild()
    {
        context = null;
        if (string.IsNullOrEmpty(tempermentString) ||
            string.IsNullOrEmpty(modeString) ||
            string.IsNullOrEmpty(keyString))
        {
            return;
        }
        Temperament temperament = new Temperament(tempermentString);
        Mode mode = Mode.CreateFromString(temperament, modeString);
        NoteInstance key = TemperamentUtil.GetNoteForString(temperament, keyString);

        context = new MusicalContext(temperament, mode, key);
    }

    public void PlaySemitone(int semitone, int octave)
    {
        int octaveWidth = context.Temperament.Notes.Length;
        int semitoneDelta = semitone + octave * octaveWidth;
        _audioPlay.PlaySemitone(semitoneDelta, octaveWidth);
    }
}
