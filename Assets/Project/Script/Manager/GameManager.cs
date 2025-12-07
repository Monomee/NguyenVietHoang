using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;
    private void OnEnable()
    {        
        GameEvents.OnPour += OnPour;
        GameEvents.OnTubeCompleted += OnTubeCompleted;
    }
    void OnDisable()
    {
        Instance = null;
        GameEvents.OnPour -= OnPour;
        GameEvents.OnTubeCompleted -= OnTubeCompleted;
    }

    [Header("DB Color")]
    [SerializeField] ColorSpriteDB colorDB;
    [SerializeField] Difficulty difficulty = Difficulty.Normal;

    [Header("Refrences")]
    [SerializeField] WinPopup win;
    [SerializeField] LosePopup lose;

    IGameState currentState;
    public LevelData levelData;   
    Stack<ICommand> history = new Stack<ICommand>(); // for undo
    TubeView selectedSource;


    private void Awake()
    {
        Instance = this;
        ColorSpriteDB.Init(colorDB);
        levelData = LevelGenerator.GenerateLevel(difficulty);
        ChangeState(new PlayState());
    }

    //=========Event Handlers=========
    void OnPour(TubeData from, TubeData to, int amount)
    {
        CheckGameEnd();
    }

    void OnTubeCompleted(TubeData tube)
    {
        AudioManager.Instance.PlayCompletedSound();
    }

    //=========Gamplay Methods=========
    public void OnTubeClicked(TubeView tube)
    {
        if (!currentState.CanHandleInput())
            return;

        if (selectedSource == null)
        {
            SelectSource(tube);
            return;
        }

        // Click itself
        if (selectedSource == tube)
        {
            ResetSelection();
            return;
        }

        if (GameLogic.CanPour(selectedSource.Data, tube.Data))
        {
            TryPour(tube);
            return;
        }

        ResetSelection();
        SelectSource(tube);
        CheckGameEnd();
    }
    void SelectSource(TubeView tube)
    {
        if (tube.Data.Colors.Count == 0) return;
        if (GameLogic.IsCompleted(tube.Data))
            return;

        selectedSource = tube;
        tube.SetHighlight(true);
    }
    void TryPour(TubeView target)
    {
        var from = selectedSource.Data;
        var to = target.Data;

        if (!GameLogic.CanPour(from, to))
        {
            ResetSelection();
            return;
        }

        ICommand cmd = new PourCommand(from, to);
        cmd.Execute();
        history.Push(cmd);
        AudioManager.Instance.PlayPourSound();

        ResetSelection();

        CheckGameEnd();
    }
    void ResetSelection()
    {
        if (selectedSource != null)
            selectedSource.SetHighlight(false);

        selectedSource = null;
    }
    void CheckGameEnd()
    {
        if (GameLogic.IsWin(levelData))
        {
            ChangeState(new WinState());
            return;
        }

        if (!GameLogic.HasAnyValidMove(levelData))
        {
            ChangeState(new LoseState());
        }
    }
    public void Undo()
    {
        if (history.Count == 0) return;

        var cmd = history.Pop();
        cmd.Undo();

        ChangeState(new PlayState());
    }
    void ChangeState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    //=========Other Methods=========
    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
   
}
