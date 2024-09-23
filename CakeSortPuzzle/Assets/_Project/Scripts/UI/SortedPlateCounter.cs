using TMPro;
using UnityEngine;

public class SortedPlateCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sortedPlateCounterText;

    [SerializeField] private GlobalEventsSO globalEventsSO;

    private int sortedPlateCount = 0;

    private int targetSortedPlateCount = 0;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        globalEventsSO.OnPlateSorted += OnPlateSorted;
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null) GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        globalEventsSO.OnPlateSorted -= OnPlateSorted;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.LevelLoaded)
        {
            sortedPlateCount = 0;
            targetSortedPlateCount = LevelManager.Instance.CurrentLevelData.TargetSortPlateCount;
            UpdateSortedPlateCounterText();
        }
    }

    private void OnPlateSorted()
    {
        IncreaseSortedPlateCount();
        UpdateSortedPlateCounterText();
    }

    private void IncreaseSortedPlateCount()
    {
        sortedPlateCount++;
    }

    private void UpdateSortedPlateCounterText()
    {
        sortedPlateCounterText.text = $"Sorted plate count : {sortedPlateCount}/{targetSortedPlateCount}";
    }
}
