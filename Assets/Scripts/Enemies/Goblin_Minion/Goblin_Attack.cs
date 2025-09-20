using UnityEngine;

public class Goblin_Attack : MonoBehaviour
{
    [SerializeField] Transform attackPoint;     // child in front
    [SerializeField] Vector2 attackBox = new(1.0f, 0.6f);
    [SerializeField] LayerMask playerMask;      // ONLY Player layer
    [SerializeField] int damage = 15;

    bool canDamage; // opened/closed by animation events

    // Animation Event at the impact frame
    public void AE_OpenHit() { canDamage = true; DoHit(); }
    // Animation Event a few frames later
    public void AE_CloseHit() { canDamage = false; }

    public void Attack() => DoHit();

    void DoHit()
    {
        if (!canDamage || !attackPoint) return;

        // box centered slightly in front so nothing behind is hit
        float halfW = attackBox.x * 0.5f;
        Vector2 fwd = transform.lossyScale.x >= 0 ? Vector2.right : Vector2.left;
        Vector3 center = attackPoint.position + (Vector3)fwd * (halfW + 0.05f);

        var hits = Physics2D.OverlapBoxAll(center, attackBox, 0f, playerMask);
        foreach (var col in hits)
            col.GetComponentInParent<PlayerHealth>()?.TakeDamage(damage);
    }

    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        float halfW = attackBox.x * 0.5f;
        Vector2 fwd = transform.lossyScale.x >= 0 ? Vector2.right : Vector2.left;
        Vector3 center = attackPoint.position + (Vector3)fwd * (halfW + 0.05f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, attackBox);
    }
}
