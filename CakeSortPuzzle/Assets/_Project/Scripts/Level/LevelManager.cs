using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoSingleton<LevelManager>
{
    public int currentContentIndex;

    public LevelDetailSO currentLevelDetail;
    public LevelDetailSO[] levelList;

    public int levelNumber;

    private void Start()
    {
        InstanceLevel();
    }

    private void InstanceLevel()
    {
        LoadLevelDatas();
        CreateContent();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextLevelContentAction();
            UpgradeLevelNumberIndex(1, true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
#endif
    }

    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        NextLevelContentAction();
        UpgradeLevelNumberIndex(1, true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevelContentAction()
    {
        UpgradeContentIndex();
        SaveLevelDatas();
    }

    private void LoadLevelDatas()
    {
        // load level number from PlayerPrefs
        levelNumber = PlayerPrefs.GetInt("LevelNumber", 1);
        // load level index from PlayerPrefs
        currentContentIndex = PlayerPrefs.GetInt("LevelIndex", 0);
    }

    private void SaveLevelDatas()
    {
        // save level number to PlayerPrefs
        PlayerPrefs.SetInt("LevelNumber", levelNumber);
        // save level index to PlayerPrefs
        PlayerPrefs.SetInt("LevelIndex", currentContentIndex);
    }

    private void CreateContent()
    {
        print(currentContentIndex);
        currentLevelDetail = levelList[currentContentIndex];
        Instantiate(currentLevelDetail.LevelPrefab, transform);
    }

    public void UpgradeLevelNumberIndex(int index, bool isSaveGame)
    {
        levelNumber += index;
        if (isSaveGame)
        {
            SaveLevelDatas();
        }
    }

    private void UpgradeContentIndex()
    {
        if (currentContentIndex < levelList.Length - 1) currentContentIndex++;
        else currentContentIndex = RandomIndex(currentContentIndex);
    }

    private int RandomIndex(int currentIndexNumber)
    {
        while (true)
        {
            var returnIndex = Random.Range(0, levelList.Length);
            if (returnIndex != currentIndexNumber) return returnIndex;
        }
    }
}