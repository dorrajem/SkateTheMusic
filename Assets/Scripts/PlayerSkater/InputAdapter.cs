using UnityEngine;
using UnityEngine.Events;

// Place on GameController alongside BeatmapConductor.
// Leave the UnityEvents empty in the Inspector —
// GameplaySetup.Awake() wires them in code.
public class InputAdapter : MonoBehaviour
{
    [Header("Events — wired by GameplaySetup at runtime")]
    public UnityEvent OnMoveLeft;
    public UnityEvent OnMoveRight;
    public UnityEvent OnHit;

    [Header("Touch swipe threshold (pixels)")]
    public float swipeThresholdPx = 40f;

    Vector2 _touchStart;
    bool    _tracking;

    void Update()
    {
        HandleKeyboard();

#if UNITY_IOS || UNITY_ANDROID
        HandleTouch();
#endif
    }

    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)  || Input.GetKeyDown(KeyCode.A))
            OnMoveLeft?.Invoke();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            OnMoveRight?.Invoke();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            OnHit?.Invoke();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _touchStart = touch.position;
            _tracking   = true;
        }

        if (_tracking && touch.phase == TouchPhase.Ended)
        {
            _tracking = false;
            Vector2 delta = touch.position - _touchStart;

            if (Mathf.Abs(delta.x) >= swipeThresholdPx)
            {
                if (delta.x > 0) OnMoveRight?.Invoke();
                else             OnMoveLeft?.Invoke();
            }
            else if (delta.magnitude < swipeThresholdPx * 0.5f)
            {
                OnHit?.Invoke();
            }
        }
    }
}
