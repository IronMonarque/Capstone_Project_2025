using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;                 // leave empty: auto-finds by tag
    [SerializeField] SpriteRenderer sr;      // optional; auto-grabbed if not set

    void Awake()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void LookAtPlayer()
    {
        if (!player) return;                 // player destroyed or not assigned

        bool faceRight = player.position.x > transform.position.x;

        // Prefer SpriteRenderer.flipX for 2D
        if (sr) sr.flipX = !faceRight;
        else
        {
            // fallback: flip by scale X only (no rotate)
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (faceRight ? 1f : -1f);
            transform.localScale = s;
        }
    }
}
