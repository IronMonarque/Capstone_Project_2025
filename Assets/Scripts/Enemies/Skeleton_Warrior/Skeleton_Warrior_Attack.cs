using UnityEngine;

public class Skeleton_Warrior_Attack : MonoBehaviour
{
    [Header("References")]
    public Transform player;            // leave empty to auto-find by tag "Player"
    public Transform attackPoint;       // child placed in front of the weapon
    public LayerMask attackMask;        // MUST include the Player layer
    public Animator animator;           // optional

    [Header("Stats")]
    public int attackDamage = 20;
    public float moveSpeed = 2.5f;
    public float chaseRadius = 6f;      // start/keep chasing within this
    public float attackRange = 1f;      // must be <= chaseRadius
    public float attackCooldown = 0.8f; // seconds between hits
    public float windup = 0.15f;        // delay to sync with swing

    float _nextAttackTime;
    Rigidbody2D _rb;
    SpriteRenderer _sr;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();                
        _sr = GetComponentInChildren<SpriteRenderer>();

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        if (!animator) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!player) return;

        LookAtPlayer(); // uses 2D-safe flip

        float dist = Vector2.Distance(transform.position, player.position);

        // CHASE
        if (dist > attackRange && dist <= chaseRadius)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            Vector2 target = (Vector2)transform.position + dir * moveSpeed * Time.deltaTime;

            if (_rb) _rb.MovePosition(target);
            else transform.position = target;

            if (animator) animator.SetFloat("velocityX", moveSpeed);
        }
        else
        {
            if (animator) animator.SetFloat("velocityX", 0f);
        }

        // ATTACK
        if (dist <= attackRange && Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + attackCooldown;
            if (animator) animator.SetTrigger("Attack1"); // optional animation
            Invoke(nameof(DoAttack), windup);            // small delay to match the swing
        }
    }

    // Kept for compatibility if you call Attack() from an Animation Event
    public void Attack()
    {
        DoAttack();
    }

    void DoAttack()
    {
        if (!attackPoint)
        {
            Debug.LogWarning($"{name}: Assign attackPoint in the Inspector.");
            return;
        }

        // Slightly smaller than attackRange so it feels fair
        var hit = Physics2D.OverlapCircle(attackPoint.position, attackRange * 0.6f, attackMask);
        if (hit)
        {
            var hp = hit.GetComponentInParent<PlayerHealth>();
            if (hp) hp.TakeDamage(attackDamage);
        }
    }

    // Replaces your old rotate+scale flip (2D-safe)
    public void LookAtPlayer()
    {
        if (!player) return;
        bool faceRight = player.position.x > transform.position.x;

        if (_sr) _sr.flipX = !faceRight;  // flip sprite only
        else
        {
            // fallback: scale flip if no SpriteRenderer
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (faceRight ? 1f : -1f);
            transform.localScale = s;
        }

        // keep attackPoint ahead of the skeleton
        if (attackPoint && attackPoint.parent == transform)
        {
            var lp = attackPoint.localPosition;
            lp.x = Mathf.Abs(lp.x) * (faceRight ? 1 : -1);
            attackPoint.localPosition = lp;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange * 0.6f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
