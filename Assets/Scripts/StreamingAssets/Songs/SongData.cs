using System;
using UnityEngine;

public enum Difficulty { Beginner, Intermediate, Advanced }

[Serializable]
public class SongData
{
    [Header("Identity")]
    public string     songId;
    public string     displayTitle;
    public string     composerName;
    public Sprite     albumArt;

    [Header("Audio & MIDI")]
    public AudioClip  musicClip;
    public string     midiFileName;   // e.g. "ode_to_joy.mid" — place file in Assets/StreamingAssets/Songs/
    public float      bpm;

    [Header("Lane Mapping")]
    // The 5 MIDI pitches that anchor your 5 lanes.
    // Default = C4 D4 E4 F4 G4
    public int[] laneRootPitches = { 60, 62, 64, 65, 67 };

    [Header("Progression")]
    public Difficulty difficulty;
    public int        starThreshold1;
    public int        starThreshold2;
    public int        starThreshold3;

    [Header("Education")]
    public string[]   notesIntroduced;
    public string     educationalTip;
}
