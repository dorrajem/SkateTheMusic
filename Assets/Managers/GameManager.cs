using UnityEngine;
using UnityEngine.SceneManagement;

namespace RhythmGame
{
    // Place on a GameObject in the Boot scene.
    // Persists across all scene loads.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Library")]
        [SerializeField] SongLibrary songLibrary;

        // The song the player has chosen to play.
        public static SongData SelectedSong { get; private set; }

        // ── Scene name constants ────────────────────────────────────────────
        const string SCENE_MENU        = "MainMenu";
        const string SCENE_SONG_SELECT = "SongSelect";
        const string SCENE_TUTORIAL    = "Tutorial";
        const string SCENE_GAMEPLAY    = "Gameplay";
        const string SCENE_RESULTS     = "Results";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Called by SongCard when the player taps a song.
        public void StartSong(string songId)
        {
            SelectedSong = songLibrary.GetById(songId);
            if (SelectedSong == null) return;
            ResultsPayload.Clear();
            SceneManager.LoadScene(SCENE_GAMEPLAY);
        }

        public void GoToTutorial()    => SceneManager.LoadScene(SCENE_TUTORIAL);
        public void GoToMenu()        => SceneManager.LoadScene(SCENE_MENU);
        public void GoToSongSelect()  => SceneManager.LoadScene(SCENE_SONG_SELECT);
        public void GoToResults()     => SceneManager.LoadScene(SCENE_RESULTS);

        public void RestartCurrentSong()
        {
            ResultsPayload.Clear();
            SceneManager.LoadScene(SCENE_GAMEPLAY);
        }
    }
}
