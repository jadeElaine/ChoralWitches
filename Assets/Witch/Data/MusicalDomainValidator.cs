using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MusicalDomainContentValidator
{
    public static void Validate(ContentMusicalDomain domain)
    {
        if (domain.temperaments.Length <= 0)
        {
            throw new Exception($"No musical temperaments defined");
        }

        for (int i = 0; i < domain.temperaments.Length; ++i)
        {
            MusicalTemperament temperament = domain.temperaments[i];

            ValidateTemperament(temperament, i);
        }

        if (domain.modes.Length <= 0)
        {
            throw new Exception($"No musical modes defined");
        }

        for (int i = 0; i < domain.modes.Length; ++i)
        {
            MusicalMode mode = domain.modes[i];

            for (int j = i + 1; j < domain.modes.Length; j++)
            {
                if (domain.modes[j].name == mode.name)
                {
                    throw new Exception($"Musical Mode {j} has duplicate name \"{mode.name}\" shared with {i}");
                }

                if (domain.modes[j].uiPlacement.x == mode.uiPlacement.x &&
                    domain.modes[j].uiPlacement.y == mode.uiPlacement.y)
                {
                    throw new Exception($"Musical Mode {j} \"{domain.modes[j].name}\" has duplicate ui placment ({mode.uiPlacement.x}, {mode.uiPlacement.y}) shared with {i} \"{mode.name}\"");
                }
            }

            if (mode.tempermentIndex < 0 || mode.tempermentIndex >= domain.temperaments.Length)
            {
                throw new Exception($"Musical Mode {i} \"{mode.name}\" references invalid temperament index {mode.tempermentIndex}");
            }

            ValidateContext(mode, domain.temperaments[mode.tempermentIndex], i);
        }
    }

    public static void ValidateTemperament(MusicalTemperament temperament, int hostIndex)
    {
        if (temperament.semitoneCount <= 0)
        {
            throw new Exception($"Temperament {hostIndex} has no defined semitones");
        }

        if (temperament.noteLetters > temperament.semitoneCount)
        {
            throw new Exception($"Temperament {hostIndex} has more notes than semitones");
        }
            
        if (temperament.rootLetterSpacing.Length != temperament.semitoneCount)
        {
            throw new Exception($"Temperament {hostIndex} defines {temperament.semitoneCount} semitones but the root letter spacing specified {temperament.rootLetterSpacing.Length}");
        }

        if (temperament.rootLetterSpacing[0] == '-' ||
            temperament.rootLetterSpacing[0] == '+')
        {
            throw new Exception($"Temperament {hostIndex} root letter spacing does not start on a letter");
        }

        for (int i = 0; i < temperament.noteLetters; ++i)
        {
            char desired = (char)('A' + i);
            int indexInSpacing = temperament.rootLetterSpacing.IndexOf(desired);
            if (indexInSpacing == -1)
            {
                throw new Exception($"Temperament {hostIndex} does not specify a spacing location for expected letter {desired}");
            }
        }
    }

    public static void ValidateContext(MusicalMode mode, MusicalTemperament temperament, int hostIndex)
    {
        //context.tempermentSpacing
        //context.chordGroups
        if (mode.tempermentSpacing.Length != temperament.semitoneCount)
        {
            throw new Exception($"Musical Mode {hostIndex} \"{mode.name}\" temperament spacing specified {mode.tempermentSpacing.Length} semitones but the temperament specifies {temperament.semitoneCount} semitones");
        }

        for (int i = 0; i < mode.tempermentSpacing.Length; ++i)
        {
            char current = mode.tempermentSpacing[i];
            if (current != '-' &&
                current != '+')
            {
                if (int.TryParse(current.ToString(), out int interval))
                {
                    if (interval < 1 || interval > temperament.noteLetters)
                    {
                        throw new Exception($"Musical Mode {hostIndex} \"{mode.name}\" temperament spacing specified interval {interval} which does not exist in this temperament");
                    }
                }
                else
                {
                    throw new Exception($"Musical Mode {hostIndex} \"{mode.name}\" temperament spacing specified unexpected character {current}");
                }
                
                for (int j = i + 1; j < mode.tempermentSpacing.Length; ++j)
                {
                    char match = mode.tempermentSpacing[j];
                    if (current == match)
                    {
                        throw new Exception($"Musical Mode {hostIndex} \"{mode.name}\" temperament spacing specified duplicate character {current}");
                    }
                }
            }
        }
    }
}
