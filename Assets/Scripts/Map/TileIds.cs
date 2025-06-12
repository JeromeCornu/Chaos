using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    [CreateAssetMenu(fileName = "Tiles", menuName = "ScriptableObjects/Map/TileIDMap", order = 0)]
    public class TileIds : ScriptableObject
    {
        private Dictionary<int , TileCompatibility> _dictionaryTiles = new Dictionary<int , TileCompatibility>();
        
        [SerializeField] private List<TileIDPair> tiles;

        public void CreateDictionary()
        {
            _dictionaryTiles = tiles.ToDictionary(pair => pair.ID, pair => pair.Tile);
        }

        public Dictionary<int, TileCompatibility> GetDictionary()
        {
            return new Dictionary<int, TileCompatibility>(_dictionaryTiles);
        }
        
        public TileCompatibility GetTile(int ID)
        {
            return (from tileIDPair in tiles where tileIDPair.ID == ID select tileIDPair.Tile).FirstOrDefault();
        }

        public int GetTileCount()
        {
            return tiles.Count;
        }
    }


    [System.Serializable]
    public class TileIDPair
    {
        public int ID;
        public TileCompatibility Tile;
    }
}