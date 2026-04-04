using UnityEngine;
using UnityEngine.UI;

// Reused in MainMenu and PauseMenu.
public class SettingsPanel : MonoBehaviour
{
    [Header("Controls")]
    public Slider latencySlider;  // range -200 to 200 ms
    public Slider musicSlider;    // range 0 to 1
    public Button closeButton;

    const string KEY_LATENCY = "latency_offset_ms";
    const string KEY_MUSIC   = "music_volume";

    void OnEnable()
    {
        latencySlider.value = PlayerPrefs.GetFloat(KEY_LATENCY, 0f);
        musicSlider  .value = PlayerPrefs.GetFloat(KEY_MUSIC,   1f);

        latencySlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(KEY_LATENCY, v));
        musicSlider  .onValueChanged.AddListener(v =>
        {
            PlayerPrefs.SetFloat(KEY_MUSIC, v);
            AudioListener.volume = v;
        });
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    void OnDisable()
    {
        latencySlider.onValueChanged.RemoveAllListeners();
        musicSlider  .onValueChanged.RemoveAllListeners();
        closeButton  .onClick.RemoveAllListeners();
        PlayerPrefs.Save();
    }
}
