using System.Collections;
using UnityEngine;

public class SkaterController : MonoBehaviour
{
    [Header("Lanes")]
    public int   totalLanes    = 5;
    public float laneSpacing   = 2f;
    public float laneOriginY   = -4f;

    [Header("Movement")]
    public float slideDuration   = 0.18f;
    public float overshootFactor = 0.15f;

    public int CurrentLane { get; private set; } = 2;

    Coroutine _slideRoutine;

    void Awake()
    {
        transform.position = LanePosition(CurrentLane);
    }

    public void MoveLeft()  => SlideToLane(CurrentLane - 1);
    public void MoveRight() => SlideToLane(CurrentLane + 1);

    public void SlideToLane(int target)
    {
        target = Mathf.Clamp(target, 0, totalLanes - 1);
        if (target == CurrentLane) return;

        int dir     = target > CurrentLane ? 1 : -1;
        CurrentLane = target;

        // Play slide SFX
        AudioManager.Instance?.PlaySlide();

        if (_slideRoutine != null) StopCoroutine(_slideRoutine);
        _slideRoutine = StartCoroutine(SlideRoutine(LanePosition(target), dir));
    }

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
