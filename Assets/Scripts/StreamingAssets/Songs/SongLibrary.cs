using UnityEngine;

[CreateAssetMenu(fileName = "SongLibrary", menuName = "RhythmGame/Song Library")]
public class SongLibrary : ScriptableObject
{
    public SongData[] songs;

    public SongData GetById(string id)
    {
        foreach (SongData s in songs)
            if (s.songId == id) return s;

        Debug.LogWarning("[SongLibrary] Song not found: " + id);
        return null;
    }
}
