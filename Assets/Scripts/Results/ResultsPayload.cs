using System.Collections.Generic;

// Static carrier — written by ScoreManager, read by ResultsScreen.
// No MonoBehaviour needed. Lives in memory between scene loads.
public static class ResultsPayload
{
    public static SongData         SongData;
    public static int              FinalScore;
    public static int              BestCombo;
    public static List<NoteResult> NoteResults = new List<NoteResult>();

    public static void Clear()
    {
        SongData    = null;
        FinalScore  = 0;
        BestCombo   = 0;
        NoteResults = new List<NoteResult>();
    }
}
