using UnityEngine;

public static class GameSettings
{
    public static int segments = 50;
    public static int depth = 3;
    public static float turretChance = 0.25f;
    public static float minHeight = 2f;
    public static float maxHeight = 8f;
    public static int maxLives = 3;
    public static float spikeChance = 0.1f;

    // stats
    public static int easyCompletions = 0;
    public static int mediumCompletions = 0;
    public static int hardCompletions = 0;

    public static void SetDifficultyEasy()
    {
        turretChance = 0.05f;
        segments = 80;
        depth = 3;
        maxHeight = 6f;
        minHeight = 1f;
        spikeChance = 0.05f;
        maxLives = 3;
    }

    public static void SetDifficultyMedium()
    {
        turretChance = 0.1f;
        segments = 120;
        depth = 4;
        maxHeight = 10f;
        minHeight = 2f;
        spikeChance = 0.1f;
        maxLives = 4;
    }

    public static void SetDifficultyHard()
    {
        turretChance = 0.2f;
        segments = 160;
        depth = 5;
        maxHeight = 18f;
        minHeight = 2f;
        spikeChance = 0.15f;
        maxLives = 5;
    }
}