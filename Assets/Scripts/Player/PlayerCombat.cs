using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Hitbox origin (place as if facing RIGHT)")]
    [SerializeField] Transform attackPoint;

    [Header("Hitbox (front-only)")]
    [SerializeField] Vector2 attackBox = new Vector2(1.0f, 0.8f); 
    [SerializeField] float backGuard = 0.05f;                      

    [Header("Combat")]
    [SerializeField] int attackDamage = 20;
    [SerializeField] float attackRate = 2f;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource attackSoundClip;

    float nextAttackTime;
    Rigidbody2D rb;
    SpriteRenderer sr;
    bool facingRight = true;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        if (!attackPoint)
        {
            var t = transform.Find("AttackPoint");
            if (t) attackPoint = t;
        }
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        UpdateFacing();

        if (Time.time >= nextAttackTime && Input.GetKeyDown(KeyCode.E))
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void UpdateFacing()
    {
       
        if (rb && Mathf.Abs(rb.linearVelocity.x) > 0.01f)
            facingRight = rb.linearVelocity.x > 0f;
        else if (sr) // 
            facingRight = !sr.flipX;
        else
            facingRight = transform.lossyScale.x >= 0f;

        // Keep the attackPoint mirrored in front if it's a direct child
        if (attackPoint && attackPoint.parent == transform)
        {
            var lp = attackPoint.localPosition;
            lp.x = Mathf.Abs(lp.x) * (facingRight ? 1 : -1);
            attackPoint.localPosition = lp;
        }
    }

    void Attack()
    {
        if (!animator) { Debug.LogError("PlayerCombat: Animator not assigned."); return; }
        if (!attackPoint) { Debug.LogError("PlayerCombat: AttackPoint not assigned."); return; }

        animator.SetTrigger("Attack");
        if (attackSoundClip) attackSoundClip.Play();

        // Build a front-only box:
        // center = attackPoint + forward * (halfWidth + backGuard)
        Vector2 forward = facingRight ? Vector2.right : Vector2.left;
        float halfW = attackBox.x * 0.5f;
        Vector3 center = attackPoint.position + (Vector3)forward * (halfW + backGuard);

        var hits = Physics2D.OverlapBoxAll(center, attackBox, 0f, enemyLayers);

        foreach (var col in hits)
        {
            // Angle gate: only keep colliders actually in front of the attackPoint
            Vector2 toTarget = (Vector2)col.bounds.ClosestPoint(attackPoint.position) - (Vector2)attackPoint.position;
            float dp = Vector2.Dot(toTarget.normalized, forward);
            if (dp <= 0.2f) continue; // behind/too sideways ? ignore

            col.GetComponentInParent<Enemy_Health>()?.TakeDamage(attackDamage);
            col.GetComponentInParent<Mushroom_Brute_Health>()?.TakeDamage(attackDamage);
            col.GetComponentInParent<Goblin_Minion_Health>()?.TakeDamage(attackDamage);
            col.GetComponentInParent<Skeleton_Warrior_Health>()?.TakeDamage(attackDamage);
            col.GetComponentInParent<BossHealth>()?.TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
                
        bool fr = facingRight;
        if (!Application.isPlaying)
        {
            var _sr = GetComponentInChildren<SpriteRenderer>();
            fr = _sr ? !_sr.flipX : (transform.lossyScale.x >= 0f);
        }

        Vector2 forward = fr ? Vector2.right : Vector2.left;
        float halfW = attackBox.x * 0.5f;
        Vector3 center = attackPoint.position + (Vector3)forward * (halfW + backGuard);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, new Vector3(attackBox.x, attackBox.y, 0f));
    }
}
