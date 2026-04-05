using UnityEngine;

/// <summary>
/// Draws the 5 horizontal staff-lane lines.
/// Works two ways:
///   1. Editor tool: right-click the component → Build Lines in edit mode.
///   2. Runtime auto-build: if no child lines exist when Play starts, lines
///      are created automatically. This means you never need to "remember"
///      to run the editor tool first.
/// </summary>
public class StaffLineBuilder : MonoBehaviour
{
    [Header("Settings")]
    public int   lineCount   = 5;
    public float originY     = -4f;
    public float spacing     = 2f;
    public float lineWidth   = 0.04f;
    public float lineLength  = 26f;
    public Color lineColor   = new Color(0.22f, 0.18f, 0.14f, 1f);
    public Material lineMaterial; // assign any Sprites/Default material

    // ────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        // Reset the parent transform so lines sit correctly in world space.
        // "Build Lines" in the editor positions points in world space, so the
        // parent transform must be at (0,0,0). If the scene somehow moved it,
        // this corrects it at startup.
        transform.position   = Vector3.zero;
        transform.rotation   = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Build lines at runtime if they are missing or have collapsed positions.
        if (!LinesAreValid())
            BuildLinesRuntime();
    }

    // ────────────────────────────────────────────────────────────────────────
    // Editor tool: right-click → Build Lines
    // ────────────────────────────────────────────────────────────────────────

    [ContextMenu("Build Lines")]
    void BuildLines()
    {
        // Remove old children first.
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        CreateLines();
        Debug.Log("[StaffLineBuilder] Built " + lineCount + " staff lines (editor).");
    }

    // ────────────────────────────────────────────────────────────────────────
    // Runtime helpers
    // ────────────────────────────────────────────────────────────────────────

    bool LinesAreValid()
    {
        if (transform.childCount != lineCount) return false;

        // Check that at least the first line has sensible world-space positions.
        var lr = transform.GetChild(0).GetComponent<LineRenderer>();
        if (lr == null) return false;

        Vector3 p0 = lr.GetPosition(0);
        Vector3 p1 = lr.GetPosition(1);
        // Both positions at the same XY means a collapsed/un-set line.
        return Mathf.Abs(p1.x - p0.x) > 0.1f;
    }

    void BuildLinesRuntime()
    {
        // Remove stale children.
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        CreateLines();
        Debug.Log("[StaffLineBuilder] Built " + lineCount + " staff lines (runtime auto-build).");
    }

    void CreateLines()
    {
        for (int i = 0; i < lineCount; i++)
        {
            float y = originY + i * spacing;

            GameObject go = new GameObject("StaffLine_" + i);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;

            LineRenderer lr  = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(-lineLength * 0.5f, y, 0f));
            lr.SetPosition(1, new Vector3( lineLength * 0.5f, y, 0f));
            lr.startWidth    = lineWidth;
            lr.endWidth      = lineWidth;
            lr.useWorldSpace = true;
            lr.loop          = false;
            lr.sortingOrder  = -1; // render behind notes and skater

            // Use the assigned material, or fall back to the default
            // Sprites/Default shader so the line is always visible.
            if (lineMaterial != null)
            {
                lr.material = lineMaterial;
            }
            else
            {
                lr.material = new Material(Shader.Find("Sprites/Default"));
            }

            lr.startColor = lineColor;
            lr.endColor   = lineColor;
        }
    }
}
