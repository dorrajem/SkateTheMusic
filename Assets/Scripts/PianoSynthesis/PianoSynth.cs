using System.Collections.Generic;
using UnityEngine;

// Add this to the same GameObject as GameManager in MainMenu.
// It persists with DontDestroyOnLoad automatically (same object).
public class PianoSynth : MonoBehaviour
{
    public static PianoSynth Instance { get; private set; }

    [Header("Samples")]
    [Tooltip("Chromatic piano clips starting from baseMidiPitch. Index 0 = C3 (MIDI 48).")]
    public AudioClip[] noteClips;
    public int baseMidiPitch = 48; // C3

    [Header("Playback")]
    [Range(0f, 1f)] public float volume = 0.75f;
    public int poolSize = 8;

    AudioSource[]        _pool;
    int                  _poolHead;
    Dictionary<int, int> _clipIndex = new Dictionary<int, int>();

    void Awake()
    {
        if (Instance != null) { Destroy(this); return; }
        Instance = this;

        _pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            _pool[i]             = gameObject.AddComponent<AudioSource>();
            _pool[i].playOnAwake = false;
            _pool[i].volume      = volume;
        }

        for (int i = 0; i < noteClips.Length; i++)
            if (noteClips[i] != null)
                _clipIndex[baseMidiPitch + i] = i;
    }

    public void PlayNote(int midiPitch, float vel = 0.8f)
    {
        AudioClip clip = NearestClip(midiPitch, out int clipPitch);
        if (clip == null) return;

        float pitchShift = Mathf.Pow(2f, (midiPitch - clipPitch) / 12f);

        AudioSource src = _pool[_poolHead % poolSize];
        _poolHead++;
        src.clip   = clip;
        src.pitch  = pitchShift;
        src.volume = volume * vel;
        src.Play();
    }

    AudioClip NearestClip(int midi, out int closestPitch)
    {
        closestPitch = -1;
        int       best   = int.MaxValue;
        AudioClip result = null;

        foreach (var kv in _clipIndex)
        {
            int d = Mathf.Abs(kv.Key - midi);
            if (d < best) { best = d; closestPitch = kv.Key; result = noteClips[kv.Value]; }
        }
        return result;
    }
}
