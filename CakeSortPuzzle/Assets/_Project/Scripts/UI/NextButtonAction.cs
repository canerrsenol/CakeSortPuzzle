using UnityEngine;
using UnityEngine.UI;

public class NextButtonAction : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnNextButtonClicked);
    }

    public void OnNextButtonClicked()
    {
        LevelManager.Instance.LoadNextLevel();
    }
}
