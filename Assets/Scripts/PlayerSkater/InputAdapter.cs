using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles TOUCH / SWIPE input for mobile and fires UnityEvents.
/// GameplaySetup.Awake() wires those events to SkaterController and BeatmapConductor.
///
/// KEYBOARD INPUT on PC / Editor is handled DIRECTLY by SkaterController.Update()
/// and does NOT go through this class. This avoids Input System focus issues.
/// </summary>
public class InputAdapter : MonoBehaviour
{
    [Header("Events — wired by GameplaySetup at runtime")]
    public UnityEvent OnMoveLeft;   // mobile swipe down  → lower lane
    public UnityEvent OnMoveRight;  // mobile swipe up    → higher lane
    public UnityEvent OnHit;        // mobile tap         → hit note

    [Header("Touch swipe threshold (pixels)")]
    public float swipeThresholdPx = 40f;

    Vector2 _touchStart;
    bool    _tracking;

    void Awake()
    {
        Debug.Log("[InputAdapter] Active on: '" + gameObject.name + "'" +
                  " — handling TOUCH only. Keyboard input is in SkaterController.");
    }

    // Only process touch; keyboard is handled directly by SkaterController.
    void Update()
    {
        HandleTouch();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        if (touch.phase == UnityEngine.TouchPhase.Began)
        {
            _touchStart = touch.position;
            _tracking   = true;
        }

        if (_tracking && touch.phase == UnityEngine.TouchPhase.Ended)
        {
            _tracking = false;
            Vector2 delta = touch.position - _touchStart;

            if (Mathf.Abs(delta.y) >= swipeThresholdPx)
            {
                if (delta.y > 0) { Debug.Log("[InputAdapter] Swipe UP → OnMoveRight"); OnMoveRight?.Invoke(); }
                else             { Debug.Log("[InputAdapter] Swipe DOWN → OnMoveLeft"); OnMoveLeft?.Invoke();  }
            }
            else if (delta.magnitude < swipeThresholdPx * 0.5f)
            {
                Debug.Log("[InputAdapter] Tap → OnHit");
                OnHit?.Invoke();
            }
        }
    }
}
