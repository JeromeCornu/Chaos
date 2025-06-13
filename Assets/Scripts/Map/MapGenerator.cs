using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int mapSize; 
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileIds tileIds;
    
        private List<List<MapCell>> MapBackend;

        private void Start()
        {
            tileIds.CreateDictionary();
            InitMapCells();
            GenerateMap();
            DrawMap();
        }

        private void InitMapCells()
        {
            MapBackend = new List<List<MapCell>>();
            
            for (int i = 0; i < mapSize.x; i++)
            {
                List<MapCell> row = new List<MapCell>();
                for (int j = 0; j < mapSize.y; j++)
                {
                    row.Add(new MapCell(tileIds.GetDictionary(), new(i,j), this));
                }
                MapBackend.Add(row);
            }
            
            MapBackend[0][0].SetRandomTile();
        }

        private void GenerateMap()
        {
            int count = 0;
            
            for (int i = 1; i < mapSize.x; i++)
                for (int j = 0; j < mapSize.y; j++)
                    MapBackend[i][j].UpdateNeighbors();

            MapCell nextCell = FindCellLessEntropy();
            nextCell.SetRandomTile();
            count++;
            
            if (count >= mapSize.x * mapSize.y)
                return;
            
            GenerateMap();
        }

        private void DrawMap()
        {
            for (int i = 0; i < mapSize.y; i++)
            {
                for (int j = 0; j < mapSize.x; j++)
                {
                    tilemap.SetTile(new Vector3Int(i - i/2, j - j/2), tileIds.GetTile(MapBackend[i][j].GetTileID()));
                }
            }
        }

        private MapCell FindCellLessEntropy()
        {
            MapCell result = MapBackend[0][0];
            
            for (int i = 1; i < mapSize.x; i++)
                for (int j = 0; j < mapSize.y; j++)
                    result = MapBackend[i][j].GetEntropy() < result.GetEntropy() ? MapBackend[i][j] : result;
            return result;
        }

        public List<List<MapCell>> GetMap()
        {
            return MapBackend;
        }

        public Vector2Int GetMapSize()
        {
            return mapSize;
        }
    }
}
