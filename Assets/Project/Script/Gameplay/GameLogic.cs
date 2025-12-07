public static class GameLogic
{
    public static bool CanPour(TubeData from, TubeData to)
    {
        if (from.Colors.Count == 0 ||
            to.Colors.Count == to.Depth) return false;
        if (to.Colors.Count == 0) return true;
        return from.Colors.Peek().Equals(to.Colors.Peek());
    }
    public static void Pour(TubeData from, TubeData to, out int poured)
    {
        poured = 0;
        if (!CanPour(from, to) ||
            IsCompleted(from) ||
            IsCompleted(to)) return;
        ColorType colorFrom = from.Colors.Peek();
        while (from.Colors.Count > 0 &&
               from.Colors.Peek() == colorFrom &&
               to.Colors.Count < to.Depth)
        {
            from.Colors.Pop();
            to.Colors.Push(colorFrom);
            poured++;
        }
    }
    public static bool IsCompleted(TubeData tube)
    {
        if (tube.Colors.Count < tube.Depth) return false;

        ColorType? topColor = tube.Colors.Peek();
        foreach (var color in tube.Colors)
        {
            if (color != topColor) return false;
        }

        return true;
    }
    public static bool IsWin(LevelData level)
    {
        foreach (var tube in level.Tubes)
        {
            if (tube.Colors.Count == 0) continue;
            if (!IsCompleted(tube)) return false;
        }
        return true;
    }
    public static bool HasAnyValidMove(LevelData level)
    {
        for (int i = 0; i < level.Tubes.Count; i++)
        {
            if (level.Tubes[i].Colors.Count == 0 || IsCompleted(level.Tubes[i])) continue;
            for (int j = 0; j < level.Tubes.Count; j++)
            {
                if (i == j) continue;
                if (CanPour(level.Tubes[i], level.Tubes[j])) return true;
            }
        }
        return false;
    }
    public static void Revert(TubeData from, TubeData to, int poured)
    {
        for (int i = 0; i < poured; i++)
        {
            from.Colors.Push(to.Colors.Pop());
        }
    }
}
