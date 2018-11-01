using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExperienceController {

    const int BaseXP = 100;
    static float Multiplier = Mathf.Pow(2, 1f / 7f);

    public const int ExperiencePerKm = 1;
    public const int ExperiencePerRacePlace = 5;

    public static int GetExperienceForLevel(int level)
    {
        if (level == 0)
            return BaseXP;
        else
            return (int)(GetExperienceForLevel(level - 1) * Multiplier) + (int)(level * Multiplier);
    }
}
