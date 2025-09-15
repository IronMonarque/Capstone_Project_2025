using UnityEngine;

public class Enemy_Health : MonoBehaviour, IDamageable
{
    public Animator animator;

    public int maxHealth = 50;
    int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("isHurt");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        
        Debug.Log("Enemy died");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        
        // ... your VFX/SFX/points here ...
        var gm = FindObjectOfType<GameManager>();
        if (gm) gm.NotifyEnemyKilled();   // <-- this line is the key

        Destroy(gameObject);
    }
}
