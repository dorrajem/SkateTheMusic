using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Panels")]
    public GameObject settingsPanel;

    void Start()
    {
        settingsPanel.SetActive(false);
        playButton    .onClick.AddListener(() => GameManager.Instance.GoToSongSelect());
        settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
        if (quitButton != null) quitButton.onClick.AddListener(() => Application.Quit());
    }
}
