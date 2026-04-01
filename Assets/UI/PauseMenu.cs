using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    // Lives inside the pause panel GameObject.
    // GameplayUI activates/deactivates the panel; this script wires the buttons.
    public class PauseMenu : MonoBehaviour
    {
        [Header("Buttons")]
        public Button resumeButton;
        public Button restartButton;
        public Button quitButton;
        public Button settingsButton;

        [Header("Settings panel")]
        public GameObject settingsPanel;

        void OnEnable()
        {
            // Panel just became visible — make sure game is paused
            Time.timeScale = 0f;

            resumeButton .onClick.AddListener(Resume);
            restartButton.onClick.AddListener(Restart);
            quitButton   .onClick.AddListener(Quit);
            settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
        }

        void OnDisable()
        {
            resumeButton .onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            quitButton   .onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
        }

        void Resume()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }

        void Restart()
        {
            Time.timeScale = 1f;
            GameManager.Instance.RestartCurrentSong();
        }

        void Quit()
        {
            Time.timeScale = 1f;
            GameManager.Instance.GoToMenu();
        }
    }
}
