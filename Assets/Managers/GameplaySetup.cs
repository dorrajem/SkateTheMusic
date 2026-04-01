using UnityEngine;

namespace RhythmGame
{
    // One small coordinator that wires InputAdapter events to the correct
    // Gameplay scene systems. Avoids any cross-scene coupling.
    // Place on the same GameObject as InputAdapter, or anywhere in the scene.
    public class GameplaySetup : MonoBehaviour
    {
        [Header("References")]
        public InputAdapter      input;
        public BeatmapConductor  conductor;
        public SkaterController  skater;
        public ScoreManager      scoreManager;
        public GameplayUI        ui;

        void Awake()
        {
            // Route input events to skater movement
            input.OnMoveLeft .AddListener(() =>
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
}
