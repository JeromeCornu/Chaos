using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.UI;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Utils;
using Random = UnityEngine.Random;

namespace Map.MapObstacles
{
    public class MapObstacleManager : NetworkBehaviour
    {
        public Inputs Input; 
        public Camera cam;
        
        private GameObject cardGrid;

        [SerializeField] private ObstacleIDs obstaclesIDs;
        [SerializeField] private GameObject obstacleUICardPrefab;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tilemap tilemapDamage;
        [SerializeField] private Tilemap tilemapVisual;
        
        private void OnEnable()
        {
            Input = new Inputs();
            Input.Enable();
        }
        
        private void Start()
        {
            obstaclesIDs.CreateDictionary();
            cam = Camera.main;
            cardGrid = UIManager.Instance.GetCardGrid();
            
            List<int> selectedObstaclesIDs = obstaclesIDs.GetDictionary().Keys.ToList().GetRandomDistinctCount(2);

            foreach (Transform child in cardGrid.transform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var obstacleID in selectedObstaclesIDs)
            {
                Obstacle obs = obstaclesIDs.GetObstacle(obstacleID);
                
                Instantiate(obstacleUICardPrefab, cardGrid.transform).TryGetComponent( out ObstacleCard card);
                card.Init(obstacleID, obs.ObstacleName, obs.Sprite, this);
            }
        }

        [Command(requiresAuthority = false)]
        public void PlaceObstacleToServer(int obstacleID, Vector3 position, bool isVisual )
        {
            PlaceObstacleRPC(obstacleID, position, isVisual);
        }

        private void PlaceObstacleRPC(int obstacleID, Vector3 position, bool isVisual)
        {
            Obstacle obs = obstaclesIDs.GetObstacle(obstacleID);
            
            Tilemap map = isVisual ? tilemapVisual : obs.DamagesPlayer ? tilemapDamage : tilemap;
            
            map.SetTile(map.WorldToCell(position), obs.ObstacleTile);
        }

        public void ClearVisualMap()
        {
            tilemapVisual.ClearAllTiles();
        }

        public void SetActiveVisualTileMap(bool active)
        {
            tilemapVisual.gameObject.SetActive(active);
        }
        
        private void OnDisable()
        {
            Input.Disable();
        }
    }
}