using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Map;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize; 
    [SerializeField] private List<GameObject> MapObjects;

    private Dictionary<int, MapCell> MapObjectDictionary;
    
    private List<List<MapCell>> Map;

    private void Start()
    {
        MapObjectDictionary = MapObjects.ToDictionary(o => o.GetInstanceID(), o => o.GetComponent<MapCell>());
        InitMapCells();
    }

    private void InitMapCells()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Map[i][j] = new MapCell(MapObjectDictionary.Keys.ToList());
            }
        }
    }

    public void GenerateMap()
    { 
        Vector2 randomCel = Random.insideUnitCircle * mapSize;


        MapCell cell = Map[(int)randomCel.x][(int)randomCel.y];
        
        cell.SetRandomObject();

    }

    public void DrawMap()
    {
        
        
        
    }
}
