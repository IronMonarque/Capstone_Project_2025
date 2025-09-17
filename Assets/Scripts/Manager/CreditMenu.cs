using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditMenu : MonoBehaviour
{
    // Set these in the Inspector to match your scene names (exactly).
    [SerializeField] private string mainMenuScene = "Main Menu";
    [SerializeField] private string restartScene = "Stage 1";

    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;                 // in case you paused earlier
        SceneManager.LoadScene(mainMenuScene);
    }

    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(restartScene);
    }
}
