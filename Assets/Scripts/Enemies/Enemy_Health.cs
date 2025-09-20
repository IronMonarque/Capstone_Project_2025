using UnityEngine;

public class Enemy_Health : MonoBehaviour, IDamageable
{
    [SerializeField] public int maxHealth = 50;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource isHurtSoundClip;
    [SerializeField] private AudioSource isDeadSoundClip;

    int currentHealth;
    bool reported; // prevents double count

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (animator) animator.SetTrigger("isHurt");
        isHurtSoundClip?.Play();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        // --- Notify FIRST so nothing can stop it ---
        NotifyOnce();

        // Visuals / audio (null-safe)
        if (animator) animator.SetBool("isDead", true);
        isDeadSoundClip?.Play();

        // Disable collision/logic, then destroy
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        this.enabled = false;

        // Small delay if you want death anim/SFX to start before removing
        Destroy(gameObject, 0.05f);
    }

    void OnDestroy()
    {
        // Safety: if something else destroys this enemy, still notify
        if (Application.isPlaying) NotifyOnce();
    }

    void NotifyOnce()
    {
        if (reported) return;
#if UNITY_2022_2_OR_NEWER
        var gm = FindFirstObjectByType<GameManager>();
#else
        var gm = FindObjectOfType<GameManager>();
#endif
        if (gm) gm.NotifyEnemyKilled();
        reported = true;
    }
}
