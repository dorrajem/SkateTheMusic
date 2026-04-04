using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Prefab & Sprites")]
    public GameObject notePrefab;
    // 0=Whole, 1=Half, 2=Quarter, 3=Eighth — must match NoteType enum order
    public Sprite[] noteTypeSprites;

    [Header("Lanes")]
    public Transform[] laneAnchors; // 5 empty GameObjects at Y = -4,-2,0,2,4

    [Header("Scroll")]
    public float scrollSpeed = 5f;

    [Header("World X")]
    public float hitLineX = -4f;
    public float cullX    = -7f;

    readonly List<NoteVisual> _active = new List<NoteVisual>();

    public void Spawn(NoteEvent evt, float currentSongTime)
    {
        float secondsUntilHit = evt.timeSeconds - currentSongTime;
        float spawnX          = hitLineX + scrollSpeed * secondsUntilHit;
        float worldY          = laneAnchors[evt.lane].position.y;

        Sprite sprite = (noteTypeSprites.Length > (int)evt.noteType)
            ? noteTypeSprites[(int)evt.noteType]
            : noteTypeSprites[0];

        GameObject go = Instantiate(notePrefab);
        NoteVisual nv = go.GetComponent<NoteVisual>();
        nv.Init(evt, worldY, spawnX, scrollSpeed, sprite);
        _active.Add(nv);
    }

    // Returns the nearest un-consumed note in the given lane and marks it consumed.
    public NoteEvent GetNearestActiveNote(int lane, float songTime)
    {
        NoteVisual best     = null;
        float      bestDist = float.MaxValue;

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
