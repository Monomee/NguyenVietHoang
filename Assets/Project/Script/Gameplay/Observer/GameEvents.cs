using System;

public static class GameEvents
{
    // gameplay
    public static event Action<TubeData, TubeData, int> OnPour;
    public static event Action<TubeData> OnTubeCompleted;
    public static event Action OnWin;
    public static event Action OnLose;
    public static event Action<TubeData, TubeData> OnUndo;
    public static void RaisePour(TubeData from, TubeData to, int amount)
        => OnPour?.Invoke(from, to, amount);

    public static void RaiseTubeCompleted(TubeData tube)
        => OnTubeCompleted?.Invoke(tube);

    public static void RaiseWin()
        => OnWin?.Invoke();

    public static void RaiseLose()
        => OnLose?.Invoke();

    public static void RaiseUndo(TubeData from, TubeData to)
    {
        OnUndo?.Invoke(from, to);
    }
}
