using UnityEngine;

namespace RhythmGame
{
    // Attach to the note prefab.
    // Assign the SpriteRenderer in the prefab; sprites are set by NoteSpawner.
    public class NoteVisual : MonoBehaviour
    {
        public NoteEvent NoteData  { get; private set; }
        public int       Lane      { get; private set; }
        public bool      Consumed  { get; set; }

        float _scrollSpeed;

        public void Init(NoteEvent evt, float worldY, float worldX, float scrollSpeed,
                         Sprite sprite)
        {
            NoteData    = evt;
            Lane        = evt.lane;
            _scrollSpeed = scrollSpeed;

            GetComponent<SpriteRenderer>().sprite = sprite;
            transform.position = new Vector3(worldX, worldY, 0f);
        }

        void Update()
        {
            transform.Translate(Vector3.left * _scrollSpeed * Time.deltaTime);
        }
    }
}
