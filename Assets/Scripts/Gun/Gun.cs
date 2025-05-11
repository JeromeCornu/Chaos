#if UNITY_EDITOR
using Mirror;
using System.Collections;
using UnityEditor;
#endif
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GunStats stats = new GunStats();

    private int currentAmmo;
    private float fireCooldown;
    private bool isReloading = false;
    public float autoReloadDelay = 2f;
    private float timeSinceLastShot = 0f;


    void Start()
    {
        currentAmmo = stats.magazineSize;
    }

    void Update()
    {
        if (isReloading) return;

        fireCooldown -= Time.deltaTime;
        timeSinceLastShot += Time.deltaTime;

        if (currentAmmo < stats.magazineSize && timeSinceLastShot >= autoReloadDelay)
        {
            StartCoroutine(Reload());
            Debug.Log("Auto reload triggered");
        }
    }


    public bool TryFireLocal(out Vector3 pos, out Quaternion rot)
    {
        pos = firePoint.position;
        float spread = Random.Range(-stats.spreadAngle, stats.spreadAngle);
        rot = firePoint.rotation * Quaternion.Euler(0, 0, spread);

        if (currentAmmo <= 0 || isReloading || fireCooldown > 0f)
            return false;

        fireCooldown = stats.fireRate;
        currentAmmo--;

        timeSinceLastShot = 0f; // reset timer

        return true;
    }


    public void ShootBullet(Vector3 position, Quaternion rotation)
    {
        Debug.Log("ShootBullet called at position: " + position);

        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Init(stats);
        NetworkServer.Spawn(bullet);
    }


    public void StartReload()
    {
        if (!isReloading)
            StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(stats.reloadTime);
        Debug.Log("gun reloaded!");
        currentAmmo = stats.magazineSize;
        isReloading = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null || stats == null) return;

        int points = 30;
        float step = stats.bulletLifetime / points;
        float gravity = stats.bulletGravity * Physics2D.gravity.y;

        for (int i = 0; i < stats.bulletsPerShot; i++)
        {
            // random spread
            float spread = Random.Range(-stats.spreadAngle, stats.spreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, spread);
            Vector3 direction = spreadRotation * firePoint.right;

            Vector3 pos = firePoint.position;
            Vector3 velocity = direction * stats.bulletSpeed;

            Gizmos.color = Color.cyan;

            for (int j = 0; j < points; j++)
            {
                Vector3 nextPos = pos + velocity * step + 0.5f * Vector3.up * gravity * step * step;
                velocity += Vector3.up * gravity * step;

                Gizmos.DrawLine(pos, nextPos);
                pos = nextPos;
            }
        }
    }

}
