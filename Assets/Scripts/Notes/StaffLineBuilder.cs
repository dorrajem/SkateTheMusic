using UnityEngine;

// Attach to an empty GameObject called StaffLines.
// Right-click this component in the Inspector → Build Lines.
// After running, remove this script — the lines are plain GameObjects.
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

    [ContextMenu("Build Lines")]
    void BuildLines()
    {
        // Remove old children first.
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        for (int i = 0; i < lineCount; i++)
        {
            GameObject go = new GameObject("StaffLine_" + i);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;

            float y  = originY + i * spacing;

            LineRenderer lr  = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(-lineLength * 0.5f, y, 0f));
            lr.SetPosition(1, new Vector3( lineLength * 0.5f, y, 0f));
            lr.startWidth    = lineWidth;
            lr.endWidth      = lineWidth;
            lr.useWorldSpace = true;
            lr.loop          = false;
            lr.material      = lineMaterial;
            lr.startColor    = lineColor;
            lr.endColor      = lineColor;
            lr.sortingOrder  = -1; // behind notes and skater

            Debug.Log("Created StaffLine_" + i + " at Y=" + y);
        }

        Debug.Log("Done — built " + lineCount + " staff lines. Remove this script when finished.");
    }
}
