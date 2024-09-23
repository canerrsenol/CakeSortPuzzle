using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject losePanel;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.LevelVictory:
                victoryPanel.SetActive(true);
                break;
            case GameState.LevelDefeated:
                losePanel.SetActive(true);
                break;
            case GameState.LevelLoaded:
                victoryPanel.SetActive(false);
                losePanel.SetActive(false);
                break;
        }
    }
}
