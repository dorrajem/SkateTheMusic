using System.Collections.Generic;

namespace RhythmGame
{
    // Written by ScoreManager at end of song.
    // Read by ResultsScreen on the next scene load.
    public static class ResultsPayload
    {
        public static SongData          SongData;
        public static int               FinalScore;
        public static int               BestCombo;
        public static List<NoteResult>  NoteResults = new List<NoteResult>();

        public static void Clear()
        {
            SongData    = null;
            FinalScore  = 0;
            BestCombo   = 0;
            NoteResults = new List<NoteResult>();
        }
    }
}
