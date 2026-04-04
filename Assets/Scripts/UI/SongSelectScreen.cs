using UnityEngine;
using UnityEngine.UI;

public class SongSelectScreen : MonoBehaviour
{
    [Header("References")]
    public SongLibrary songLibrary;
    public Transform   cardContainer;  // the Content child inside your ScrollView
    public GameObject  songCardPrefab;
    public Button      backButton;

    void Start()
    {
        backButton.onClick.AddListener(() => GameManager.Instance.GoToMenu());

        foreach (SongData song in songLibrary.songs)
        {
            GameObject go   = Instantiate(songCardPrefab, cardContainer);
            SongCard   card = go.GetComponent<SongCard>();
            card.Setup(song);
        }
    }
}
