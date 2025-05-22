public enum StatType
{
    bulletsPerShot,
    magazineSize,
    fireRate,
    reloadTime,
    bulletSpeed,
    bulletLifetime,
    bulletGravity,
    bulletBounces,
    spreadAngle,
    bulletDamage,
    health,
    speed,
    jumpForce
}

[System.Serializable]
public class StatModifier
{
    public StatType stat;
    public float value;
}
