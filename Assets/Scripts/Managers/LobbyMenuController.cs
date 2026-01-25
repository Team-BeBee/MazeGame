using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject endingInfoPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Scene Names")]
    [SerializeField] private string gameplaySceneName = "MazeScene";

    private void Start()
    {
        ShowMain();
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnClickOptions()
    {
        ShowOnly(optionsPanel);
    }

    public void OnClickEndingInfo()
    {
        ShowOnly(endingInfoPanel);
    }

    public void OnClickCredits()
    {
        ShowOnly(creditsPanel);
    }

    public void OnClickBack()
    {
        ShowMain();
    }

    public void OnClickQuit()
    {
        Application.Quit();
        // 에디터에서는 종료가 안 되는 게 정상
        Debug.Log("Quit called (works in build).");
    }

    private void ShowMain()
    {
        ShowOnly(mainMenuPanel);
    }

    private void ShowOnly(GameObject target)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(target == mainMenuPanel);
        if (optionsPanel != null) optionsPanel.SetActive(target == optionsPanel);
        if (endingInfoPanel != null) endingInfoPanel.SetActive(target == endingInfoPanel);
        if (creditsPanel != null) creditsPanel.SetActive(target == creditsPanel);
    }
}
