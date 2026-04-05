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

    [Header("Lane Fallback — used if anchors are missing or mispositioned")]
    public float laneOriginY  = -4f;  // world Y of lane 0
    public float laneSpacingY =  2f;  // world Y delta per lane

    [Header("Scroll")]
    public float scrollSpeed = 5f;

    [Header("World X")]
    public float hitLineX = -4f;
    public float cullX    = -7f;

    readonly List<NoteVisual> _active = new List<NoteVisual>();

    // Returns the correct world Y for a given lane index.
    float LaneY(int lane)
    {
        if (laneAnchors != null && lane < laneAnchors.Length && laneAnchors[lane] != null)
        {
            float anchorY = laneAnchors[lane].position.y;
            // Sanity-check: if the anchor is way off-screen, fall back to computed value.
            if (Mathf.Abs(anchorY) < 50f)
                return anchorY;

            Debug.LogWarning("[NoteSpawner] laneAnchors[" + lane + "].position.y = " + anchorY +
                             " — looks wrong (off-screen). Using computed lane Y instead. " +
                             "Fix: set LaneAnchors parent to (0,0,0) and give each child its Y.");
        }
        return laneOriginY + lane * laneSpacingY;
    }

    public void Spawn(NoteEvent evt, float currentSongTime)
    {
        float secondsUntilHit = evt.timeSeconds - currentSongTime;
        float spawnX          = hitLineX + scrollSpeed * secondsUntilHit;
        float worldY          = LaneY(evt.lane);

        // Pick sprite — gracefully handle an empty or incomplete array.
        Sprite sprite = null;
        if (noteTypeSprites != null && noteTypeSprites.Length > 0)
            sprite = noteTypeSprites[Mathf.Min((int)evt.noteType, noteTypeSprites.Length - 1)];

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
