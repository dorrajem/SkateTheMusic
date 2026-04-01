using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    // Master game clock. Uses AudioSettings.dspTime for sample-accurate sync.
    // Place in the Gameplay scene. Wire references in the Inspector.
    public class BeatmapConductor : MonoBehaviour
    {
        [Header("References")]
        public AudioSource    musicSource;
        public NoteSpawner    noteSpawner;
        public ScoreManager   scoreManager;

        [Header("Timing")]
        public float lookAheadSeconds = 2f;
        public float perfectWindowSec = 0.030f; // ±30 ms
        public float goodWindowSec    = 0.080f; // ±80 ms

        // Runtime state
        List<NoteEvent> _beatmap;
        int             _nextIndex;
        double          _dspStart;
        bool            _running;

        public float SongTime { get; private set; }

        // ── Lifecycle ───────────────────────────────────────────────────────

        void Start()
        {
            SongData song = GameManager.SelectedSong;
            if (song == null) { Debug.LogError("[Conductor] No song selected."); return; }

            _beatmap = MidiParser.Parse(song.midiFileName, song.laneRootPitches);

            musicSource.clip = song.musicClip;

            // Schedule start 100 ms from now so PlayScheduled has headroom
            _dspStart = AudioSettings.dspTime + 0.1;
            musicSource.PlayScheduled(_dspStart);

            _nextIndex = 0;
            _running   = true;
        }

        void Update()
        {
            if (!_running) return;

            // Sample-accurate song time, compensated for player's latency setting
            SongTime = (float)(AudioSettings.dspTime - _dspStart)
                       - SettingsPanel.GetLatencySeconds();

            // Spawn upcoming notes
            while (_nextIndex < _beatmap.Count &&
                   _beatmap[_nextIndex].timeSeconds <= SongTime + lookAheadSeconds)
            {
                noteSpawner.Spawn(_beatmap[_nextIndex], SongTime);
                _nextIndex++;
            }

            // Detect song end
            if (_nextIndex >= _beatmap.Count && !musicSource.isPlaying && SongTime > 1f)
            {
                _running = false;
                scoreManager.FinishSong(GameManager.SelectedSong);
            }
        }

        // ── Hit registration (called by InputAdapter via Gameplay wiring) ──

        public void RegisterInput(int lane)
        {
            NoteEvent nearest = noteSpawner.GetNearestActiveNote(lane, SongTime);
            if (nearest == null) return;

            float diff = Mathf.Abs(SongTime - nearest.timeSeconds);

            if (diff <= perfectWindowSec)
                scoreManager.RegisterHit(nearest, HitGrade.Perfect);
            else if (diff <= goodWindowSec)
                scoreManager.RegisterHit(nearest, HitGrade.Good);
            else
                scoreManager.RegisterMiss(nearest);
        }
    }
}
