using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RhythmGame
{
    // Attach to the note recap card prefab used inside ResultsScreen.
    public class NoteCard : MonoBehaviour
    {
        [Header("UI")]
        public TMP_Text noteNameText;  // "C4"
        public TMP_Text accuracyText;  // "8 / 10"
        public TMP_Text labelText;     // "Nailed it!" etc.
        public Image    background;

        [Header("Colours")]
        public Color colorGreat    = new Color(0.60f, 0.85f, 0.55f); // green
        public Color colorOkay     = new Color(0.98f, 0.78f, 0.33f); // amber
        public Color colorNeedWork = new Color(0.90f, 0.45f, 0.40f); // coral

        public void Setup(string noteName, int hits, int total)
        {
            noteNameText.text = noteName;
            accuracyText.text = total > 0 ? $"{hits} / {total}" : "–";

            float ratio = total > 0 ? (float)hits / total : 0f;

            if (ratio >= 0.8f)
            {
                labelText.text   = "Nailed it!";
                background.color = colorGreat;
            }
            else if (ratio >= 0.5f)
            {
                labelText.text   = "Almost!";
                background.color = colorOkay;
            }
            else
            {
                labelText.text   = "Keep practising";
                background.color = colorNeedWork;
            }
        }
    }
}
