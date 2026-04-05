using UnityEngine;

// Attach to the NotePrefab.
// NoteSpawner calls Init() after instantiating.
public class NoteVisual : MonoBehaviour
{
    public NoteEvent NoteData    { get; private set; }
    public int       Lane        { get; private set; }
    public bool      Consumed    { get; set; }

    float _scrollSpeed;

    public void Init(NoteEvent evt, float worldY, float worldX, float scrollSpeed, Sprite sprite)
    {
        NoteData     = evt;
        Lane         = evt.lane;
        _scrollSpeed = scrollSpeed;

        var sr = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sr.sprite = sprite;
        }
        else
        {
            // No art assigned yet — render a visible placeholder so notes are
            // always testable. Assign noteTypeSprites in NoteSpawner's inspector
            // to replace this with real art.
            sr.sprite = UnityEngine.Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                Vector2.one * 0.5f);
            sr.color = new Color(1f, 0.6f, 0.1f); // orange placeholder
            sr.transform.localScale = Vector3.one;
        }

        transform.position = new Vector3(worldX, worldY, 0f);
    }

    void Update()
    {
        transform.Translate(Vector3.left * _scrollSpeed * Time.deltaTime);
    }
}
