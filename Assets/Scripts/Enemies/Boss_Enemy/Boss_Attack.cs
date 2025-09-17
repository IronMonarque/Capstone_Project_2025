using UnityEngine;

public class Boss_Attack : MonoBehaviour
{
    public int attackDamage = 40;
    
    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    [SerializeField] private AudioSource bossAttackSoundClip;


    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        bossAttackSoundClip.Play();

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }
}
