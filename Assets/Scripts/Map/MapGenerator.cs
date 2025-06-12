using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int mapSize; 
        [SerializeField] private List<GameObject> MapObjects;
        [SerializeField] private Tilemap map;
        [SerializeField] private TileBase tileBase;
        [SerializeField] private TileIds tileIds;
        [SerializeField] private MapTiles mapTiles;

        private Dictionary<int, MapCell> MapObjectDictionary;
    
        private List<List<MapCell>> Map;

        private void Start()
        {
            MapObjectDictionary = MapObjects.ToDictionary(o => o.GetInstanceID(), o => o.GetComponent<MapCell>());
            
            
            //TODO : REDO ALL OF THE SETUP WITH DE DICTIONARY
            tileIds.CreateDictionary();
            InitMapCells();
            GenerateMap();
        }

        private void InitMapCells()
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    Map[i][j] = new MapCell(tileIds.GetDictionary());
                }
            }
        }

        private void GenerateMap()
        {
        
        
            for (int i = 0; i < mapSize.y; i++)
            {
                for (int j = 0; j < mapSize.x; j++)
                {
                    map.SetTile(new Vector3Int(i,j), tileBase);
                }
            }
        
            // Vector2 randomCel = Random.insideUnitCircle * mapSize;
            //
            //
            // MapCell cell = Map[(int)randomCel.x][(int)randomCel.y];
            //
            // cell.SetRandomObject();

        }

        public void DrawMap()
        {
        
        
        
        }
    }
}
