using System;
using System.Collections;
using System.Collections.Generic;
using Music.Contex;
using UnityEngine;

public class WitchUserController
{
    private WitchMusicController _musicControllerRef;

    public struct UserInputState
    {
        public Vector2 Move;

        public bool Walk;
        public bool Dash;
        public bool Jump;

        public bool Interact;

        public Vector2 ToneHint;
        public int ToneExplicit;

        public bool RaiseNote;
        public bool LowerNote;
        public bool Alternate;
        public bool Stomp;
        
        public bool Chorus;
        public bool Major;
        public bool DoubleMajor;
        public bool Minor;
        public bool Seventh;
        public bool DoubleSeventh;
        public bool Ninth;
        public bool Hold;
        public bool Modulate;
    }

    
    public enum ToneViability
    {
        Ideal=0,
        Viable=1,
        Unusual=2
    }

    public enum ToneOffset
    {
        InScale=0,
        Natural=1,
        Flat=2,
        DoubleFlat=3,
        Sharp=4,
        DoubleSharp=5,
    }
    public struct ModalTone
    {
        public string text;
        public float weight;
        public ToneViability viability;
        public ToneOffset offset;
    }

    public struct ToneSet
    {
        public ModalTone[] tones;
    }

    public enum ActivationStyle
    {
        Default=0,
        Aux,
        AuxAndLower,
        Lower,
        LowerAndRaise,
        Raise,
        DoubleRaise,
        LowerAndDoubleRaise,
        NUM_ACTIVATION_STYLES
    }

    public enum ModifierStyle
    {
        Default=0,
        Seventh,
        DoubleSeventh,
        Modulate,
        NUM_MODIFIER_STYLES
    }

    public struct ChordEntry
    {
        public ModalTone root;
        public int[] semitonalOffsets;
    }

    public struct ChordPlaySpace
    {
        public ChordEntry[][] entriesByActivationAndModifierStyle;
    }

    public enum MusicalForm
    {
        Solo=0,
        Chorus,
    }
    
    public struct WitchMusicState
    {
        public MusicalForm form;
        public int activeStyleKey;
        public int activeTone;

        public ToneSet soloScale;
        public ToneSet soloChromatic;

        public ChordPlaySpace[] choralPlaySpaces;
    }

    private UserInputState _inputState;
    public UserInputState InputState => _inputState;

    private WitchMusicState _musicState;
    public WitchMusicState MusicState => _musicState;

    private ToneSelector _toneSelector;
    public ToneSelector Selector => _toneSelector;
    
    private WitchPlayerAvatar _targetAvatar;
    public WitchPlayerAvatar TargetAvatar => _targetAvatar;

    public WitchUserController(WitchMusicController musicControllerRef)
    {
        _musicControllerRef = musicControllerRef;
        _toneSelector = new ToneSelector(12);
    }
    
    public void AssignAvatar(WitchPlayerAvatar avatar)
    {
        _targetAvatar = avatar;
    }

    public void OverrideInputState(UserInputState inputState)
    {
        _inputState = inputState;
        if (_inputState.ToneHint.sqrMagnitude > 1.0f)
        {
            _inputState.ToneHint = _inputState.ToneHint.normalized;
        }
        //Debug.Log($"{inputState.Move}{inputState.ToneExplicit}");
    }

    public void Update()
    {
        int desiredStyleKey = GetStyleKey();
        if (desiredStyleKey != _musicState.activeStyleKey)
        {
            _musicState.activeStyleKey = desiredStyleKey;
            
            _toneSelector.ChangeDesiredTones(GetTonesForStyle(desiredStyleKey), true);
        }
        
        _toneSelector.Update();
    }

    public void ChangeContext(WitchManager.WitchMusicContext context)
    {
        BuildTones(context);
        
        _toneSelector.ChangeDesiredTones(GetTonesForStyle(_musicState.activeStyleKey), true);
    }

    public ModalTone[] GetTonesForStyle(int styleKey)
    {
        if (!_inputState.Chorus)
        {
            return _inputState.Alternate ? _musicState.soloChromatic.tones : _musicState.soloScale.tones;
        }

        return _musicState.soloScale.tones; // @TODO: Chords
    }

    public int GetStyleKey()
    {
        if (!_inputState.Chorus)
        {
            return (_inputState.Alternate ? 2 : 0);
        }

        return 1 +
               (_inputState.Major ? 2 : 0) +
               (_inputState.DoubleMajor ? 4 : 0) +
               (_inputState.Minor ? 8 : 0) +
               (_inputState.DoubleSeventh ? 16 : 0) +
               (_inputState.Seventh ? 32 : 0) +
               (_inputState.Ninth ? 64 : 0) +
               (_inputState.Modulate ? 128 : 0);
    }
    
    private void BuildTones(WitchManager.WitchMusicContext context)
    {
        int letters = context.temperament.noteLetters;
        int semitones = context.temperament.semitoneCount;
        _musicState.soloScale.tones = new ModalTone[letters];
        _musicState.soloChromatic.tones = new ModalTone[semitones];

        int tonicLetterIndex = 0;
        int tonicAccidentals = 0;
        
        if (context.temperament.rootLetterSpacing[context.keyIndex] == '-' ||
            context.temperament.rootLetterSpacing[context.keyIndex] == '+')
        {
            int targetSemitone = context.keyIndex;
            if (context.preferFlatOverSharp)
            {
                while (context.temperament.rootLetterSpacing[targetSemitone] == '-' ||
                       context.temperament.rootLetterSpacing[targetSemitone] == '+')
                {
                    tonicAccidentals--;
                    targetSemitone = (targetSemitone + 1) % semitones;
                }
            }
            else
            {
                while (context.temperament.rootLetterSpacing[targetSemitone] == '-' ||
                       context.temperament.rootLetterSpacing[targetSemitone] == '+')
                {
                    tonicAccidentals++;
                    targetSemitone = (targetSemitone + semitones - 1) % semitones;
                }
            }
            tonicLetterIndex = (context.temperament.rootLetterSpacing[targetSemitone] - 'A');
        }
        else
        {
            tonicLetterIndex = (context.temperament.rootLetterSpacing[context.keyIndex] - 'A');
            tonicAccidentals = 0;
        }

        int lastInterval = context.mode.tempermentSpacing[0] - '1';
        int currentLetterIndex = tonicLetterIndex;

        string tonicName = ((char) ('A' + currentLetterIndex)).ToString();
        for (int j = 0; j < tonicAccidentals; ++j) { tonicName += "#"; }
        for (int j = 0; j > tonicAccidentals; --j) { tonicName += "b"; }

        int scaleNote = 0;
        ModalTone tonic = new ModalTone 
            { text=tonicName, weight=1.5f, viability=ToneViability.Viable, offset=ToneOffset.InScale };
        _musicState.soloScale.tones[scaleNote++] = tonic;
        _musicState.soloChromatic.tones[0] = tonic;

        for (int i = 1; i < semitones; ++i)
        {
            int currentUnboundSemitone = (context.keyIndex + i);

            char semitoneChar = context.mode.tempermentSpacing[i];
            bool isInScale = (semitoneChar != '-' && semitoneChar != '+');
            
            if (isInScale)
            {
                int currentInterval = semitoneChar - '1';
                int delta = currentInterval - lastInterval;
                currentLetterIndex += delta;
                lastInterval = currentInterval;
            }

            int targetLetter = currentLetterIndex + ((semitoneChar == '+') ? 1 : 0);
            int semitoneIndexOfCurrentLetter = context.temperament.rootLetterSpacing.IndexOf((char)(targetLetter % letters + 'A'));
            int currentAccidentals = (currentUnboundSemitone - semitoneIndexOfCurrentLetter);
            int pivot = semitones / 2;
            while (currentAccidentals > pivot) currentAccidentals -= semitones;
            while (currentAccidentals < pivot - semitones) currentAccidentals += semitones;


            string noteName = ((char)(targetLetter % letters + 'A')).ToString();
            for (int j = 0; j < currentAccidentals; ++j) { noteName += '#'; }
            for (int j = 0; j > currentAccidentals; --j) { noteName += 'b'; }

            ToneOffset offset = ToneOffset.InScale;
            float noteWeight = (semitoneChar=='5') ? 1.5f : 1.0f;
            if (isInScale)
            {
                _musicState.soloScale.tones[scaleNote++] = new ModalTone 
                    { text=noteName, weight=noteWeight, viability=ToneViability.Viable, offset=offset };
            }
            else
            {
                noteWeight = 0.66f;

                if (currentAccidentals == 0)
                {
                    noteName += 'n';
                }

                offset = ToneOffset.Natural;
                if( currentAccidentals > 1 ) {offset = ToneOffset.DoubleSharp;}
                else if( currentAccidentals < -1 ) {offset = ToneOffset.DoubleFlat;}
                else if( currentAccidentals > 0 ) {offset = ToneOffset.Sharp;}
                else if( currentAccidentals < 0 ) {offset = ToneOffset.Flat;}
            }

            _musicState.soloChromatic.tones[i] = new ModalTone 
                { text=noteName, weight=noteWeight, viability=ToneViability.Viable, offset=offset };
        }
    }

}
