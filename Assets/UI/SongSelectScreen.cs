using UnityEngine;
using UnityEngine.UI;

namespace RhythmGame
{
    public class SongSelectScreen : MonoBehaviour
    {
        [Header("References")]
        public SongLibrary  songLibrary;
        public Transform    cardContainer;   // ScrollRect content transform
        public GameObject   songCardPrefab;  // prefab with SongCard component
        public Button       backButton;

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
}
