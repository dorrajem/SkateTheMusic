using System.Collections.Generic;
using UnityEngine;

public class BeatmapConductor : MonoBehaviour
{
    [Header("References")]
    public AudioSource  musicSource;
    public NoteSpawner  noteSpawner;
    public ScoreManager scoreManager;

    [Header("Timing")]
    public float lookAheadSeconds = 2f;
    public float perfectWindowSec = 0.030f;
    public float goodWindowSec    = 0.080f;

    List<NoteEvent> _beatmap;
    int             _nextIndex;
    double          _dspStart;
    bool            _running;

    // Other scripts can read this to know the current playback position.
    public float SongTime { get; private set; }

    void Start()
    {
        SongData song = GameManager.SelectedSong;

        if (song == null)
        {
            Debug.LogError("[BeatmapConductor] No song selected. Play from the Main Menu.");
            return;
        }

        _beatmap = MidiParser.Parse(song.midiFileName, song.laneRootPitches);
        Debug.Log("[BeatmapConductor] Loaded " + _beatmap.Count + " notes for: " + song.displayTitle);

        musicSource.clip = song.musicClip;

        // Schedule 100ms ahead so PlayScheduled has headroom.
        _dspStart = AudioSettings.dspTime + 0.1;
        musicSource.PlayScheduled(_dspStart);

        _nextIndex = 0;
        _running   = true;
    }

    void Update()
    {
        if (!_running) return;

        // Latency offset read from PlayerPrefs (set by SettingsPanel).
        float latency = PlayerPrefs.GetFloat("latency_offset_ms", 0f) / 1000f;
        SongTime = (float)(AudioSettings.dspTime - _dspStart) - latency;

        // Spawn notes entering the lookahead window.
        while (_nextIndex < _beatmap.Count &&
               _beatmap[_nextIndex].timeSeconds <= SongTime + lookAheadSeconds)
        {
            noteSpawner.Spawn(_beatmap[_nextIndex], SongTime);
            _nextIndex++;
        }

        // Detect song end.
        if (_nextIndex >= _beatmap.Count && !musicSource.isPlaying && SongTime > 1f)
        {
            _running = false;
            scoreManager.FinishSong(GameManager.SelectedSong);
        }
    }

    // Called by InputAdapter (via GameplaySetup) when the player hits.
    public void RegisterInput(int lane)
    {
        NoteEvent nearest = noteSpawner.GetNearestActiveNote(lane, SongTime);
        if (nearest == null) return;

        float diff = Mathf.Abs(SongTime - nearest.timeSeconds);

        if      (diff <= perfectWindowSec) scoreManager.RegisterHit(nearest, HitGrade.Perfect);
        else if (diff <= goodWindowSec)    scoreManager.RegisterHit(nearest, HitGrade.Good);
        else                               scoreManager.RegisterMiss(nearest);
    }
}
