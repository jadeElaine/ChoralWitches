using System.Collections.Generic;
using Music.Contex;
using Music.Support;
using UnityEngine;

[RequireComponent(typeof(AudioPlay))]
[RequireComponent(typeof(InputCapture))]
public class MusicMechanic : MonoBehaviour
{
    private MusicalContext _ctx = null;
    private InputCapture _input = null;
    private AudioPlay _audioPlay = null;
    private Dictionary<int, AudioSource> _currentlyPlaying = new Dictionary<int,AudioSource>();

    // X -X -X X-X -X -X
    // 12Tet: T (-X/X-) (-X/X-) (-XP/P-P/PX-) (-X/X-) (-X/X-)
    //
    // Slotting:
    // key at front - 7 slots: []
    //
    [SerializeField]
    private string _defaultTempermentSpacing = "X-XX-X-XX-X-";
    [SerializeField]
    private string _defaultKeyLetter = "C";
    [SerializeField]
    private int _mode = 0;

    //[SerializeField]
    //private int _key = 0;

    public MusicalContext Context => _ctx;

    void Awake()
    {
        _ctx = MusicalContext.Create(
            _defaultTempermentSpacing,
            (ModesHeptatonic)_mode, 
            _defaultKeyLetter);
    }

    void Start()
    {
        _audioPlay = GetComponent<AudioPlay>();
        _input = GetComponent<InputCapture>();
        _input.down += OnBeginNote;
        _input.up += OnEndNote;
    }

    void LateUpdate()
    {
        // clean finished sounds
        var keys = _currentlyPlaying.Keys;
        foreach (var k in keys)
        {
            if (!_currentlyPlaying[k].isPlaying)
            {
                _currentlyPlaying.Remove(k);
            }
        }
    }

    private void OnBeginNote(in NoteTriggers trigger)
    {
        Debug.Log($"Beginning {trigger.ToString()}");
        switch (trigger)
        {
            case NoteTriggers.EXPLICIT_UNISON: BeginScaleNote(0); break;
            
            case NoteTriggers.EXPLICIT_BELOW_OCTAVE: BeginOctaveNote(-1); break;
            case NoteTriggers.EXPLICIT_ABOVE_OCTAVE: BeginOctaveNote(1); break;
            
            case NoteTriggers.SCALE_DOWN_1: BeginScaleNote(-1); break;
            case NoteTriggers.SCALE_DOWN_2: BeginScaleNote(-2); break;
            case NoteTriggers.SCALE_DOWN_3: BeginScaleNote(-3); break;
            case NoteTriggers.SCALE_DOWN_4: BeginScaleNote(-4); break;
            case NoteTriggers.SCALE_DOWN_5: BeginScaleNote(-5); break;
            case NoteTriggers.SCALE_DOWN_6: BeginScaleNote(-6); break;
            case NoteTriggers.SCALE_UP_1: BeginScaleNote(1); break;
            case NoteTriggers.SCALE_UP_2: BeginScaleNote(2); break;
            case NoteTriggers.SCALE_UP_3: BeginScaleNote(3); break;
            case NoteTriggers.SCALE_UP_4: BeginScaleNote(4); break;
            case NoteTriggers.SCALE_UP_5: BeginScaleNote(5); break;
            case NoteTriggers.SCALE_UP_6: BeginScaleNote(6); break;
            
            case NoteTriggers.ACCIDENTAL_DOWN_1: BeginAccidentalNote(-1); break;
            case NoteTriggers.ACCIDENTAL_DOWN_2: BeginAccidentalNote(-2); break;
            case NoteTriggers.ACCIDENTAL_DOWN_3: BeginAccidentalNote(-3); break;
            case NoteTriggers.ACCIDENTAL_DOWN_4: BeginAccidentalNote(-4); break;
            case NoteTriggers.ACCIDENTAL_DOWN_5: BeginAccidentalNote(-5); break;
            case NoteTriggers.ACCIDENTAL_UP_1: BeginAccidentalNote(1); break;
            case NoteTriggers.ACCIDENTAL_UP_2: BeginAccidentalNote(2); break;
            case NoteTriggers.ACCIDENTAL_UP_3: BeginAccidentalNote(3); break;
            case NoteTriggers.ACCIDENTAL_UP_4: BeginAccidentalNote(4); break;
            case NoteTriggers.ACCIDENTAL_UP_5: BeginAccidentalNote(5); break;
            case NoteTriggers.ACCIDENTAL_UP_6: BeginAccidentalNote(6); break;
            case NoteTriggers.ACCIDENTAL_UP_7: BeginAccidentalNote(7); break;
        }
    }
    private void OnEndNote(in NoteTriggers trigger)
    {
        Debug.Log($"Ending {trigger.ToString()}");
        switch (trigger)
        {
            case NoteTriggers.EXPLICIT_UNISON: EndScaleNote(0); break;
            
            case NoteTriggers.EXPLICIT_BELOW_OCTAVE: EndOctaveNote(-1); break;
            case NoteTriggers.EXPLICIT_ABOVE_OCTAVE: EndOctaveNote(1); break;
            
            case NoteTriggers.SCALE_DOWN_1: EndScaleNote(-1); break;
            case NoteTriggers.SCALE_DOWN_2: EndScaleNote(-2); break;
            case NoteTriggers.SCALE_DOWN_3: EndScaleNote(-3); break;
            case NoteTriggers.SCALE_DOWN_4: EndScaleNote(-4); break;
            case NoteTriggers.SCALE_DOWN_5: EndScaleNote(-5); break;
            case NoteTriggers.SCALE_DOWN_6: EndScaleNote(-6); break;
            case NoteTriggers.SCALE_UP_1: EndScaleNote(1); break;
            case NoteTriggers.SCALE_UP_2: EndScaleNote(2); break;
            case NoteTriggers.SCALE_UP_3: EndScaleNote(3); break;
            case NoteTriggers.SCALE_UP_4: EndScaleNote(4); break;
            case NoteTriggers.SCALE_UP_5: EndScaleNote(5); break;
            case NoteTriggers.SCALE_UP_6: EndScaleNote(6); break;
            
            case NoteTriggers.ACCIDENTAL_DOWN_1: EndAccidentalNote(-1); break;
            case NoteTriggers.ACCIDENTAL_DOWN_2: EndAccidentalNote(-2); break;
            case NoteTriggers.ACCIDENTAL_DOWN_3: EndAccidentalNote(-3); break;
            case NoteTriggers.ACCIDENTAL_DOWN_4: EndAccidentalNote(-4); break;
            case NoteTriggers.ACCIDENTAL_DOWN_5: EndAccidentalNote(-5); break;
            case NoteTriggers.ACCIDENTAL_UP_1: EndAccidentalNote(1); break;
            case NoteTriggers.ACCIDENTAL_UP_2: EndAccidentalNote(2); break;
            case NoteTriggers.ACCIDENTAL_UP_3: EndAccidentalNote(3); break;
            case NoteTriggers.ACCIDENTAL_UP_4: EndAccidentalNote(4); break;
            case NoteTriggers.ACCIDENTAL_UP_5: EndAccidentalNote(5); break;
            case NoteTriggers.ACCIDENTAL_UP_6: EndAccidentalNote(6); break;
            case NoteTriggers.ACCIDENTAL_UP_7: EndAccidentalNote(7); break;
        }
    }

    private void BeginScaleNote(int x)
    {
        int globalSemitone = _ctx.GetAbsoluteSemitoneForUnboundScaleDegree(x);
        BeginSemitone(globalSemitone);
    }
    private void BeginAccidentalNote(int x)
    {
        
    }
    private void BeginOctaveNote(int x)
    {
        int globalSemitone = _ctx.GetAbsoluteSemitoneForUnboundScaleDegree(0) + _ctx.GetTempermentWidth() * x;
        BeginSemitone(globalSemitone);
    }

    private void EndScaleNote(int x)
    {
        int globalSemitone = _ctx.GetAbsoluteSemitoneForUnboundScaleDegree(x);
        EndSemitone(globalSemitone);
    }
    private void EndAccidentalNote(int x)
    {
        
    }
    private void EndOctaveNote(int x)
    {
        int globalSemitone = _ctx.GetAbsoluteSemitoneForUnboundScaleDegree(0) + _ctx.GetTempermentWidth() * x;
        EndSemitone(globalSemitone);
    }

    private void BeginSemitone(int x)
    {
        AudioSource voice = _audioPlay.PlaySemitone(x, _ctx.GetTempermentWidth());
        _currentlyPlaying[x] = voice;
    }
    private void EndSemitone(int x)
    {
        if (_currentlyPlaying.ContainsKey(x))
        {
            _currentlyPlaying[x].Stop();
            _currentlyPlaying.Remove(x);
        }
    }
}
