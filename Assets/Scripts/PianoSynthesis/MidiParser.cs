using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public static class MidiParser
{
    static readonly string[] NOTE_NAMES =
        { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    // midiFileName: just the filename, e.g. "ode_to_joy.mid"
    // Place the file in Assets/StreamingAssets/Songs/
    public static List<NoteEvent> Parse(string midiFileName, int[] laneRootPitches)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Songs", midiFileName);

        if (!File.Exists(path))
        {
            Debug.LogError("[MidiParser] File not found: " + path);
            return new List<NoteEvent>();
        }

        MidiFile  midi   = MidiFile.Read(path);
        TempoMap  tempo  = midi.GetTempoMap();
        var       notes  = midi.GetNotes();
        var       result = new List<NoteEvent>();

        foreach (var note in notes)
        {
            double startSec = TimeConverter
                .ConvertTo<MetricTimeSpan>(note.Time, tempo).TotalSeconds;
            double durSec = TimeConverter
                .ConvertTo<MetricTimeSpan>(note.Length, tempo).TotalSeconds;

            int    pitch  = note.NoteNumber;
            int    octave = (pitch / 12) - 1;
            string name   = NOTE_NAMES[pitch % 12] + octave;

            result.Add(new NoteEvent
            {
                timeSeconds     = (float)startSec,
                durationSeconds = (float)durSec,
                midiPitch       = pitch,
                lane            = PitchToLane(pitch, laneRootPitches),
                noteName        = name,
                noteType        = DurToNoteType((float)durSec)
            });
        }

        result.Sort((a, b) => a.timeSeconds.CompareTo(b.timeSeconds));

        // ── Melody filter ────────────────────────────────────────────────────
        // Piano MIDI files contain both a melody (right hand) and accompaniment
        // (left hand) playing simultaneously. Keep only the highest-pitched note
        // within each simultaneous group so only one note appears per beat.
        result = FilterMelody(result);

        Debug.Log("[MidiParser] " + result.Count + " melody notes after filtering.");
        return result;
    }

    // Groups notes that start within MELODY_WINDOW_SEC of each other and keeps
    // only the highest-pitched note from each group (= the melody note).
    const float MELODY_WINDOW_SEC = 0.05f; // 50 ms — tune if needed

    static List<NoteEvent> FilterMelody(List<NoteEvent> notes)
    {
        if (notes.Count == 0) return notes;

        var filtered = new List<NoteEvent>(notes.Count);
        int i = 0;

        while (i < notes.Count)
        {
            float      groupTime = notes[i].timeSeconds;
            NoteEvent  best      = notes[i];
            i++;

            // Collect everything within the window and pick highest pitch.
            while (i < notes.Count &&
                   notes[i].timeSeconds - groupTime < MELODY_WINDOW_SEC)
            {
                if (notes[i].midiPitch > best.midiPitch)
                    best = notes[i];
                i++;
            }

            filtered.Add(best);
        }

        return filtered;
    }

    public static string MidiToName(int pitch)
    {
        int octave = (pitch / 12) - 1;
        return NOTE_NAMES[pitch % 12] + octave;
    }

    static readonly int[] DEFAULT_ROOTS = { 60, 62, 64, 67, 69 }; // C4 D4 E4 G4 A4
    static bool _warnedAboutRoots;

    static int PitchToLane(int pitch, int[] roots)
    {
        // Guard: if the SongData has no lane roots (or corrupted binary data was
        // deserialized as near-zero values), fall back to a sensible default so
        // notes distribute across all 5 lanes instead of all landing on lane 0.
        if (roots == null || roots.Length == 0 || roots[0] < 10)
        {
            if (!_warnedAboutRoots)
            {
                _warnedAboutRoots = true;
                Debug.LogWarning("[MidiParser] laneRootPitches is null, empty, or contains " +
                                 "corrupted values (near-zero). Using default roots " +
                                 "{60,62,64,67,69} (C4 D4 E4 G4 A4). " +
                                 "Fix: open your SongLibrary asset, select each song entry, " +
                                 "and set laneRootPitches to 5 MIDI pitch numbers that cover " +
                                 "the note range of that song.");
            }
            roots = DEFAULT_ROOTS;
        }

        int best = 0, bestDist = int.MaxValue;
        for (int i = 0; i < roots.Length; i++)
        {
            int d = Mathf.Abs(pitch - roots[i]);
            if (d < bestDist) { bestDist = d; best = i; }
        }

        // Clamp so lane index is always valid regardless of roots array length.
        return Mathf.Clamp(best, 0, roots.Length - 1);
    }

    static NoteType DurToNoteType(float dur)
    {
        if (dur >= 1.8f) return NoteType.Whole;
        if (dur >= 0.9f) return NoteType.Half;
        if (dur >= 0.4f) return NoteType.Quarter;
        return NoteType.Eighth;
    }
}
