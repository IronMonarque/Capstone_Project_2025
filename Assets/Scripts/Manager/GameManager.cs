using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool playerHasDied = false;

    public float restartDelay = 1f;
    public void PlayerDied()
    {
        if (playerHasDied == false)
        {
            playerHasDied = true;
            Debug.Log("YOU DIED");
            Invoke("Restart", restartDelay);
        }
        
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
