using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Map
{
    public class MapCell
    {
        public bool IsDone;
        
        private int _tileID;
        private Dictionary<int, TileCompatibility> _possibleGameObjectIDs;
        private Vector2 _coord;
        private MapGenerator _mapGen;

        public MapCell(Dictionary<int, TileCompatibility> possibleGameObjectIDs, Vector2Int coord, MapGenerator mapGen)
        {
            _possibleGameObjectIDs = possibleGameObjectIDs;
            _coord= coord;
            _mapGen = mapGen;
        }

        public void SetRandomTile()
        {
            _tileID = (int)(Random.value * _possibleGameObjectIDs.Count);
            
            IsDone = true;
        }

        public void UpdateNeighbors()
        {
            foreach (Cardinals card in Enum.GetValues(typeof(Cardinals)))
            {
                UpdateNeighbor(card);
            }
        }

        private void UpdateNeighbor(Cardinals cardinal)
        {
            MapCell neighbor = GetNeighbor(cardinal);
            
            if (neighbor == null || neighbor.IsDone)
                return;
            
            List<String> validSocketsInCardinal = new List<String>();
            
            foreach (var kvp in _possibleGameObjectIDs)
            {
                validSocketsInCardinal.Add(kvp.Value.GetSocketID(cardinal));
            }

            for (var index = 0; index < neighbor._possibleGameObjectIDs.Count; index++)
            {
                // TODO : FIX THIS SHIT IT WAS FOREACH BUT CANT EDIT ARRAY IN FOREACH,
                // MIGRATED TO FOR WITH RIDER'S HELP AND IT FUCKED UP -_-
                
                var tileCompatibility = neighbor._possibleGameObjectIDs[index];
                if (!CheckForCompatibilityInList(validSocketsInCardinal, tileCompatibility.GetSocketID(cardinal.Opposite())))
                {
                    neighbor.DeletePossibility(index);
                }
            }
        }

        private bool CheckForCompatibilityInList(List<String> possibleSockets, String CheckedSocket)
        {
            return possibleSockets.Count(socket => CheckForSocketCompatibility(socket, CheckedSocket)) > 0;
        }
        
        
        private bool CheckForSocketCompatibility(String socketID, String checkedSocket)
        {
            return (checkedSocket == "1" && socketID == "1") ||
                   (checkedSocket == "0" && socketID == "0") ||
                   (checkedSocket + "s" == socketID) ||
                   (checkedSocket == socketID + "s");
        }
        
        private List<MapCell> GetNeighbors()
        {
            List<MapCell> neighbors = new List<MapCell>
            {
                GetNeighbor(Cardinals.North),
                GetNeighbor(Cardinals.East),
                GetNeighbor(Cardinals.South),
                GetNeighbor(Cardinals.West)
            };

            return neighbors;
        }

        private MapCell GetNeighbor(Cardinals cardinal)
        {
            switch (cardinal)
            {
                case Cardinals.North:
                    if (IsCellInBounds((int)_coord.x + 1, (int)_coord.y))
                        return _mapGen.GetMap()[(int)_coord.x + 1][(int)_coord.y];
                    break;
                case Cardinals.East:
                    
                    if (IsCellInBounds((int)_coord.x, (int)_coord.y + 1))
                        return _mapGen.GetMap()[(int)_coord.x][(int)_coord.y + 1];
                    
                    break;
                case Cardinals.South:
                    
                    if (IsCellInBounds((int)_coord.x - 1, (int)_coord.y))
                        return _mapGen.GetMap()[(int)_coord.x - 1][(int)_coord.y];
                    break;
                case Cardinals.West:
                    if (IsCellInBounds((int)_coord.x, (int)_coord.y - 1))
                        return _mapGen.GetMap()[(int)_coord.x][(int)_coord.y - 1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null);
                    
            }
            return null;
        }

        private bool IsCellInBounds(int X, int Y)
        {
            return X >= 0 && X <= _mapGen.GetMapSize().x && Y <= _mapGen.GetMapSize().y && Y >= 0;
        }

        private void DeletePossibility(int id)
        {
            _possibleGameObjectIDs.Remove(id);
        }

        public void SetCellAsTile(int id)
        {
            _tileID = id;
            
            IsDone = true;
        }

        public int GetTileID()
        {
            return _tileID;
        }

        public int GetEntropy()
        {
            return _possibleGameObjectIDs.Count;
        }
    }
}
