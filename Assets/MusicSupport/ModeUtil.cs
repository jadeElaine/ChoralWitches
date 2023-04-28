using System;
using System.Collections.Generic;
using Music.Support;
using UnityEngine;

public static class ModeUtil
{
    public static int[] CalculateBlueNotes(in int[] keyRelativeNoteSemitones, in int[] keyRelativeAdjacentMajorNoteSemitones)
    {
        List<int> blueNoteList = new List<int>();
        for (int i = 1; i <= Mathf.Min(5, keyRelativeAdjacentMajorNoteSemitones.Length-1); i += 2)
        {
            int prevMajorSemitone = keyRelativeAdjacentMajorNoteSemitones[i];
            int nextMajorSemitone = keyRelativeAdjacentMajorNoteSemitones[i+1];

            int prevInScale = Array.IndexOf(keyRelativeNoteSemitones, prevMajorSemitone);
            int nextInScale = Array.IndexOf(keyRelativeNoteSemitones, nextMajorSemitone);

            int potentialSemitoneScaleIndex = Array.IndexOf(keyRelativeNoteSemitones, prevMajorSemitone + 1);

            //Debug.Log($"Updating new blue note range w semitone {prevMajorSemitone} - {nextMajorSemitone}, which matches with scale degrees {prevInScale} - {nextInScale}.  Hitting index {potentialSemitoneScaleIndex}");

            if (prevInScale != -1 && nextInScale != -1 && potentialSemitoneScaleIndex == -1)
            {
                blueNoteList.Add(prevMajorSemitone+1);
            }
        }
        return blueNoteList.ToArray();
    }

    public static int[] CalculateAscendingMelodicNotes(in int[] keyRelativeNoteSemitones, in int[] keyRelativeAdjacentMajorNoteSemitones)
    {
        int second = keyRelativeNoteSemitones[1];
        int penultimate = keyRelativeNoteSemitones[^2];
        int ultimate = keyRelativeNoteSemitones[^1];

        int secondInMajor = keyRelativeAdjacentMajorNoteSemitones[1];
        int penultimateInMajor = keyRelativeAdjacentMajorNoteSemitones[^2];
        int ultimateInMajor = keyRelativeAdjacentMajorNoteSemitones[^1];

        if (second == secondInMajor && ultimate == ultimateInMajor-1)
        {
            if (penultimate == penultimateInMajor-1)
            {
                return new [] { penultimateInMajor, ultimateInMajor };
            }
            return new [] { ultimateInMajor };
        }

        return new int[0];
    }

    public static int[] CalculateDescendingResolutionNotes(in int[] keyRelativeNoteSemitones, in int[] keyRelativeAdjacentMajorNoteSemitones)
    {
        int second = keyRelativeNoteSemitones[1];

        int secondInMajor = keyRelativeAdjacentMajorNoteSemitones[1];

        int potentialSemitoneScaleIndex = Array.IndexOf(keyRelativeNoteSemitones, second - 1);

        if (second == secondInMajor && potentialSemitoneScaleIndex == -1)
        {
            return new [] { second-1 };
        }

        return new int[0];
    }
    public static AltModeNote[] CalculateAltModeNotes(in int[] keyRelativeNoteSemitones, in int[] keyRelativeAdjacentMajorNoteSemitones)
    {
        int third = keyRelativeNoteSemitones[2];
        int fourth = keyRelativeNoteSemitones[3];
        int fifth = keyRelativeNoteSemitones[4];
        int ultimate = keyRelativeNoteSemitones[^1];

        int thirdInMajor = keyRelativeAdjacentMajorNoteSemitones[2];
        int fourthInMajor = keyRelativeAdjacentMajorNoteSemitones[3];
        int fifthInMajor = keyRelativeAdjacentMajorNoteSemitones[4];
        int ultimateInMajor = keyRelativeAdjacentMajorNoteSemitones[^1];

        List<AltModeNote> result = new List<AltModeNote>();
        if ( third == thirdInMajor-1 && fifth == fifthInMajor && ultimate == ultimateInMajor-1)
        {
            result.Add(new AltModeNote(2, AltModeNote.ShiftType.Natural));
        }
        if (fourth == fourthInMajor - 1 && fifth == fifthInMajor)
        {
            result.Add(new AltModeNote(3, AltModeNote.ShiftType.Flat));
        }
        if ( fifth == fifthInMajor-1 && third == thirdInMajor-1)
        {
            result.Add(new AltModeNote(4, AltModeNote.ShiftType.Flat));
        }
        return result.ToArray();
    }
}
