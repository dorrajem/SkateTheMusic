using UnityEngine;

// Add to the Persistent GameObject in MainMenu alongside GameManager.
// Handles background music (loops between scenes) and UI sound effects.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    public AudioClip menuMusic;       // loops on Main Menu and Song Select
    public AudioClip gameplayMusic;   // optional ambient loop under the song
    [Range(0f, 1f)] public float musicVolume = 0.4f;

    [Header("SFX")]
    public AudioClip sfxSlide;        // skater moves between lanes
    public AudioClip sfxPerfect;      // perfect hit
    public AudioClip sfxGood;         // good hit
    public AudioClip sfxMiss;         // miss
    public AudioClip sfxButton;       // any UI button press
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    AudioSource _musicSource;
    AudioSource _sfxSource;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        // GameManager already calls DontDestroyOnLoad on this GameObject

        // Music source — loops continuously
        _musicSource             = gameObject.AddComponent<AudioSource>();
        _musicSource.loop        = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume      = musicVolume;

        // SFX source — one-shot, no loop
        _sfxSource             = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop        = false;
        _sfxSource.playOnAwake = false;
        _sfxSource.volume      = sfxVolume;
    }

    // ── Music ───────────────────────────────────────────────────────────────

    public void PlayMenuMusic()
    {
        if (menuMusic == null) return;
        if (_musicSource.clip == menuMusic && _musicSource.isPlaying) return;
        _musicSource.clip = menuMusic;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    // Call this when entering Gameplay so the menu music fades out cleanly.
    public void StopMusicForGameplay()
    {
        _musicSource.Stop();
    }

    // ── SFX ─────────────────────────────────────────────────────────────────

    public void PlaySlide()
    {
        PlaySFX(sfxSlide);
    }

    public void PlayHitFeedback(HitGrade grade)
    {
        switch (grade)
        {
            case HitGrade.Perfect: PlaySFX(sfxPerfect); break;
            case HitGrade.Good:    PlaySFX(sfxGood);    break;
            case HitGrade.Miss:    PlaySFX(sfxMiss);    break;
        }
    }

    public void PlayButton()
    {
        PlaySFX(sfxButton);
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ── Volume control (called by SettingsPanel) ────────────────────────────

    public void SetMusicVolume(float v)
    {
        musicVolume          = v;
        _musicSource.volume  = v;
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume           = v;
        _sfxSource.volume   = v;
    }
}
