using Mirror;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Bullet : NetworkBehaviour
{
    private Rigidbody2D rb;
    private float lifetime;
    private int bouncesLeft;
    private float gravity;
    private float damage;

    public void Init(GunStats stats)
    {
        rb = GetComponent<Rigidbody2D>();
        lifetime = stats.bulletLifetime;
        bouncesLeft = stats.bulletBounces;
        gravity = stats.bulletGravity;
        damage = stats.bulletDamage;

        rb.gravityScale = gravity;
        rb.velocity = transform.right * stats.bulletSpeed;
    }

    void Update()
    {
        if (lifetime > 0f)
            lifetime -= Time.deltaTime;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        float margin = 0.1f;

        bool outOfScreen = viewPos.x < -margin || viewPos.x > 1f + margin || viewPos.y < -margin;
        bool expired = lifetime > 0f && lifetime <= 0f;

        if (outOfScreen || expired)
        {
            if (isServer) NetworkServer.Destroy(gameObject);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isServer)
        {
            Health health = collision.collider.GetComponentInParent<Health>();

            if (health != null)
            {
                health.TakeDamage((int)damage);
            }

            if (bouncesLeft > 0)
            {
                bouncesLeft--;
                // nothing to do (physics 2D handles it)
            }
            else
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }

}
