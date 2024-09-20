using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    private int levelNumber;
    private LevelDetailSO currentLevelDetail;
    public LevelDetailSO CurrentLevelDetail => currentLevelDetail;
    [SerializeField] private LevelDetailSO[] levelList;
    private GameObject levelContent;
    public GameObject LevelContent => levelContent;

    private void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        LoadLevelData();
        CreateContent();

        GameManager.Instance.ChangeGameState(GameState.Loaded);
    }

    public void LoadCurrentLevel()
    {
        DestroyOldContent();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        UpgradeLevelNumber();
        DestroyOldContent();
        CreateContent();
    }

    private void DestroyOldContent()
    {
        if (levelContent != null)
            Destroy(levelContent);
    }

    public void UpgradeLevelNumber()
    {
        levelNumber++;
        SaveLevelData();
    }

    private void LoadLevelData()
    {
        levelNumber = PlayerPrefs.GetInt("LevelNumber", 1);
    }

    private void SaveLevelData()
    {
        PlayerPrefs.SetInt("LevelNumber", levelNumber);
    }

    private void CreateContent()
    {
        currentLevelDetail = levelList[levelNumber%levelList.Length - 1];
        levelContent = Instantiate(currentLevelDetail.LevelPrefab, transform);
    }
}