using System;

namespace Music.Support
{
    public static class TemperamentUtil 
    {
        
        public static NoteInstance GetNoteForString(in Temperament temperament, in string noteString)
        {
            for (int i = -4; i < 4; ++i)
            {
                int AccidentalLength = Const.GetAccidentalName(i).Length;
                if (noteString.EndsWith(Const.GetAccidentalName(i)) && noteString.Length > AccidentalLength)
                {
                    string isolatedNote = noteString.Substring(0, noteString.Length - AccidentalLength);
                    for (int j = 0; j < temperament.Notes.Length; ++j)
                    {
                        if (isolatedNote == temperament.Notes[j].FormalName)
                        {
                            //Debug.Log($"Captured \"{isolatedNote}\" with accidental offset ({i-4}) which will target formalIndex ({Notes[j].FormalIndex}), and absolute offset ({j})");
                            return new NoteInstance(j + i, temperament.Notes[j].FormalIndex, i);
                        }
                    }
                }
            }

            return null;
        }

        public static string GetStringForNote(in Temperament temperament, in NoteInstance note)
        {
            if(note.Accidental < -4 || note.Accidental > 4) {throw new Exception($"Invalid Accidental Offset {note.Accidental}");}

            int rootSemitone = (note.AbsoluteSemitoneInTemperment - note.Accidental + temperament.Notes.Length) % temperament.Notes.Length;
            return $"{temperament.Notes[rootSemitone].FormalName}{Const.GetAccidentalName(note.Accidental)}";
        }

        public static NoteInstance GetAdjacentNote(in Temperament temperament, in NoteInstance key, in NoteInstance root, int semitones, int minAccidental, int maxAccidental)
        {
            if (semitones == 0) { return root; }

            int targetAnchorNote = (root.ScaleIndex + (semitones > 0 ? 1 : -1));
            int safeAnchorNote = (targetAnchorNote + temperament.AnchorNotes.Length) % temperament.AnchorNotes.Length;
            //Debug.Log($"Starting from root {GetStringForNote(root)} ({root.AbsoluteNoteInTemperment},{root.TempermentAnchor},{root.Accidental}) and stepping up {semitones} to anchor {targetAnchorNote} safetying to {safeAnchorNote}");

            int baseSemitoneDelta = (temperament.AnchorNotes[(key.ScaleIndex+safeAnchorNote)%temperament.AnchorNotes.Length] -
                                     temperament.AnchorNotes[(key.ScaleIndex+root.ScaleIndex)%temperament.AnchorNotes.Length]);
            if (semitones < 0 && baseSemitoneDelta > 0) baseSemitoneDelta -= temperament.Notes.Length;
            if (semitones > 0 && baseSemitoneDelta < 0) baseSemitoneDelta += temperament.Notes.Length;
                
            int targetAccidental = root.Accidental + semitones - baseSemitoneDelta;
            while (targetAccidental > temperament.Notes.Length / 2) { targetAccidental -= temperament.Notes.Length; }
            while (targetAccidental < -temperament.Notes.Length / 2) { targetAccidental += temperament.Notes.Length; }

            //Debug.Log($"Mid Resoloving to safe tone {safeAnchorNote} with accidental {targetAccidental} ({minAccidental}|{maxAccidental})");

            if (targetAccidental < minAccidental || targetAccidental > maxAccidental)
            {
                int tonalOffset = baseSemitoneDelta - root.Accidental;
                int newAbsolute = root.AbsoluteSemitoneInTemperment + tonalOffset;
                return GetAdjacentNote(
                    temperament, key,
                    new NoteInstance(newAbsolute, safeAnchorNote, 0), 
                    semitones - tonalOffset,
                    minAccidental, maxAccidental);
            }

            return new NoteInstance(root.AbsoluteSemitoneInTemperment + semitones, safeAnchorNote, targetAccidental);
        }

        public static NoteInstance GetAdjacentNoteRestricted(in Temperament temperament, in NoteInstance key, in NoteInstance root, int semitones,
            in int restrictedAccidental)
        {
            if (restrictedAccidental < 0)
            {
                return GetAdjacentNote(temperament, key, root, semitones, restrictedAccidental, 0);
            }
            if (restrictedAccidental > 0)
            {
                return GetAdjacentNote(temperament, key, root, semitones, 0, restrictedAccidental);
            }
            return GetAdjacentNote(temperament, key, root, semitones, -1, 1);
        }
    }
}
