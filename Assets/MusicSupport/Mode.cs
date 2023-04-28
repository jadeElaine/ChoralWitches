using System;
using System.Globalization;
using System.Linq;

namespace Music.Support
{
    public enum ModalGroup
    {
        Heptatonic=0,
        Pentatonic,
        Quadrastep,
    }

    public enum ModesHeptatonic
    {
        Ionian=0, // Major
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Aeolian, // minor
        Locrian
    }
    public enum ModesPentatonic
    {
        Major=0,    // Lydian, Ionian, Mixolydian
        MajorBlues, // Ionian, Mixolydian, dorian
        Suspended,  // Mixolydian, dorian, aeolian
        Minor,      // dorian, aeolian, phrygian
        MinorBlues  // aeolian, phrygian, locrian
    }
    public enum ModesQuadrastep
    {
        Nakazora=0, // Ionian compatible
                    // (No Dorian compatible)
        In,         // Phrygian compatible
        Chinese,    // Lydian compatible
                    // (No Mixolydian compatible)
        Hirajoshi,  // Aoelian compatible
        Iwato,      // Locrian compatible
    }

    public class Mode
    {
        public int Group { get; }
        public int Index { get; }
        public string Name { get; }
        public int[] Spacings { get; }
        public int[] Intervals { get; }

        public Mode(in int group_, in int index_, in string name_, in int[] spacings)
        {
            Group = group_;
            Index = index_;
            Name = name_;
            Spacings = spacings;
            Intervals = new int[spacings.Length];
            int interval = 0;
            for (int i = 0; i < spacings.Length; ++i)
            {
                Intervals[i] = interval;
                interval += Spacings[i];
            }
        }

        public static Mode Create<T>(Temperament temperament, T value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("Must send an enum type to ResolveModeGroupAndIndex");
            }

            int group = -1;
            int[] intervals = null;
            if (typeof(T) == typeof(ModesHeptatonic)) { group = 0; intervals = temperament.GetHeptatonicIntervals(); }
            if (typeof(T) == typeof(ModesPentatonic)) { group = 1; intervals = temperament.GetPentatonicIntervals(); }
            if (typeof(T) == typeof(ModesQuadrastep)) { group = 2; intervals = temperament.GetQuadrastepIntervals(); }
            if (group == -1)
            {
                throw new Exception($"Unrecognized Mode Group {typeof(T).FullName}");
            }

            int modeIndex = value.ToInt32(CultureInfo.InvariantCulture);
            string name = Enum.GetNames(typeof(T))[modeIndex];
            
            return new Mode(group, modeIndex, name, ResolveModeIntervals(modeIndex, intervals));
        }

        public static Mode CreateFromString(Temperament temperament, string name)
        {
            string[] heptatonics = Enum.GetNames(typeof(ModesHeptatonic));
            string[] pentatonics = Enum.GetNames(typeof(ModesPentatonic));
            string[] quadrasteps = Enum.GetNames(typeof(ModesQuadrastep));
            if (heptatonics.Contains(name))
            {
                ModesHeptatonic id = (ModesHeptatonic)Enum.Parse(typeof(ModesHeptatonic), name);
                return Create(temperament,id);
            }
            if(pentatonics.Contains(name))
            {
                ModesPentatonic id = (ModesPentatonic)Enum.Parse(typeof(ModesPentatonic), name);
                return Create(temperament,id);
            }
            if (quadrasteps.Contains(name))
            {
                ModesQuadrastep id = (ModesQuadrastep)Enum.Parse(typeof(ModesQuadrastep), name);
                return Create(temperament,id);
            }
            throw new Exception($"Unrecognized Mode {name}");
        }
        
        private static int[] ResolveModeIntervals(in int modeIndex_, in int[] baseIntervals_)
        {
            int size = baseIntervals_.Length;
            int[] result = new int[size];
            for (int i = 0; i < size; ++i) { result[i] = baseIntervals_[(i + modeIndex_) % size]; }
            return result;
        }   
    }

    public class AltModeNote
    {
        public enum ShiftType { Flat=0, Natural }

        public readonly int ScaleIndex;
        public readonly ShiftType Shift;

        public AltModeNote(in int scaleIndex_, in ShiftType shift_)
        {
            ScaleIndex = scaleIndex_;
            Shift = shift_;
        }

        public int GetShift()
        {
            switch (Shift)
            {
                case ShiftType.Flat: return -1;
                case ShiftType.Natural: return 1;
            }
            return 0;
        }
    }
}
