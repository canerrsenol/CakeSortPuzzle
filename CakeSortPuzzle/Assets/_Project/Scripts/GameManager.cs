using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameState GameState;
    public delegate void GameStateChangedEvent(GameState state);
    public event GameStateChangedEvent OnGameStateChanged;

    public void CompleteLevel()
    {
        ChangeGameState(GameState.Victory);
    }

    public void ChangeGameState(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.Playing:
                break;
            case GameState.Victory:
                break;
            case GameState.Lose:
                break;
        }
        if (GameState != newGameState)
            OnGameStateChanged?.Invoke(newGameState);
        GameState = newGameState;
    }
}

public enum GameState
{
    Started,
    Playing,
    Victory,
    Lose
}