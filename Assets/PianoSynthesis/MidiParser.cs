using System.Collections.Generic;
using UnityEngine;
// Requires DryWetMidi — drop Melanchall.DryWetMidi.dll into Assets/Plugins/
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace RhythmGame
{
    public static class MidiParser
    {
        static readonly string[] NOTE_NAMES =
            { "C","C#","D","D#","E","F","F#","G","G#","A","A#","B" };

        // midiFileName: filename only, e.g. "ode_to_joy.mid"
        // Lives in Application.streamingAssetsPath/Songs/
        public static List<NoteEvent> Parse(string midiFileName, int[] laneRootPitches)
        {
            string path = System.IO.Path.Combine(
                Application.streamingAssetsPath, "Songs", midiFileName);

            if (!System.IO.File.Exists(path))
            {
                Debug.LogError($"[MidiParser] File not found: {path}");
                return new List<NoteEvent>();
            }

            MidiFile   midi  = MidiFile.Read(path);
            TempoMap   tempo = midi.GetTempoMap();
            var        notes = midi.GetNotes();
            var        result = new List<NoteEvent>();

            foreach (var note in notes)
            {
                double startSec = TimeConverter
                    .ConvertTo<MetricTimeSpan>(note.Time, tempo).TotalSeconds;
                double durSec = TimeConverter
                    .ConvertTo<MetricTimeSpan>(note.Length, tempo).TotalSeconds;

                int pitch  = note.NoteNumber;
                int octave = pitch / 12 - 1;
                string name = NOTE_NAMES[pitch % 12] + octave;

                result.Add(new NoteEvent
                {
                    timeSeconds     = (float)startSec,
                    durationSeconds = (float)durSec,
                    midiPitch       = pitch,
                    lane            = PitchToLane(pitch, laneRootPitches),
                    noteName        = name,
                    noteType        = DurationToNoteType((float)durSec)
                });
            }

            result.Sort((a, b) => a.timeSeconds.CompareTo(b.timeSeconds));
            return result;
        }

        // Public helper so other scripts can display a note name from a raw pitch
        public static string MidiToName(int pitch)
        {
            int octave = pitch / 12 - 1;
            return NOTE_NAMES[pitch % 12] + octave;
        }

        static int PitchToLane(int pitch, int[] roots)
        {
            int best = 0, bestDist = int.MaxValue;
            for (int i = 0; i < roots.Length; i++)
            {
                int d = Mathf.Abs(pitch - roots[i]);
                if (d < bestDist) { bestDist = d; best = i; }
            }
            return best;
        }

        // Threshold in seconds — tune if your songs use a different BPM range
        static NoteType DurationToNoteType(float dur)
        {
            if (dur >= 1.8f) return NoteType.Whole;
            if (dur >= 0.9f) return NoteType.Half;
            if (dur >= 0.4f) return NoteType.Quarter;
            return NoteType.Eighth;
        }
    }
}
