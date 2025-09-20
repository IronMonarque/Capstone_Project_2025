using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player death")]
    [SerializeField] float restartDelay = 1f;
    bool playerHasDied;

    [Header("Level clear")]
    [SerializeField] float nextLevelDelay = 1f;      // small pause before switching scenes
    [SerializeField] string nextSceneName = "";      // leave empty to use BuildIndex+1
    int enemiesRemaining;
    bool levelCleared;

    void Start()
    {
        // Count enemies at scene start.
        // Option A: by tag "Enemy"
        enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;

        // (Optional) If you prefer a component marker instead of tags:
        // enemiesRemaining = FindObjectsOfType<EnemyMarker>(true).Length;

        Debug.Log($"[GameManager] Enemies to defeat: {enemiesRemaining}");
    }

    // Call this from enemies when they die
    public void NotifyEnemyKilled()
    {
        enemiesRemaining = Mathf.Max(0, enemiesRemaining - 1);
        Debug.Log($"[GameManager] Enemy down. Remaining: {enemiesRemaining}");

        if (!levelCleared && !playerHasDied && enemiesRemaining == 0)
        {
            levelCleared = true;
            Invoke(nameof(LoadNextScene), nextLevelDelay);
        }
    }

    public void PlayerDied()
    {
        if (!playerHasDied)
        {
            playerHasDied = true;
            Debug.Log("YOU DIED");
            Invoke(nameof(Restart), restartDelay);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
