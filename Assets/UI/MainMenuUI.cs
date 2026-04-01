using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    // Wire buttons in the Inspector.
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        public Button playButton;
        public Button tutorialButton;
        public Button settingsButton;

        [Header("Panels")]
        public GameObject settingsPanel; // the SettingsPanel GameObject

        void Start()
        {
            settingsPanel.SetActive(false);

            playButton    .onClick.AddListener(() => GameManager.Instance.GoToSongSelect());
            tutorialButton.onClick.AddListener(() => GameManager.Instance.GoToTutorial());
            settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
        }
    }
}
