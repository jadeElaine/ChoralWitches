using System;
using System.Collections.Generic;

namespace Music.Support
{
    // X-XX-X-XX-X-
    // X--X-X--X--X-X--X--
    //
    // A ! B C ! D ! E F ! G !
    // A ! ! B ! C ! ! D ! ! E ! F ! ! G ! !
    public class Temperament
    {
        public static readonly string[] INTERVAL_NUMERALS = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
        public class NoteDefinition
        {
            public string FormalName { get; }
            public int FormalIndex { get; }
            public bool AlwaysAccidental { get; }
        
            public NoteDefinition(in string formalName_, in int formalIndex_, in bool alwaysAccidental_)
            {
                FormalName = formalName_;
                FormalIndex = formalIndex_;
                AlwaysAccidental = alwaysAccidental_;
            }
        }

        public int[] AnchorNotes { get; }
        public NoteDefinition[] Notes { get; }

        public Temperament(in string noteString_)
        {
            List<int> anchorNoteList = new List<int>();
            Notes = new NoteDefinition[noteString_.Length];
            int formalIndex = 0;
            for (int i = 0; i < noteString_.Length; ++i)
            {
                if (noteString_[i] == '-')
                {
                    Notes[i] = new NoteDefinition("",-1,true);
                }
                else
                {
                    anchorNoteList.Add(i);
                    //Debug.Log($"Loading {splitNoteStrings[i]} at position {i} for formalIndex {formalIndex}");
                    Notes[i] = new NoteDefinition("" +(char)('A' + formalIndex), formalIndex, false);
                    formalIndex++;
                }
            }

            AnchorNotes = anchorNoteList.ToArray();
        }

        public int[] GetHeptatonicIntervals()
        {
            int[] result = new int[AnchorNotes.Length];
            const int ionicOffset = -2; // everything is based on the major scale in C, even though the notes start at A for music theory reasons
            for (int i = 0; i < AnchorNotes.Length; ++i)
            {
                int targetIndex = (i + ionicOffset + AnchorNotes.Length) % AnchorNotes.Length;
                int nextAnchor = (i + 1) % AnchorNotes.Length;
                result[targetIndex] = (AnchorNotes[nextAnchor] - AnchorNotes[i] + Notes.Length) % Notes.Length;
            }
            
            return result;
        }
        public int[] GetPentatonicIntervals()
        {
            int[] hept = GetHeptatonicIntervals();
            return new [] {hept[0], hept[1], hept[2] + hept[3], hept[4], hept[5] + hept[6]};
        }
        public int[] GetQuadrastepIntervals()
        {
            int[] hept = GetHeptatonicIntervals();
            return new [] {hept[0]+hept[1], hept[2], hept[3]+hept[4], hept[5], hept[6]};
        }
    }
}
