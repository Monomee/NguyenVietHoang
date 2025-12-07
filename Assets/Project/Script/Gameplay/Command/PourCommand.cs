public class PourCommand : ICommand
{
    TubeData from;
    TubeData to;
    int poured;
    public PourCommand(TubeData from, TubeData to)
    {
        this.from = from;
        this.to = to;
    }
    public void Execute()
    {
        GameLogic.Pour(from, to, out poured);

        GameEvents.RaisePour(from, to, poured);

        if (GameLogic.IsCompleted(to))
            GameEvents.RaiseTubeCompleted(to);
    }

    public void Undo()
    {
        GameLogic.Revert(from, to, poured);
        GameEvents.RaiseUndo(from, to);
    }
}
