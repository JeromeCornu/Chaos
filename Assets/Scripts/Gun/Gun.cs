using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Mirror;

public class Gun : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject bulletUIPrefab;
    public GunStats stats = new GunStats();
    private Coroutine reloadCoroutine;

    private int currentAmmo;
    private float fireCooldown;
    private bool isReloading = false;
    public float autoReloadDelay = 2f;
    private float timeSinceLastShot = 0f;

    // UI elements
    public Image reloadCircle;
    public GameObject bulletsDisplay; // container holding all bullet icons
    private List<GameObject> bulletIcons = new List<GameObject>();

    private void Start()
    {
        currentAmmo = stats.magazineSize;

        // Instantiate bullet icons into the container
        for (int i = 0; i < stats.magazineSize; i++)
        {
            GameObject icon = Instantiate(bulletUIPrefab, bulletsDisplay.transform);
            bulletIcons.Add(icon);
        }

        // Initialize UI state
        reloadCircle.fillAmount = 0f;
        reloadCircle.gameObject.SetActive(false);
        bulletsDisplay.SetActive(true);

        UpdateAmmoDisplay(currentAmmo);
    }

    private void Update()
    {
        if (isReloading) return;

        fireCooldown -= Time.deltaTime;
        timeSinceLastShot += Time.deltaTime;

        // Trigger auto-reload after delay if not at full ammo
        if (currentAmmo < stats.magazineSize && timeSinceLastShot >= autoReloadDelay)
        {
            reloadCoroutine = StartCoroutine(Reload());
            Debug.Log("Auto reload triggered");
        }
    }

    public bool TryFireLocal(out Vector3 pos, out Quaternion rot)
    {
        pos = firePoint.position;
        float spread = Random.Range(-stats.spreadAngle, stats.spreadAngle);
        rot = firePoint.rotation * Quaternion.Euler(0, 0, spread);

        if (currentAmmo <= 0 || fireCooldown > 0f)
            return false;

        fireCooldown = stats.fireRate;
        currentAmmo--;
        timeSinceLastShot = 0f;

        // If reload is in progress, cancel it
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;

            // Only hide the reload UI if it was visible
            if (reloadCircle.gameObject.activeSelf)
            {
                reloadCircle.gameObject.SetActive(false);
                bulletsDisplay.SetActive(true);
            }

            Debug.Log("Reload cancelled due to firing");
        }

        UpdateAmmoDisplay(currentAmmo);
        return true;
    }

    public void ShootBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Init(stats);
        NetworkServer.Spawn(bullet);
    }

    public IEnumerator Reload()
    {
        isReloading = true;

        // Only show the reload UI if ammo is empty
        bool showReloadUI = currentAmmo == 0;

        if (showReloadUI)
        {
            bulletsDisplay.SetActive(false);
            reloadCircle.fillAmount = 0f;
            reloadCircle.gameObject.SetActive(true);
        }

        float elapsed = 0f;
        float reloadTime = stats.reloadTime;

        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;

            if (showReloadUI)
                reloadCircle.fillAmount = Mathf.Clamp01(elapsed / reloadTime);

            yield return null;
        }

        currentAmmo = stats.magazineSize;
        isReloading = false;
        reloadCoroutine = null;

        if (showReloadUI)
        {
            reloadCircle.gameObject.SetActive(false);
            bulletsDisplay.SetActive(true);
        }

        UpdateAmmoDisplay(currentAmmo);
        Debug.Log("Gun reloaded!");
    }

    public int GetCurrentAmmo() => currentAmmo;

    public void UpdateAmmoDisplay(int ammo)
    {
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            bulletIcons[i].SetActive(i < ammo);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null || stats == null) return;

        int points = 30;
        float step = stats.bulletLifetime / points;
        float gravity = stats.bulletGravity * Physics2D.gravity.y;

        for (int i = 0; i < stats.bulletsPerShot; i++)
        {
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
