using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{

    public int maxHealth = 250;
    int currentHealth;

    public Animator animator;
    public GameObject deathEffect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("isDead", true);

        Debug.Log("Boss died");
        
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }

}