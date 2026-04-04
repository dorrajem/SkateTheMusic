using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to SongCard.prefab and wire all fields inside the prefab Inspector.
public class SongCard : MonoBehaviour
{
    [Header("UI References")]
    public Image    thumbnail;
    public TMP_Text titleText;
    public TMP_Text composerText;
    public TMP_Text difficultyText;
    public Image[]  starImages;    // exactly 3 elements
    public Sprite   starFilled;
    public Sprite   starEmpty;
    public Button   playButton;

    SongData _song;

    public void Setup(SongData song)
    {
        _song = song;

        if (song.albumArt != null)
            thumbnail.sprite = song.albumArt;

        titleText     .text = song.displayTitle;
        composerText  .text = song.composerName;
        difficultyText.text = song.difficulty.ToString();

        // Show saved star progress.
        int best  = PlayerPrefs.GetInt("best_" + song.songId, 0);
        int stars = best >= song.starThreshold3 ? 3
                  : best >= song.starThreshold2 ? 2
                  : best >= song.starThreshold1 ? 1 : 0;

        for (int i = 0; i < starImages.Length; i++)
            starImages[i].sprite = i < stars ? starFilled : starEmpty;

        playButton.onClick.AddListener(() => GameManager.Instance.StartSong(_song.songId));
    }
}
