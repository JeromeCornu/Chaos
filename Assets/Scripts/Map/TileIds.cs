using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    [CreateAssetMenu(fileName = "Tiles", menuName = "ScriptableObjects/Map/TileIDMap", order = 0)]
    public class TileIds : ScriptableObject
    {
        private Dictionary<int , TileBase> _dictionaryTiles = new Dictionary<int , TileBase>();
        
        [SerializeField] private List<TileIDPair> tiles;

        public void CreateDictionary()
        {
            _dictionaryTiles = tiles.ToDictionary(pair => pair.ID, pair => pair.Tile);
        }
        
        public TileBase GetTile(int ID)
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
        public TileBase Tile;
    }
}