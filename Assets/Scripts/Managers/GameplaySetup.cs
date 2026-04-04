using UnityEngine;

// Wires InputAdapter events to BeatmapConductor and SkaterController.
// Keeps all the routing in one place so InputAdapter has no direct dependencies.
public class GameplaySetup : MonoBehaviour
{
    [Header("References")]
    public InputAdapter     input;
    public BeatmapConductor conductor;
    public SkaterController skater;
    public ScoreManager     scoreManager;
    public GameplayUI       ui;

    void Awake()
    {
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
