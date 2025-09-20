using UnityEngine;

public class Skeleton_Warrior_Health : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource isHurtSoundClip;
    [SerializeField] AudioSource isDeadSoundClip;
    [SerializeField] public int maxHealth = 50;

    // If some controllers use different names, set them per-prefab in Inspector
    [SerializeField] string hurtParam = "isHurt";
    [SerializeField] string deadParam = "isDead";

    int currentHealth;
    bool reported;

    // cache hashes
    int hurtHash, deadHash;

    void Awake()
    {
        hurtHash = Animator.StringToHash(hurtParam);
        deadHash = Animator.StringToHash(deadParam);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;

        if (animator && HasParam(animator, hurtHash)) animator.SetTrigger(hurtHash);
        isHurtSoundClip?.Play();

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        NotifyOnce();

        if (animator && HasParam(animator, deadHash)) animator.SetBool(deadHash, true);
        isDeadSoundClip?.Play();

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        this.enabled = false;

        Destroy(gameObject, 0.05f);
    }

    void OnDestroy()
    {
        if (Application.isPlaying) NotifyOnce();
    }

    void NotifyOnce()
    {
        if (reported) return;
        var gm = FindObjectOfType<GameManager>();
        if (gm) gm.NotifyEnemyKilled();
        reported = true;
    }

    static bool HasParam(Animator a, int nameHash)
    {
        if (!a) return false;
        foreach (var p in a.parameters)
            if (p.nameHash == nameHash) return true;
        return false;
    }

    void Start() => currentHealth = maxHealth;
}
