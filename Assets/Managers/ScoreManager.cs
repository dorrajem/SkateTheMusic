using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    // Tracks score, combo, and note results during gameplay.
    // Call FinishSong() when the song ends — it fills ResultsPayload and loads Results.
    public class ScoreManager : MonoBehaviour
    {
        [Header("Points per grade")]
        public int pointsPerfect = 300;
        public int pointsGood    = 100;

        // Runtime state
        int              _score;
        int              _combo;
        int              _bestCombo;
        List<NoteResult> _noteResults = new List<NoteResult>();

        // Read by GameplayUI each frame
        public int Score     => _score;
        public int Combo     => _combo;
        public int BestCombo => _bestCombo;

        // ── Called by BeatmapConductor ──────────────────────────────────────

        public void RegisterHit(NoteEvent evt, HitGrade grade)
        {
            _combo++;
            if (_combo > _bestCombo) _bestCombo = _combo;

            int points = (grade == HitGrade.Perfect ? pointsPerfect : pointsGood)
                         + _combo * 10; // combo bonus
            _score += points;

            _noteResults.Add(new NoteResult
            {
                noteName = evt.noteName,
                wasHit   = true,
                grade    = grade
            });
        }

        public void RegisterMiss(NoteEvent evt)
        {
            _combo = 0;
            _noteResults.Add(new NoteResult
            {
                noteName = evt.noteName,
                wasHit   = false,
                grade    = HitGrade.Miss
            });
        }

        // ── Called when song ends ───────────────────────────────────────────

        public void FinishSong(SongData song)
        {
            // Save best score to PlayerPrefs
            string key  = "best_" + song.songId;
            int    prev = PlayerPrefs.GetInt(key, 0);
            if (_score > prev) PlayerPrefs.SetInt(key, _score);
            PlayerPrefs.Save();

            // Fill the static payload for the Results scene to read
            ResultsPayload.SongData    = song;
            ResultsPayload.FinalScore  = _score;
            ResultsPayload.BestCombo   = _bestCombo;
            ResultsPayload.NoteResults = _noteResults;

            GameManager.Instance.GoToResults();
        }
    }
}
