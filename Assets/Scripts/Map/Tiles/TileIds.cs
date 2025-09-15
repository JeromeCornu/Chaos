using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    [CreateAssetMenu(fileName = "Tiles", menuName = "ScriptableObjects/Map/TileIDMap", order = 0)]
    public class TileIds : ScriptableObject
    {
        private static Dictionary<int , TileCompatibility> _dictionaryTiles = new Dictionary<int , TileCompatibility>();
        
        [SerializeField] private List<TileCompatibility> tiles;

        public void CreateDictionary()
        {
            _dictionaryTiles = tiles.Select((obj, index) => new { index, obj }).ToDictionary(x => x.index ,x => x.obj);
        }

        public Dictionary<int, TileCompatibility> GetDictionary()
        {
            return new Dictionary<int, TileCompatibility>(_dictionaryTiles);
        }

        public static TileCompatibility GetTileCompatibility(int id)
        {
            return _dictionaryTiles[id];
        }
        
        public TileBase GetTile(int ID)
        {
            return _dictionaryTiles[ID].Tile;
        }
    }
}