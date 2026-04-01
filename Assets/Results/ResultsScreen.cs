using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RhythmGame
{
    // Reads ResultsPayload (written by ScoreManager) and drives the results UI.
    // Wire all references in the Inspector.
    public class ResultsScreen : MonoBehaviour
    {
        [Header("Score")]
        public TMP_Text songTitleText;
        public TMP_Text scoreText;
        public TMP_Text bestComboText;

        [Header("Stars")]
        public Image[]  starImages;    // 3 Image components
        public Sprite   starFilled;
        public Sprite   starEmpty;

        [Header("Note recap")]
        public Transform  recapContainer;    // horizontal/vertical layout group
        public GameObject noteCardPrefab;    // prefab with NoteCard component

        [Header("Educational tip")]
        public TMP_Text tipText;

        [Header("Buttons")]
        public Button retryButton;
        public Button songSelectButton;

        // ── Lifecycle ───────────────────────────────────────────────────────

        void Start()
        {
            SongData song = ResultsPayload.SongData;

            if (song == null)
            {
                Debug.LogWarning("[ResultsScreen] No payload — returning to menu.");
                GameManager.Instance.GoToMenu();
                return;
            }

            // Populate text
            songTitleText.text  = song.displayTitle;
            scoreText    .text  = ResultsPayload.FinalScore.ToString("N0");
            bestComboText.text  = $"Best combo: {ResultsPayload.BestCombo}x";
            tipText      .text  = song.educationalTip;

            // Stars
            int stars = CalcStars(ResultsPayload.FinalScore, song);
            StartCoroutine(AnimateStars(stars));

            // Note recap cards
            BuildRecap(ResultsPayload.NoteResults, song);

            // Buttons
            retryButton      .onClick.AddListener(() => GameManager.Instance.RestartCurrentSong());
            songSelectButton .onClick.AddListener(() => GameManager.Instance.GoToSongSelect());
        }

        // ── Stars ───────────────────────────────────────────────────────────

        int CalcStars(int score, SongData song)
        {
            if (score >= song.starThreshold3) return 3;
            if (score >= song.starThreshold2) return 2;
            if (score >= song.starThreshold1) return 1;
            return 0;
        }

        IEnumerator AnimateStars(int earned)
        {
            // First set all to empty
            foreach (Image img in starImages) img.sprite = starEmpty;

            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < starImages.Length; i++)
            {
                if (i < earned)
                {
                    starImages[i].sprite = starFilled;
                    // Simple pop scale animation
                    yield return StartCoroutine(PopScale(starImages[i].transform));
                }
                yield return new WaitForSeconds(0.15f);
            }
        }

        IEnumerator PopScale(Transform t)
        {
            float elapsed = 0f;
            while (elapsed < 0.25f)
            {
                elapsed += Time.deltaTime;
                float f  = elapsed / 0.25f;
                // Overshoot then settle
                float s  = f < 0.6f ? Mathf.Lerp(0f, 1.25f, f / 0.6f)
                                    : Mathf.Lerp(1.25f, 1f, (f - 0.6f) / 0.4f);
                t.localScale = Vector3.one * s;
                yield return null;
            }
            t.localScale = Vector3.one;
        }

        // ── Note recap ──────────────────────────────────────────────────────

        void BuildRecap(List<NoteResult> results, SongData song)
        {
            if (song.notesIntroduced == null || song.notesIntroduced.Length == 0) return;

            // Count hits and total appearances per note name
            var hits  = new Dictionary<string, int>();
            var total = new Dictionary<string, int>();

            foreach (NoteResult r in results)
            {
                if (!total.ContainsKey(r.noteName)) { total[r.noteName] = 0; hits[r.noteName] = 0; }
                total[r.noteName]++;
                if (r.wasHit) hits[r.noteName]++;
            }

            // One card per note introduced in this song
            foreach (string note in song.notesIntroduced)
            {
                GameObject go   = Instantiate(noteCardPrefab, recapContainer);
                NoteCard   card = go.GetComponent<NoteCard>();
                int h = hits .ContainsKey(note) ? hits [note] : 0;
                int t = total.ContainsKey(note) ? total[note] : 0;
                card.Setup(note, h, t);
            }
        }
    }
}
