using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManagerMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameSelectionPanel;
    public GameObject creditsPanel;

    [Header("Buttons")]
    public GameObject startButton;
    public GameObject quitButton;
    public GameObject creditsButton;
    public GameObject game1Button;
    public GameObject game2Button;
    public GameObject game3Button;
    public GameObject closeGameSelectionPanelButton;
    public GameObject closeCreditsPanelButton;

    void Start()
    {
        // Hide panels at the start
        gameSelectionPanel.SetActive(false);
        creditsPanel.SetActive(false);

        // Assign button click events
        startButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ToggleGameSelectionPanel);
        quitButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(QuitGame);
        creditsButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ToggleCreditsPanel);
        game1Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadGameScene(1));
        game2Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadGameScene(2));
        game3Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadGameScene(3));
        closeGameSelectionPanelButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CloseGameSelectionPanel);
        closeCreditsPanelButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CloseCreditsPanel);
    }

    public void ToggleGameSelectionPanel()
    {
        gameSelectionPanel.SetActive(!gameSelectionPanel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ToggleCreditsPanel()
    {
        creditsPanel.SetActive(!creditsPanel.activeSelf);
    }

    public void CloseGameSelectionPanel()
    {
        gameSelectionPanel.SetActive(false);
    }

    public void CloseCreditsPanel()
    {
        creditsPanel.SetActive(false);
    }

    public void LoadGameScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
