using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Map
{
    public class MapCell
    {
        public bool IsDone;
        public Vector2 Coord {get;}

        private int _tileID;
        private List<int> _possibleTileIDs;
        private MapGenerator _mapGen;

        public MapCell(List<int> possibleGameObjectIDs, Vector2Int coord, MapGenerator mapGen)
        {
            _possibleTileIDs = possibleGameObjectIDs;
            Coord= coord;
            _mapGen = mapGen;
        }
        
        public int GetTileID()
        {
            return _tileID;
        }

        public int GetEntropy()
        {
            return _possibleTileIDs.Count;
        }

        public void SetRandomTile()
        {
            if (IsDone) return;
            
            Random rand = new Random();
            
            _tileID = _possibleTileIDs[rand.Next(_possibleTileIDs.Count)];
            
            IsDone = true;
        }
        
        public void SetCellAsTile(int id)
        {
            _tileID = id;
            IsDone = true;
        }

        public void UpdateNeighbors()
        {
            foreach (Cardinals card in Enum.GetValues(typeof(Cardinals)))
            {
                UpdateNeighbor(card);
            }
        }

        public void UpdateNeighbor(Cardinals cardinal)
        {
            MapCell neighbor = GetNeighbor(cardinal);
            List<int> toRemove = new List<int>();
            
            if (neighbor == null || neighbor.IsDone)
                return;

            if (IsDone)
            {
                try
                {
                    for (var index = 0; index < neighbor._possibleTileIDs.Count; index++)
                    {
                        var tileID = neighbor._possibleTileIDs[index];

                        String cardinalSocket = TileIds.GetTileCompatibility(_tileID).GetSocketID(cardinal);
                        String otherSocket = TileIds.GetTileCompatibility(tileID).GetSocketID(cardinal.Opposite());
                        bool isCompatible = CheckForSocketCompatibility(
                            cardinalSocket,
                            otherSocket);
                    
                        if (!isCompatible)
                        {
                            toRemove.Add(tileID);
                        }
                    }
                    neighbor._possibleTileIDs.RemoveAll(i => toRemove.Contains(i));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                List<String> validSocketsForCardinal = new List<String>();
                
                foreach (var tileID in _possibleTileIDs)
                {
                    validSocketsForCardinal.Add(TileIds.GetTileCompatibility(tileID).GetSocketID(cardinal));
                }
                
                for (var index = 0; index < neighbor._possibleTileIDs.Count; index++)
                {
                    var tileID = neighbor._possibleTileIDs[index];
                    if (!CheckForCompatibilityInList(validSocketsForCardinal, TileIds.GetTileCompatibility(tileID).GetSocketID(cardinal.Opposite())))
                    {
                        toRemove.Add(tileID);
                    }
                }
                
            }

            if (toRemove.Count == _possibleTileIDs.Count)
            {
                Debug.Log($"neighbor.Coord = {neighbor.Coord}, tile.Coord = {Coord}");
            }
            
            neighbor._possibleTileIDs.RemoveAll(i => toRemove.Contains(i));
        }

        private bool CheckForCompatibilityInList(List<String> possibleSockets, String checkedSocket)
        {
            return possibleSockets.Count(socket => CheckForSocketCompatibility(socket, checkedSocket)) > 0;
        }

        private bool CheckForSocketCompatibility(String socketID, String checkedSocket)
        {
            return (checkedSocket == "-1" && socketID == "-1") ||
                   (checkedSocket == "0" && socketID == "0") ||
                   (checkedSocket + "s" == socketID) ||
                   (checkedSocket == socketID + "s");
        }

        private MapCell GetNeighbor(Cardinals cardinal)
        {
            switch (cardinal)
            {
                case Cardinals.North:
                    if (IsCellInBounds((int)Coord.x + 1, (int)Coord.y))
                        return _mapGen.GetMap()[(int)Coord.x + 1][(int)Coord.y];
                    break;
                case Cardinals.East:
                    
                    if (IsCellInBounds((int)Coord.x, (int)Coord.y + 1))
                        return _mapGen.GetMap()[(int)Coord.x][(int)Coord.y + 1];
                    
                    break;
                case Cardinals.South:
                    
                    if (IsCellInBounds((int)Coord.x - 1, (int)Coord.y))
                        return _mapGen.GetMap()[(int)Coord.x - 1][(int)Coord.y];
                    break;
                case Cardinals.West:
                    if (IsCellInBounds((int)Coord.x, (int)Coord.y - 1))
                        return _mapGen.GetMap()[(int)Coord.x][(int)Coord.y - 1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null);
                    
            }
            return null;
        }

        private bool IsCellInBounds(int X, int Y)
        {
            return X >= 0 && X <= _mapGen.GetMapSize().x - 1 && Y <= _mapGen.GetMapSize().y - 1 && Y >= 0;
        }

        public List<MapCell> GetNeighbors()
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

        public override string ToString()
        {
            String Ids = "";

            foreach (int id in _possibleTileIDs)
            {
                Ids += id + ",";
            }
            
            return  $"IsDone : {IsDone}, coords : {Coord.ToString()}, Possible tile : {Ids}";
        }
    }
}
