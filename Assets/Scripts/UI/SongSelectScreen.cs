using UnityEngine;
using UnityEngine.UI;

// Simplified song select — no ScrollView, no runtime spawning.
// Place 3 SongCard GameObjects directly in the scene.
// Assign each one in the Inspector slots below.
public class SongSelectScreen : MonoBehaviour
{
    [Header("Song Library")]
    public SongLibrary songLibrary;

    [Header("The 3 song cards — place directly in scene hierarchy")]
    public SongCard card0;
    public SongCard card1;
    public SongCard card2;

    [Header("Back button")]
    public Button backButton;

    void Start()
    {
        AudioManager.Instance?.PlayMenuMusic();

        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayButton();
            GameManager.Instance.GoToMenu();
        });

        // Wire each card to the matching SongData by index.
        // If SongLibrary has fewer than 3 songs, extra cards are hidden.
        SetupCard(card0, 0);
        SetupCard(card1, 1);
        SetupCard(card2, 2);
    }

    void SetupCard(SongCard card, int index)
    {
        if (card == null) return;

        if (songLibrary.songs.Length > index)
        {
            card.gameObject.SetActive(true);
            card.Setup(songLibrary.songs[index]);
        }
        else
        {
            card.gameObject.SetActive(false);
        }
    }
}
