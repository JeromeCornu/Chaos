using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map.MapObstacles
{
    [CreateAssetMenu(fileName = "Obstacle", menuName = "ScriptableObjects/Obstacle/ObstacleIDs", order = 2)]
    public class ObstacleIDs : ScriptableObject
    {
        private static Dictionary<int , Obstacle> _dictionaryTiles = new Dictionary<int , Obstacle>();
        
        [SerializeField] private List<Obstacle> tiles;

        public void CreateDictionary()
        {
            _dictionaryTiles = tiles.Select((obj, index) => new { index, obj }).ToDictionary(x => x.index ,x => x.obj);
        }

        public Dictionary<int, Obstacle> GetDictionary()
        {
            return new Dictionary<int, Obstacle>(_dictionaryTiles);
        }

        public Obstacle GetObstacle(int id)
        {
            return _dictionaryTiles[id];
        }
    }
}