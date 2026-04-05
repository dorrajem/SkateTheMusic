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

    void OnEnable()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        HandleTouch();
    }

    void HandleTouch()
    {
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 0) return;
        var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            _touchStart = touch.screenPosition;
            _tracking   = true;
        }

        if (_tracking && touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            _tracking = false;
            Vector2 delta = touch.screenPosition - _touchStart;

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
