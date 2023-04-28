namespace Music.Support
{
    public class NoteInstance
    {
        public int AbsoluteSemitoneInTemperment { get; }
        public int ScaleIndex { get; }
        public int Accidental { get; }

        public NoteInstance(in int absoluteSemitoneInTemperment_, in int scaleIndex_, in int accidental_)
        {
            AbsoluteSemitoneInTemperment = absoluteSemitoneInTemperment_;
            ScaleIndex = scaleIndex_;
            Accidental = accidental_;
        }
    }
}