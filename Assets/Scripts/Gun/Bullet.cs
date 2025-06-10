using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private Rigidbody2D rb;
    private float lifetime;
    private int bouncesLeft;
    private float gravity;
    private float damage;

    [HideInInspector]
    public bool isLocalVisualOnly = false;

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

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Ignore local duplicatas (client-side prediction)
        if (isLocalVisualOnly) return;

        var bullets = FindObjectsOfType<Bullet>();
        foreach (var b in bullets)
        {
            if (!b.isLocalVisualOnly) continue;

            // replace with better verification if needed (with ID, timestamps, etc.)
            float distance = Vector2.Distance(b.transform.position, transform.position);
            if (distance < 0.5f)
            {
                Destroy(b.gameObject); // delete local visuel doublon
                break;
            }
        }
    }

    void Update()
    {
        if (isLocalVisualOnly) return;

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            if (isServer)
                NetworkServer.Destroy(gameObject);
        }
        else
        {
            // avoid projectiles to go out of the screen limits (client)
            if (!isServer)
            {
                Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
                float margin = 0.1f;
                bool outOfScreen = viewPos.x < -margin || viewPos.x > 1f + margin || viewPos.y < -margin;

                if (outOfScreen)
                    Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLocalVisualOnly) return;

        if (!isServer) return;

        Health health = collision.collider.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage((int)damage);
        }

        if (bouncesLeft > 0)
        {
            bouncesLeft--;
            // physics 2D handles bounce
        }
        else
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
