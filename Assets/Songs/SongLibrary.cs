using UnityEngine;

namespace RhythmGame
{
    [CreateAssetMenu(fileName = "SongLibrary", menuName = "RhythmGame/Song Library")]
    public class SongLibrary : ScriptableObject
    {
        public SongData[] songs;

        public SongData GetById(string id)
        {
            foreach (var s in songs)
                if (s.songId == id) return s;
            Debug.LogWarning($"[SongLibrary] Song '{id}' not found.");
            return null;
        }
    }
}
