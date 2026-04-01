using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    // Instantiates NoteVisual prefabs and manages their lifetime.
    public class NoteSpawner : MonoBehaviour
    {
        [Header("Prefab & Sprites")]
        public GameObject notePrefab;
        // Index matches NoteType enum: 0=Whole, 1=Half, 2=Quarter, 3=Eighth
        public Sprite[] noteTypeSprites;

        [Header("Lane positions (world Y)")]
        // Assign 5 transforms in the Inspector — one per lane
        public Transform[] laneAnchors;

        [Header("Scroll")]
        public float scrollSpeed = 5f;  // units per second; set from BPM at runtime

        [Header("World X positions")]
        public float hitLineX =  -4f;  // where the skater's hit zone is
        public float cullX    =  -7f;  // notes left of this are destroyed

        readonly List<NoteVisual> _active = new();

        // ── Called by BeatmapConductor ──────────────────────────────────────

        public void Spawn(NoteEvent evt, float currentSongTime)
        {
            float secondsUntilHit = evt.timeSeconds - currentSongTime;
            float spawnX          = hitLineX + scrollSpeed * secondsUntilHit;

            float worldY = laneAnchors[evt.lane].position.y;

            Sprite sprite = noteTypeSprites.Length > (int)evt.noteType
                ? noteTypeSprites[(int)evt.noteType]
                : noteTypeSprites[0];

            GameObject go = Instantiate(notePrefab);
            NoteVisual nv = go.GetComponent<NoteVisual>();
            nv.Init(evt, worldY, spawnX, scrollSpeed, sprite);
            _active.Add(nv);
        }

        // Returns the nearest un-consumed note in the given lane,
        // marks it consumed, and removes it from the active list.
        public NoteEvent GetNearestActiveNote(int lane, float songTime)
        {
            NoteVisual best      = null;
            float      bestDist  = float.MaxValue;

            foreach (NoteVisual nv in _active)
            {
                if (nv == null || nv.Consumed || nv.Lane != lane) continue;
                float d = Mathf.Abs(nv.NoteData.timeSeconds - songTime);
                if (d < bestDist) { bestDist = d; best = nv; }
            }

            if (best == null) return null;

            best.Consumed = true;
            _active.Remove(best);
            Destroy(best.gameObject);
            return best.NoteData;
        }

        void Update()
        {
            // Destroy notes that scrolled past the cull line without being hit
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i] == null || _active[i].transform.position.x < cullX)
                {
                    if (_active[i] != null) Destroy(_active[i].gameObject);
                    _active.RemoveAt(i);
                }
            }
        }
    }
}
