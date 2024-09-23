public class GameManager : MonoSingleton<GameManager>
{
    private GameState GameState;
    public GameState CurrentGameState => GameState;
    public delegate void GameStateChangedEvent(GameState state);
    public event GameStateChangedEvent OnGameStateChanged;

    public void ChangeGameState(GameState newGameState)
    {
        if (GameState != newGameState)
            OnGameStateChanged?.Invoke(newGameState);
        GameState = newGameState;
    }
}

public enum GameState
{
    None,
    LevelLoaded,
    LevelStarted,
    LevelVictory,
    LevelDefeated
}