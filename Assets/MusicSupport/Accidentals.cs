namespace Music.Support
{
    public enum Accidentals
    {
        QuadFlat=-4,
        TripleFlat,
        DoubleFlat,
        Flat,
        Natural,
        Sharp,
        DoubleSharp,
        TripleSharp,
        QuadSharp
    }

    public static partial class Const
    {
        private static readonly string[] AccidentalNames = {"bbbb", "bbb", "bb", "b", "", "#", "##", "###", "####"};
        public static string GetAccidentalName(int shift)
        {
            return AccidentalNames[shift + 4];
        }
    }
}