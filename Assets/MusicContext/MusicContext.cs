using System;
using System.Collections.Generic;
using Music.Support;
using UnityEngine;

namespace Music.Contex
{
    // NOT MUSICAL - GAME CONTEXT
    //public enum Auras { Red=0, Purple, Blue, Green }
    //public static readonly NoteRasterization[] AuraRootKeyIonian = {Notes.G, Notes.C, Notes.F, Notes.DsEf};

    public class MusicalContext : IDisposable
    {
        public class ContextualNoteEntry
        {
            public NoteInstance Note { get; }
            public bool IsBlue { get; }
            public bool IsAscendingMelodic { get; }
            public bool IsDescendingResolution { get; }
            public bool FromAltScale { get; }

            public bool IsBaseScale => !IsBlue && !IsAscendingMelodic && !IsDescendingResolution && !FromAltScale;
            public int AccidentalRelativeToMajor { get; }

            public ContextualNoteEntry(in NoteInstance note_, in bool isBlue_, in bool isAscendingMelodic_,
                in bool isDescendingResolution_, in bool fromAltScale_, in int accidentalRelativeToMajor_)
            {
                Note = note_;
                IsBlue = isBlue_;
                IsAscendingMelodic = isAscendingMelodic_;
                IsDescendingResolution = isDescendingResolution_;
                FromAltScale = fromAltScale_;
                AccidentalRelativeToMajor = accidentalRelativeToMajor_;
            }
        }

        public Temperament Temperament { get; }
        public Mode Mode { get; }
        public NoteInstance Key { get; }

        public Dictionary<int, ContextualNoteEntry> NoteDictionary;

        public int[] KeyRelativeNoteSemitones { get; }
        public int[] KeyRelativeAdjacentMajorNoteSemitones { get; }

        public NoteInstance[] ScaleNotes { get; }
        public int[] BlueNotes { get; }
        public int[] AscendingMelodicNotes { get; }
        public int[] DescendingResolutionNotes { get; }
        public AltModeNote[] AltModeNotes { get; }

        public string DebugString { get; }


        public MusicalContext(
            in Temperament temperament,
            in Mode mode_,
            in NoteInstance key_)
        {
            Temperament = temperament;
            Mode = mode_;
            Key = key_;

            NoteDictionary = new Dictionary<int, ContextualNoteEntry>();

            KeyRelativeNoteSemitones = new int[Mode.Spacings.Length];
            KeyRelativeAdjacentMajorNoteSemitones = new int[Mode.Spacings.Length];
            ScaleNotes = new NoteInstance[Mode.Spacings.Length];
            //AltNotes = (Mode.Group == 0) ? Context.Const.HeptatonicModeAlts[Mode.Index] : Context.Const.AltsNone;

            BuildScale();
            PopulateDictionaryWithScale();

            BlueNotes = ModeUtil.CalculateBlueNotes(KeyRelativeNoteSemitones, KeyRelativeAdjacentMajorNoteSemitones);
            AscendingMelodicNotes =
                ModeUtil.CalculateAscendingMelodicNotes(KeyRelativeNoteSemitones,
                    KeyRelativeAdjacentMajorNoteSemitones);
            DescendingResolutionNotes =
                ModeUtil.CalculateDescendingResolutionNotes(KeyRelativeNoteSemitones,
                    KeyRelativeAdjacentMajorNoteSemitones);
            AltModeNotes =
                ModeUtil.CalculateAltModeNotes(KeyRelativeNoteSemitones, KeyRelativeAdjacentMajorNoteSemitones);

            PopulateDictionaryWithAlternateNotes();

            PopulateDictionaryWithAltScaleNotes();

            DebugString = BuildDebugString();
        }

        public static MusicalContext Create<T>(in string tempermentString_, /* Letter Offset +2 (for C),*/ in T modeEnum_, in string keyString_)
            where T : struct, IConvertible
        {
            Temperament temperament = new Temperament(tempermentString_);
            Mode mode = Mode.Create(temperament, modeEnum_);
            NoteInstance key = TemperamentUtil.GetNoteForString(temperament, keyString_);
            if (key == null)
            {
                throw new Exception($"Unable to find key within Temperment {keyString_}");
            }

            return new MusicalContext(temperament, mode, key);
        }

        public void Dispose()
        {
        }

        private void BuildScale()
        {
            Mode associatedeMajorMode = Mode.Create(Temperament, ModesHeptatonic.Ionian);

            NoteInstance currentNote = new NoteInstance(Key.AbsoluteSemitoneInTemperment, 0, Key.Accidental);
            int relativeNote = 0;
            int majorRelativeNote = 0;
            int preferredAccidental = 0;
            for (int i = 0; i < Mode.Spacings.Length; ++i)
            {
                if (currentNote.Accidental < 0 && preferredAccidental == 0)
                {
                    preferredAccidental = -2;
                }

                if (currentNote.Accidental > 0 && preferredAccidental == 0)
                {
                    preferredAccidental = 2;
                }

                int offset = Mode.Spacings[i];

                KeyRelativeNoteSemitones[i] = relativeNote;
                KeyRelativeAdjacentMajorNoteSemitones[i] = majorRelativeNote;
                ScaleNotes[i] = currentNote;

                currentNote =
                    TemperamentUtil.GetAdjacentNoteRestricted(Temperament, Key, currentNote, offset, preferredAccidental);
                relativeNote += offset;
                majorRelativeNote += associatedeMajorMode.Spacings[i];
            }
        }

        private void PopulateDictionaryWithScale()
        {
            for (int i = 0; i < ScaleNotes.Length; ++i)
            {
                int accidentalRelativeToMajor = KeyRelativeNoteSemitones[i] - KeyRelativeAdjacentMajorNoteSemitones[i];
                NoteDictionary[KeyRelativeNoteSemitones[i]] = new ContextualNoteEntry(ScaleNotes[i], false, false,
                    false, false, accidentalRelativeToMajor);
            }
        }

        private void PopulateDictionaryWithAlternateNotes()
        {
            for (int i = 0; i < BlueNotes.Length; ++i)
            {
                int semitone = BlueNotes[i];
                NoteInstance previous = NoteDictionary[semitone - 1].Note;
                NoteInstance current = TemperamentUtil.GetAdjacentNote(Temperament, Key, previous, 1, -1, 1);
                int accidentalRelativeToMajor = KeyRelativeNoteSemitones[previous.ScaleIndex + 1] -
                                                KeyRelativeAdjacentMajorNoteSemitones[previous.ScaleIndex + 1];
                NoteDictionary[semitone] =
                    new ContextualNoteEntry(current, true, false, false, false, accidentalRelativeToMajor);
            }

            for (int i = 0; i < AscendingMelodicNotes.Length; ++i)
            {
                int semitone = AscendingMelodicNotes[i];
                NoteInstance previous = NoteDictionary[semitone - 1].Note;
                NoteInstance current = new NoteInstance(previous.AbsoluteSemitoneInTemperment + 1, previous.ScaleIndex,
                    previous.Accidental + 1);
                NoteDictionary[semitone] = new ContextualNoteEntry(current, false, true, false, false, 0);
            }

            for (int i = 0; i < DescendingResolutionNotes.Length; ++i)
            {
                int semitone = DescendingResolutionNotes[i];
                NoteInstance previous = NoteDictionary[semitone + 1].Note;
                NoteInstance current = new NoteInstance(previous.AbsoluteSemitoneInTemperment - 1, previous.ScaleIndex,
                    previous.Accidental - 1);
                NoteDictionary[semitone] = new ContextualNoteEntry(current, false, false, true, false, 0);
            }
        }

        private void PopulateDictionaryWithAltScaleNotes()
        {
            for (int j = 0; j < AltModeNotes.Length; ++j)
            {
                AltModeNote alt = AltModeNotes[j];
                int targetRelativeSemitone = KeyRelativeNoteSemitones[alt.ScaleIndex] + alt.GetShift();
                //Debug.Log($"altScale from note {alt.ScaleIndex} (semitone: {KeyRelativeNoteSemitones[alt.ScaleIndex]}) with shift {alt.GetShift()} - targeting semitone {targetRelativeSemitone}");

                if (NoteDictionary.ContainsKey(targetRelativeSemitone))
                {
                    ContextualNoteEntry oldCNE = NoteDictionary[targetRelativeSemitone];
                    NoteDictionary[targetRelativeSemitone] = new ContextualNoteEntry(
                        oldCNE.Note, oldCNE.IsBlue, oldCNE.IsAscendingMelodic, oldCNE.IsDescendingResolution, true,
                        oldCNE.AccidentalRelativeToMajor);
                }
                else
                {
                    NoteInstance baseNote = ScaleNotes[alt.ScaleIndex];
                    int baseSemitone = baseNote.AbsoluteSemitoneInTemperment;
                    int targetAbsoluteSemitone = baseSemitone + alt.GetShift();
                    int targetAccidental = baseNote.Accidental + alt.GetShift();
                    NoteInstance altNote = new NoteInstance(targetAbsoluteSemitone, alt.ScaleIndex, targetAccidental);

                    int accidentalRelativeToMajor = KeyRelativeNoteSemitones[alt.ScaleIndex] -
                                                    KeyRelativeAdjacentMajorNoteSemitones[alt.ScaleIndex];
                    NoteDictionary[targetRelativeSemitone] = new ContextualNoteEntry(
                        altNote, false, false, false, true, accidentalRelativeToMajor);
                }
            }
        }

        private string BuildDebugString()
        {
            string result = $"{Mode.Name} in the Key of {TemperamentUtil.GetStringForNote(Temperament, Key)}\n";
            for (int i = 0; i < Temperament.Notes.Length; ++i)
            {
                if (NoteDictionary.ContainsKey(i))
                {
                    string indexString = i.ToString("D2");
                    ContextualNoteEntry CNE = NoteDictionary[i];
                    if (CNE.IsBlue)
                    {
                        result +=
                            $"{indexString}: [<color=#{0:0}{1:0}{2:255}>BLUE</color>] {TemperamentUtil.GetStringForNote(Temperament, CNE.Note)}\n";
                    }
                    else if (CNE.IsAscendingMelodic)
                    {
                        result +=
                            $"{indexString}: [<color=#{0:255}{1:64}{2:0}>MEL</color>] {TemperamentUtil.GetStringForNote(Temperament, CNE.Note)}\n";
                    }
                    else if (CNE.IsDescendingResolution)
                    {
                        result +=
                            $"{indexString}: [<color=#{0:0}{1:128}{2:255}>MEL</color>] {TemperamentUtil.GetStringForNote(Temperament, CNE.Note)}\n";
                    }
                    else if (CNE.FromAltScale)
                    {
                        result +=
                            $"{indexString}: [<color=#{0:128}{1:128}{2:128}>ALT</color>] {TemperamentUtil.GetStringForNote(Temperament, CNE.Note)}\n";
                    }
                    else
                    {
                        result +=
                            $"{indexString}: {Const.GetAccidentalName(CNE.AccidentalRelativeToMajor)}{Temperament.INTERVAL_NUMERALS[CNE.Note.ScaleIndex]} {TemperamentUtil.GetStringForNote(Temperament, CNE.Note)}\n";
                    }
                }
            }

            return result;
        }

        public int GetTempermentWidth()
        {
            return Temperament.Notes.Length;
        }
        public Tuple<int,int> GetToneAndOctaveFromUnboundScaleDegree(int degree)
        {
            int tone = (degree + (ScaleNotes.Length * 100)) % ScaleNotes.Length;
            int octave = Mathf.FloorToInt(degree * 1.0f / ScaleNotes.Length);
            return new Tuple<int,int>(tone, octave);
        }
        public int GetAbsoluteSemitoneForUnboundScaleDegree(int degree)
        {
            Tuple<int,int> toneAndOctave = GetToneAndOctaveFromUnboundScaleDegree(degree);
            int semitone = ScaleNotes[toneAndOctave.Item1].AbsoluteSemitoneInTemperment;
            int tempermentWidth = GetTempermentWidth();
            return semitone + toneAndOctave.Item2 * tempermentWidth;

        }
    }
}