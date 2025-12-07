using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelGenerator
{
    public static LevelData GenerateLevel(Difficulty difficulty)
    {
        DifficultyConfig cfg = DifficultyTable.Get(difficulty);

        for (int attempt = 0; attempt < cfg.maxShuffleAttempts; attempt++)
        {
            LevelData level = GenerateLevel(
                cfg.numberOfTubes,
                cfg.emptyTubes,
                cfg.depth,
                cfg.colorCount
            );

            // RULE 5: prevent deadlock
            if (!GameLogic.HasAnyValidMove(level))
                continue;

            // RULE 3: prevent completed tubes at start
            int completed = level.Tubes.Count(t => GameLogic.IsCompleted(t));
            if (completed > cfg.maxCompletedAtStart)
                continue;

            int validMoves = CountValidMoves(level);
            if (validMoves < cfg.minValidMoves)
                continue;

            return level;
        }

        // fallback 
        return GenerateLevel(
            cfg.numberOfTubes,
            cfg.emptyTubes,
            cfg.depth,
            cfg.colorCount
        );
    }

    static LevelData GenerateLevel(int numberOfTubes, int emptyTubes, int depth, int colorCount)
    {
        LevelData level = new LevelData(numberOfTubes);
        for (int i = 0; i < numberOfTubes; i++)
            level.Tubes.Add(new TubeData(depth));

        int fillableCount = numberOfTubes - emptyTubes;
        if (fillableCount <= 0) return level;

        List<TubeData> fillable = level.Tubes.Take(fillableCount).ToList();
        List<ColorType> colors = GenerateColors(colorCount, depth);

        foreach (var color in colors)
        {
            var candidates = fillable.Where(t =>
                t.Colors.Count < t.Depth &&
                !(t.Colors.Count >= 2 && t.Colors.Peek() == color)
            ).ToList();

            if (candidates.Count == 0)
            {
                candidates = fillable.Where(t => t.Colors.Count < t.Depth).ToList();
            }

            if (candidates.Count == 0)
            {
                return GenerateLevel(numberOfTubes, emptyTubes, depth, colorCount);
            }

            TubeData tube = candidates[Random.Range(0, candidates.Count)];
            tube.Colors.Push(color);
        }

        return level;
    }

    // ===== HELPERS =====
    static List<ColorType> GenerateColors(int colorCount, int depth)
    {
        List<ColorType> colors = new();
        for (int i = 0; i < colorCount; i++)
            for (int j = 0; j < depth; j++)
                colors.Add((ColorType)i);

        Shuffle(colors);
        return colors;
    }

    static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    static int CountValidMoves(LevelData level)
    {
        int count = 0;
        for (int i = 0; i < level.Tubes.Count; i++)
        {
            for (int j = 0; j < level.Tubes.Count; j++)
            {
                if (i == j) continue;
                if (GameLogic.CanPour(level.Tubes[i], level.Tubes[j]))
                    count++;
            }
        }
        return count;
    }
}
