using System.Collections;
using UnityEngine;


    // Shared between Tutorial and Gameplay scenes.
    // Place on the skater GameObject.
    public class SkaterController : MonoBehaviour
    {
        [Header("Lanes")]
        public int   totalLanes  = 5;
        public float laneSpacing = 2f;   // world-unit gap between lane centres
        public float laneOriginY = -4f;  // Y position of lane 0

        [Header("Movement")]
        public float slideDuration   = 0.18f;
        public float overshootFactor = 0.15f; // fraction of laneSpacing to overshoot

        [Header("References")]
        public Animator skaterAnimator; // optional — assign if you have one

        public int CurrentLane { get; private set; } = 2; // start centre

        Coroutine _slideRoutine;

        void Awake()
        {
            transform.position = LanePosition(CurrentLane);
        }

        // ── Public API ──────────────────────────────────────────────────────

        public void SlideToLane(int target)
        {
            target = Mathf.Clamp(target, 0, totalLanes - 1);
            if (target == CurrentLane) return;

            int dir    = target > CurrentLane ? 1 : -1;
            CurrentLane = target;

            if (_slideRoutine != null) StopCoroutine(_slideRoutine);
            _slideRoutine = StartCoroutine(SlideRoutine(LanePosition(target), dir));
        }

        public void MoveLeft()  => SlideToLane(CurrentLane - 1);
        public void MoveRight() => SlideToLane(CurrentLane + 1);

        public void PlayHitFeedback()
        {
            skaterAnimator?.SetTrigger("hit");
        }

        public void PlayMissFeedback()
        {
            skaterAnimator?.SetTrigger("miss");
        }

        // ── Internals ───────────────────────────────────────────────────────

        IEnumerator SlideRoutine(Vector3 target, int dir)
        {
            skaterAnimator?.SetBool("sliding", true);

            Vector3 start     = transform.position;
            Vector3 overshoot = target + Vector3.up * (laneSpacing * overshootFactor * dir);
            float   elapsed   = 0f;

            // Phase 1: ease toward overshoot
            float p1 = slideDuration * 0.65f;
            while (elapsed < p1)
            {
                elapsed += Time.deltaTime;
                float t  = Mathf.SmoothStep(0f, 1f, elapsed / p1);
                transform.position = Vector3.Lerp(start, overshoot, t);
                // Squash & stretch
                float s = 1f - 0.15f * t;
                transform.localScale = new Vector3(s, 2f - s, 1f);
                yield return null;
            }

            // Phase 2: settle to exact lane
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
            skaterAnimator?.SetBool("sliding", false);
        }

        Vector3 LanePosition(int lane) =>
            new Vector3(transform.position.x, laneOriginY + lane * laneSpacing, 0f);

        // Visualise lanes in Scene view
        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < totalLanes; i++)
                Gizmos.DrawWireSphere(
                    new Vector3(transform.position.x, laneOriginY + i * laneSpacing, 0f),
                    0.2f);
        }
    }
