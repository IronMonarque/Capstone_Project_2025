using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float attackRange = 1.4f;
    [SerializeField] float attackCooldown = 1.0f;
    [SerializeField] float windup = 0.12f; // delay so the hit lines up with the swing

    Boss boss;
    Rigidbody2D rb;
    Transform target;
    Boss_Attack damager;

    float nextAttackTime;
    float pendingAttackAt = -1f; // when to actually apply damage

    Transform FindPlayer()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        return go ? go.transform : null;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>() ?? animator.GetComponentInParent<Boss>();
        rb = animator.GetComponent<Rigidbody2D>() ?? animator.GetComponentInParent<Rigidbody2D>();
        damager = animator.GetComponent<Boss_Attack>() ?? animator.GetComponentInParent<Boss_Attack>();

        target = (boss && boss.player) ? boss.player : FindPlayer();

        nextAttackTime = 0f;
        pendingAttackAt = -1f;

        if (!rb) Debug.LogError("Boss_Run: Rigidbody2D not found.", animator);
        if (!damager) Debug.LogWarning("Boss_Run: Boss_Attack not found; boss will animate but not deal damage.", animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!rb) return;

        // If player is gone, stop and try to reacquire.
        if (!target)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            target = FindPlayer();
            return;
        }

        // Face the player (optional if Boss.LookAtPlayer is used elsewhere)
        var sr = (boss ? boss.GetComponentInChildren<SpriteRenderer>() : null);
        if (sr) sr.flipX = (target.position.x < rb.position.x);

        // Distance on X only (2D side scroller)
        float dist = Mathf.Abs(target.position.x - rb.position.x);

        // If we have a pending hit and it's time, deal damage once.
        if (pendingAttackAt > 0f && Time.time >= pendingAttackAt)
        {
            pendingAttackAt = -1f;
            damager?.Attack();
        }

        // In range to attack?
        if (dist <= attackRange && Time.time >= nextAttackTime)
        {
            // Stop horizontal movement while attacking
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // Fire animator trigger (make sure your controller has a Trigger named exactly "Attack")
            animator?.ResetTrigger("Attack");
            animator?.SetTrigger("Attack");

            // Schedule damage application after a tiny windup
            pendingAttackAt = Time.time + windup;
            nextAttackTime = Time.time + attackCooldown;
            return;
        }

        // Otherwise, chase toward the player on X
        if (dist > attackRange)
        {
            Vector2 from = rb.position;
            Vector2 to = new Vector2(target.position.x, from.y);
            Vector2 next = Vector2.MoveTowards(from, to, speed * Time.deltaTime);
            rb.MovePosition(next);
        }
    }
}
