using UnityEngine;
using UnityEngine.UI;

public class RetryButtonAction : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnRetryButtonClicked);
    }
    
    public void OnRetryButtonClicked()
    {
        LevelManager.Instance.LoadCurrentLevel();
    }
}
