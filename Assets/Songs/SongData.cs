using System;
using UnityEngine;

namespace RhythmGame
{
    public enum Difficulty { Beginner, Intermediate, Advanced }

    [Serializable]
    public class SongData
    {
        [Header("Identity")]
        public string     songId;           // e.g. "ode_to_joy"
        public string     displayTitle;     // e.g. "Ode to Joy"
        public string     composerName;
        public Sprite     albumArt;

        [Header("Audio & MIDI")]
        public AudioClip  musicClip;
        public string     midiFileName;     // filename only, e.g. "ode_to_joy.mid"
        public float      bpm;
        public float      latencyOffsetMs;  // per-song fine-tune

        [Header("Lane Mapping")]
        // MIDI pitches that anchor the 5 lanes for this song.
        // Example C-major: 60,62,64,65,67 = C4,D4,E4,F4,G4
        public int[]      laneRootPitches = { 60, 62, 64, 65, 67 };

        [Header("Progression")]
        public Difficulty difficulty;
        public int        starThreshold1;
        public int        starThreshold2;
        public int        starThreshold3;

        [Header("Education")]
        public string[]   notesIntroduced; // e.g. { "C4", "D4", "E4" }
        public string     educationalTip;
    }
}
