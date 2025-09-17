using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private AudioSource attackSoundClip;


    float nextAttackTime;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        if (!attackPoint)
        {
            var t = transform.Find("AttackPoint");
            if (t) attackPoint = t;
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetKeyDown(KeyCode.E))
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void Attack()
    {
        if (!animator) { Debug.LogError("PlayerCombat: Animator not assigned."); return; }
        if (!attackPoint) { Debug.LogError("PlayerCombat: AttackPoint not assigned."); return; }
        

        animator.SetTrigger("Attack");
        attackSoundClip.Play();

        var hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (var col in hits)
        {
            // Many enemies put colliders on child objects — use InParent to be safe.
            var skel = col.GetComponentInParent<Enemy_Health>();
            if (skel != null) skel.TakeDamage(attackDamage);

            var boss = col.GetComponentInParent<BossHealth>();
            if (boss != null) boss.TakeDamage(attackDamage);

            // If neither component exists, do nothing (no NullRef!)
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
