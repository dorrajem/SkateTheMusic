using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("HUD")]
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text noteNameText;
    public TMP_Text songTitleText;

    [Header("Feedback")]
    // Animator with triggers: "perfect", "good", "miss"
    public Animator feedbackAnimator;

    [Header("Pause")]
    public Button     pauseButton;
    public GameObject pauseMenuPanel;

    [Header("References")]
    public ScoreManager  scoreManager;
    public BeatmapConductor conductor;

    bool _paused;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        pauseButton.onClick.AddListener(TogglePause);

        if (GameManager.SelectedSong != null)
            songTitleText.text = GameManager.SelectedSong.displayTitle;
    }

    void Update()
    {
        scoreText.text = scoreManager.Score.ToString("N0");
        comboText.text = scoreManager.Combo > 1 ? "x" + scoreManager.Combo : "";
    }

    // Called externally after a hit or miss to flash the note name.
    public void ShowFeedback(HitGrade grade, string noteName)
    {
        noteNameText.text = noteName;
        string trigger = grade == HitGrade.Perfect ? "perfect"
                       : grade == HitGrade.Good    ? "good"
                                                   : "miss";
        feedbackAnimator?.SetTrigger(trigger);
    }

    void TogglePause()
    {
        _paused = !_paused;
        Time.timeScale = _paused ? 0f : 1f;
        pauseMenuPanel.SetActive(_paused);
    }
}
