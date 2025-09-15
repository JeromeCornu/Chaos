using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map.MapObstacles
{
    [CreateAssetMenu(fileName = "Obstacle", menuName = "ScriptableObjects/Obstacle/Obstacle", order = 2)]
    public class Obstacle : ScriptableObject
    {
        [SerializeField] private string obstacleName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private TileBase obstacleTile;
        [SerializeField] private bool damagesPlayer;

        public TileBase ObstacleTile => obstacleTile;
        public bool DamagesPlayer => damagesPlayer;
        public Sprite Sprite => sprite;

        public string ObstacleName => obstacleName;
    }
}