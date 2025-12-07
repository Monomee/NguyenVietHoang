public class LoseState : IGameState
{
    public void Enter()
    {
        GameEvents.RaiseLose();
    }

    public void Exit()
    {

    }
    public bool CanHandleInput()
    {
        return false;
    }
}
