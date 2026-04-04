using UnityEngine;
using UnityEngine.SceneManagement;

// Place this on a GameObject in the MainMenu scene.
// It persists across all scene loads via DontDestroyOnLoad.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Song Library")]
    public SongLibrary songLibrary;

    // The song the player chose — read by BeatmapConductor in Gameplay.
    public static SongData SelectedSong { get; private set; }

    // Scene names — must match exactly what you named them in Build Settings.
    const string SCENE_MENU        = "MainMenu";
    const string SCENE_SONG_SELECT = "SongSelect";
    const string SCENE_GAMEPLAY    = "Gameplay";
    const string SCENE_RESULTS     = "Results";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called by SongCard when the player presses Play.
    public void StartSong(string songId)
    {
        SelectedSong = songLibrary.GetById(songId);
        if (SelectedSong == null) return;

        ResultsPayload.Clear();
        SceneManager.LoadScene(SCENE_GAMEPLAY);
    }

    public void RestartCurrentSong()
    {
        ResultsPayload.Clear();
        SceneManager.LoadScene(SCENE_GAMEPLAY);
    }

    public void GoToMenu()       => SceneManager.LoadScene(SCENE_MENU);
    public void GoToSongSelect() => SceneManager.LoadScene(SCENE_SONG_SELECT);
    public void GoToResults()    => SceneManager.LoadScene(SCENE_RESULTS);
}
