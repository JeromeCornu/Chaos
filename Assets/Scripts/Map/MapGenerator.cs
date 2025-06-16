using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int mapSize; 
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileIds tileIds;
    
        private List<List<MapCell>> MapBackend;

        private static SynchronizationContext unityContext;

        [Button]
        private async void Start()
        {
            unityContext = SynchronizationContext.Current;
            tileIds.CreateDictionary();
            InitMapCells();
            await Task.Run(CreateMap);
        }

        private async void CreateMap()
        {
            Preset();
            
            try
            {
                await Task.Run(() => GenerateMap(MapBackend[0][0], mapSize.x * mapSize.y * 2));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

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
                    row.Add(new MapCell(tileIds.GetDictionary().Keys.ToList(), new(i,j), this));
                }
                MapBackend.Add(row);
            }
        }

        private void Preset()
        {
            // MapBackend[0][0].SetCellAsTile(1);
            //
            // UpdateNeighbors();
            //
            // var cell = FindCellLessEntropy();
            //
            // cell.SetRandomTile();
            //
            // Debug.Log("Chose : " + cell.GetTileID());
            //
            // UpdateNeighbors();

            // for (int i = 0; i < mapSize.y; i++)
            // {
            //     MapBackend[2][i].SetCellAsTile(4);
            // }

            // for (int i = 0; i < mapSize.x; i++)
            //     for (int j = 0; j < mapSize.y; j++)
            //         MapBackend[i][j].SetCellAsTile(4);

        }

        private void GenerateMap(MapCell cell, int depth)
        {
            if (depth <= 0 || IsDone()) return;
            
            cell.SetRandomTile();
            
            UpdateNeighbors();
            
            GenerateMap(FindCellLessEntropy(), depth - 1);
        }

        private void UpdateNeighbors()
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    MapBackend[i][j].UpdateNeighbors();
                }
            }
        }
        
        [Button]
        private void DrawMap()
        {
            unityContext.Post(_ => { DrawMapMainThread();}, null);
        }
        
        private void DrawMapMainThread()
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    if (MapBackend[i][j].IsDone)
                    {
                        tilemap.SetTile(new Vector3Int(j - mapSize.y / 2, i - mapSize.x / 2), tileIds.GetTile(MapBackend[i][j].GetTileID()));
                    }
                }
            }
        }

        private MapCell FindCellLessEntropy()
        {
            MapCell result = MapBackend[0][0];
            
            for (int i = 1; i < mapSize.x; i++)
                for (int j = 0; j < mapSize.y; j++)
                    result = MapBackend[i][j].GetEntropy() < result.GetEntropy() && !MapBackend[i][j].IsDone ? MapBackend[i][j] : result;
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

        private bool IsDone()
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    if (!MapBackend[i][j].IsDone){return false;}
                }
            }
            return true;
        }
    }
}
