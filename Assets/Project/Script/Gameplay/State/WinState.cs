public class WinState : IGameState
{
    public void Enter()
    {
        GameEvents.RaiseWin();
    }

    public void Exit()
    {
    }
    public bool CanHandleInput()
    {
        return false;
    }
}
