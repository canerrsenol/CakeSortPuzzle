using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private Transform[] conveyorPoints;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Plate platePrefab;
    private int spawnCycle = 3;
    private int currentSpawnIndex = 0;

    private LevelDetailSO levelData;

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
        if (gameState == GameState.Loaded)
        {
            levelData = LevelManager.Instance.CurrentLevelDetail;
            SpawnPlates();
        }
    }

    private void SpawnPlates()
    {
        for (int i = 0; i < spawnCycle; i++)
        {
            if(currentSpawnIndex >= levelData.LevelPlates.Count) return;
            Plate plate = Instantiate(platePrefab, spawnPoint.position, Quaternion.identity);
            plate.InitializePlate(levelData.LevelPlates[currentSpawnIndex], conveyorPoints[i].position);
            currentSpawnIndex++;
        }
    }
}
