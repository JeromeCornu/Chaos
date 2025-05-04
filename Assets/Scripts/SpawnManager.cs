using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // To change if more than 2 players
    public Vector2 GetSpawnPosition(int playerIndex)
    {
        if (playerIndex == 0)
            return spawnPoint1.position;
        else
            return spawnPoint2.position;
    }
}
