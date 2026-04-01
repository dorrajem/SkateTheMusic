using System;

namespace RhythmGame
{
    public enum NoteType { Whole, Half, Quarter, Eighth }

    [Serializable]
    public class NoteEvent
    {
        public float    timeSeconds;
        public float    durationSeconds;
        public int      midiPitch;
        public int      lane;
        public string   noteName;
        public NoteType noteType;
    }
}
