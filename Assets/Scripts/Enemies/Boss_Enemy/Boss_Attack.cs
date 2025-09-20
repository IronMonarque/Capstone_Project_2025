using UnityEngine;

public class Boss_Attack : MonoBehaviour
{
    public int attackDamage = 40;

    // Place this child in front of the boss *when facing RIGHT*
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 1f;
    [SerializeField] LayerMask attackMask;          // must include Player layer
    [SerializeField] AudioSource bossAttackSoundClip;

    // Call from animation event or AI when the swing lands
    public void Attack()
    {
        // Safe audio
        bossAttackSoundClip?.Play();

        // Fallback if no attackPoint assigned: use current position + your offset
        Vector3 center;
        if (attackPoint)
        {
            center = attackPoint.position;
        }
        else
        {
            Vector3 pos = transform.position;
            pos += transform.right * attackOffset.x;
            pos += transform.up * attackOffset.y;
            center = pos;
        }

        // Do the hit
        Collider2D hit = Physics2D.OverlapCircle(center, attackRange, attackMask);
        if (hit)
        {
            // Use InParent in case the collider is on a child and health is on the root
            var hp = hit.GetComponentInParent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(attackDamage);
        }
    }

    // Keep your existing offset if you like (used only when attackPoint is null)
    public Vector3 attackOffset;

    // Debug: shows the hit circle in Scene view
    void OnDrawGizmosSelected()
    {
        Vector3 center = attackPoint ? attackPoint.position
                                     : transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
    }
}
