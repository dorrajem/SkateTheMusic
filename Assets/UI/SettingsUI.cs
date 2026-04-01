using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    // Reused in MainMenu and PauseMenu.
    // Saves settings to PlayerPrefs immediately on change.
    public class SettingsPanel : MonoBehaviour
    {
        [Header("Controls")]
        public Slider latencySlider;   // range: -200 to 200 ms
        public Slider musicSlider;     // range: 0 to 1
        public Slider sfxSlider;       // range: 0 to 1
        public Button closeButton;

        const string KEY_LATENCY = "latency_offset_ms";
        const string KEY_MUSIC   = "music_volume";
        const string KEY_SFX     = "sfx_volume";

        void OnEnable()
        {
            // Load saved values (defaults: 0ms latency, full volume)
            latencySlider.value = PlayerPrefs.GetFloat(KEY_LATENCY, 0f);
            musicSlider  .value = PlayerPrefs.GetFloat(KEY_MUSIC,   1f);
            sfxSlider    .value = PlayerPrefs.GetFloat(KEY_SFX,     1f);

            latencySlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(KEY_LATENCY, v));
            musicSlider  .onValueChanged.AddListener(v =>
            {
                PlayerPrefs.SetFloat(KEY_MUSIC, v);
                AudioListener.volume = v; // simple global volume control
            });
            sfxSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(KEY_SFX, v));

            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        void OnDisable()
        {
            latencySlider.onValueChanged.RemoveAllListeners();
            musicSlider  .onValueChanged.RemoveAllListeners();
            sfxSlider    .onValueChanged.RemoveAllListeners();
            closeButton  .onClick       .RemoveAllListeners();
            PlayerPrefs.Save();
        }

        // Static helper so BeatmapConductor can read latency easily
        public static float GetLatencySeconds() =>
            PlayerPrefs.GetFloat(KEY_LATENCY, 0f) / 1000f;
    }
}
