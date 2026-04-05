using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("HUD — always visible")]
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text songTitleText;

    [Header("Note name — flashes on hit")]
    public TMP_Text noteNameText;   // place this near the skater

    [Header("Hit feedback popup")]
    // One TMP_Text centred on screen (or near the hit line).
    // This script handles showing and hiding it — no Animator needed.
    public TMP_Text feedbackText;

    [Header("Feedback colours")]
    public Color colorPerfect  = new Color(0.30f, 0.80f, 0.40f); // green
    public Color colorGood     = new Color(0.95f, 0.75f, 0.20f); // amber
    public Color colorMiss     = new Color(0.90f, 0.30f, 0.25f); // red

    [Header("Pause")]
    public Button     pauseButton;
    public GameObject pauseMenuPanel;

    [Header("References")]
    public ScoreManager     scoreManager;
    public BeatmapConductor conductor;

    Coroutine _feedbackRoutine;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        pauseButton.onClick.AddListener(TogglePause);

        // Start with feedback text invisible
        SetFeedbackAlpha(0f);

        if (GameManager.SelectedSong != null)
            songTitleText.text = GameManager.SelectedSong.displayTitle;
    }

    void Update()
    {
        scoreText.text = scoreManager.Score.ToString("N0");
        comboText.text = scoreManager.Combo > 1 ? "x" + scoreManager.Combo : "";
    }

    // Called by ScoreManager after every hit or miss.
    public void ShowFeedback(HitGrade grade, string noteName)
    {
        noteNameText.text = noteName;

        string word  = grade == HitGrade.Perfect ? "Perfect!"
                     : grade == HitGrade.Good    ? "Good!"
                                                 : "Miss";
        Color  color = grade == HitGrade.Perfect ? colorPerfect
                     : grade == HitGrade.Good    ? colorGood
                                                 : colorMiss;

        if (_feedbackRoutine != null) StopCoroutine(_feedbackRoutine);
        _feedbackRoutine = StartCoroutine(FeedbackPop(word, color));
    }

    // Pops up, holds briefly, then fades out.
    IEnumerator FeedbackPop(string word, Color color)
    {
        feedbackText.text  = word;
        feedbackText.color = color;

        // Pop in — scale from 0.5 to 1 quickly
        float elapsed = 0f;
        float popTime = 0.08f;
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            float t  = elapsed / popTime;
            float s  = Mathf.Lerp(0.5f, 1f, t);
            feedbackText.transform.localScale = Vector3.one * s;
            SetFeedbackAlpha(t);
            yield return null;
        }

        SetFeedbackAlpha(1f);
        feedbackText.transform.localScale = Vector3.one;

        // Hold
        yield return new WaitForSeconds(0.35f);

        // Fade out
        elapsed = 0f;
        float fadeTime = 0.2f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            SetFeedbackAlpha(1f - (elapsed / fadeTime));
            yield return null;
        }

        SetFeedbackAlpha(0f);
    }

    void SetFeedbackAlpha(float a)
    {
        Color c   = feedbackText.color;
        c.a       = a;
        feedbackText.color = c;
    }

    void TogglePause()
    {
        bool willPause = !pauseMenuPanel.activeSelf;
        Time.timeScale = willPause ? 0f : 1f;
        pauseMenuPanel.SetActive(willPause);
    }
}
