using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the skater's lane position.
///
/// KEYBOARD INPUT (PC / Editor) is read DIRECTLY here via Keyboard.current
/// so it is never blocked by event-chain failures or Game View focus issues.
///
/// Mobile / touch input is handled by InputAdapter → GameplaySetup → MoveLeft/MoveRight.
/// </summary>
public class SkaterController : MonoBehaviour
{
    [Header("Lanes")]
    public int   totalLanes  = 5;
    public float laneSpacing = 2f;
    public float laneOriginY = -4f;

    [Header("Movement")]
    public float slideDuration   = 0.18f;
    public float overshootFactor = 0.15f;

    [Header("Hit Detection — auto-discovered if left blank")]
    public BeatmapConductor conductor;

    public int CurrentLane { get; private set; } = 2;

    Coroutine _slideRoutine;

    // ── Lifecycle ──────────────────────────────────────────────────────────

    void Awake()
    {
        transform.position = LanePosition(CurrentLane);

        // Auto-discover conductor if not assigned in Inspector.
#pragma warning disable CS0618
        if (conductor == null)
            conductor = FindObjectOfType<BeatmapConductor>();
#pragma warning restore CS0618

        Debug.Log("[SkaterController] Awake. Lane=" + CurrentLane +
                  "  pos=" + transform.position +
                  "  conductor=" + (conductor != null ? conductor.gameObject.name : "NULL"));

        // Allow the Input System to receive events even when the Game View
        // is not focused — fixes the most common "keys do nothing" issue in Editor.
#if UNITY_EDITOR
        InputSystem.settings.backgroundBehavior =
            InputSettings.BackgroundBehavior.IgnoreFocus;
        Debug.Log("[SkaterController] Input System set to IgnoreFocus (editor only).");
#endif
    }

    // ── Keyboard input (PC / Editor only) ─────────────────────────────────
    // Reading input HERE instead of through InputAdapter → GameplaySetup
    // eliminates every possible failure point in the event chain.

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // UP arrow / W → higher lane
        if (kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame)
        {
            Debug.Log("[SkaterController] ↑ key → MoveRight  (lane " +
                      CurrentLane + " → " + Mathf.Min(CurrentLane + 1, totalLanes - 1) + ")");
            MoveRight();
            conductor?.RegisterInput(CurrentLane);
        }

        // DOWN arrow / S → lower lane
        if (kb.downArrowKey.wasPressedThisFrame || kb.sKey.wasPressedThisFrame)
        {
            Debug.Log("[SkaterController] ↓ key → MoveLeft  (lane " +
                      CurrentLane + " → " + Mathf.Max(CurrentLane - 1, 0) + ")");
            MoveLeft();
            conductor?.RegisterInput(CurrentLane);
        }

        // Space / Enter → hit note in current lane
        if (kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame)
        {
            Debug.Log("[SkaterController] HIT key → RegisterInput lane " + CurrentLane);
            conductor?.RegisterInput(CurrentLane);
        }
    }

    // ── Public movement API (also used by GameplaySetup for touch events) ─

    public void MoveLeft()  => SlideToLane(CurrentLane - 1);
    public void MoveRight() => SlideToLane(CurrentLane + 1);

    public void SlideToLane(int target)
    {
        target = Mathf.Clamp(target, 0, totalLanes - 1);
        if (target == CurrentLane) return;

        int dir     = target > CurrentLane ? 1 : -1;
        CurrentLane = target;

        AudioManager.Instance?.PlaySlide();

        if (_slideRoutine != null) StopCoroutine(_slideRoutine);
        _slideRoutine = StartCoroutine(SlideRoutine(LanePosition(target), dir));
    }

    // ── Slide animation ────────────────────────────────────────────────────

    IEnumerator SlideRoutine(Vector3 target, int dir)
    {
        Vector3 start     = transform.position;
        Vector3 overshoot = target + Vector3.up * (laneSpacing * overshootFactor * dir);
        float   elapsed   = 0f;

        float p1 = slideDuration * 0.65f;
        while (elapsed < p1)
        {
            elapsed += Time.deltaTime;
            float t  = Mathf.SmoothStep(0f, 1f, elapsed / p1);
            transform.position   = Vector3.Lerp(start, overshoot, t);
            float s              = 1f - 0.15f * t;
            transform.localScale = new Vector3(s, 2f - s, 1f);
            yield return null;
        }

        elapsed = 0f;
        float p2 = slideDuration * 0.35f;
        while (elapsed < p2)
        {
            elapsed += Time.deltaTime;
            float t  = elapsed / p2;
            transform.position   = Vector3.Lerp(overshoot, target, t);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t);
            yield return null;
        }

        transform.position   = target;
        transform.localScale = Vector3.one;
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    Vector3 LanePosition(int lane) =>
        new Vector3(transform.position.x, laneOriginY + lane * laneSpacing, 0f);

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < totalLanes; i++)
            Gizmos.DrawWireSphere(
                new Vector3(transform.position.x, laneOriginY + i * laneSpacing, 0f), 0.2f);
    }
}
