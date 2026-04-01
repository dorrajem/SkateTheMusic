using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RhythmGame
{
    // Place alongside GameManager in the Boot scene.
    // Waits one frame for all Awake() calls to finish, then loads Main Menu.
    public class BootLoader : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return null; // let GameManager.Awake() run first
            SceneManager.LoadScene("MainMenu");
        }
    }
}
