using UnityEngine;

/// <summary>
/// Wires InputAdapter events → BeatmapConductor + SkaterController.
/// References can be assigned in the Inspector OR are auto-discovered
/// at startup, so the binding survives scene reorganisations.
/// </summary>
public class GameplaySetup : MonoBehaviour
{
    [Header("References — assign in Inspector or leave blank to auto-discover")]
    public InputAdapter     input;
    public BeatmapConductor conductor;
    public SkaterController skater;
    public ScoreManager     scoreManager;
    public GameplayUI       ui;

    void Awake()
    {
        // ── Auto-discover any unassigned references ───────────────────────
        if (input       == null) input       = GetComponent<InputAdapter>();
        if (conductor   == null) conductor   = GetComponent<BeatmapConductor>();
        if (scoreManager== null) scoreManager= GetComponent<ScoreManager>();

        // InputAdapter may be on a different GO — fall back to scene-wide search.
        if (input == null)
        {
#pragma warning disable CS0618
            input = FindObjectOfType<InputAdapter>();
#pragma warning restore CS0618
            if (input != null)
                Debug.Log("[GameplaySetup] Found InputAdapter via scene search on: '" + input.gameObject.name + "'");
        }

        // SkaterController lives on a separate Skater GO — search scene-wide.
        if (skater == null)
        {
#pragma warning disable CS0618
            skater = FindObjectOfType<SkaterController>();
#pragma warning restore CS0618
        }

        // GameplayUI lives on the UI canvas — search scene-wide.
        if (ui == null)
        {
#pragma warning disable CS0618
            ui = FindObjectOfType<GameplayUI>();
#pragma warning restore CS0618
        }

        // ── Guard-rail: report anything still missing ────────────────────
        bool ok = true;
        if (input     == null) { Debug.LogError("[GameplaySetup] " + name + ": 'input' (InputAdapter) is null. " +
                                                "Add an InputAdapter to this GameObject or assign it in the Inspector."); ok = false; }
        if (conductor == null) { Debug.LogError("[GameplaySetup] " + name + ": 'conductor' (BeatmapConductor) is null. " +
                                                "Add a BeatmapConductor to this GameObject or assign it in the Inspector."); ok = false; }
        if (skater    == null) { Debug.LogError("[GameplaySetup] " + name + ": 'skater' (SkaterController) could not be found. " +
                                                "Make sure a Skater GameObject with a SkaterController component exists in the scene."); ok = false; }
        if (!ok) return; // stop here — wiring with nulls would crash on input

        Debug.Log("[GameplaySetup] ✓ Wired successfully:" +
                  "\n  input     → '" + input.gameObject.name + "' (" + input.GetType().Name + ")" +
                  "\n  conductor → '" + conductor.gameObject.name + "'" +
                  "\n  skater    → '" + skater.gameObject.name + "' at position " + skater.transform.position +
                  "\n  (If skater position is (960,490,0) or similar, it's the WRONG/invisible SkaterController!" +
                  " The real Skater should be near X=-5, Y=0 in world space.)");

        // ── Wire input events ────────────────────────────────────────────
        input.OnMoveLeft.AddListener(() =>
        {
            skater.MoveLeft();
            conductor.RegisterInput(skater.CurrentLane);
        });

        input.OnMoveRight.AddListener(() =>
        {
            skater.MoveRight();
            conductor.RegisterInput(skater.CurrentLane);
        });

        input.OnHit.AddListener(() =>
        {
            conductor.RegisterInput(skater.CurrentLane);
        });
    }
}
