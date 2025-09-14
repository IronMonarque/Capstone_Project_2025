using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 150;
    int currentHealth;

    public Animator animator;
    public GameObject deathEffect;
    public Health_Bar healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        animator.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("isDead", true);

        Debug.Log("Player died");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject);

        FindObjectOfType<GameManager>().PlayerDied();
    }

    IEnumerator DamageAnimation()
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < 3; i++)
        {
            foreach (SpriteRenderer sr in srs)
            {
                Color c = sr.color;
                c.a = 0;
                sr.color = c;
            }

            yield return new WaitForSeconds(.1f);

            foreach (SpriteRenderer sr in srs)
            {
                Color c = sr.color;
                c.a = 1;
                sr.color = c;
            }

            yield return new WaitForSeconds(.1f);
        }
    }

}