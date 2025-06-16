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
        
        private int _tileID;
        private List<int> _possibleTileIDs;
        public Vector2 Coord {get;}
        private MapGenerator _mapGen;

        public MapCell(List<int> possibleGameObjectIDs, Vector2Int coord, MapGenerator mapGen)
        {
            _possibleTileIDs = possibleGameObjectIDs;
            Coord= coord;
            _mapGen = mapGen;
        }

        public void SetRandomTile()
        {
            if (IsDone) return;
            
            Random rand = new Random();
            
            _tileID = _possibleTileIDs[rand.Next(_possibleTileIDs.Count)];

            // for (int i = 0; i < _possibleTileIDs.Count; i++)
            // {
            //     if (_possibleTileIDs[i] != _tileID)
            //     {
            //         _possibleTileIDs.Remove(_possibleTileIDs[i]);
            //     }
            // }
            
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
            
            if (neighbor == null || neighbor.IsDone)
                return;

            if (IsDone)
            {
                try
                {
                    List<int> toRemove = new List<int>();
                    for (var index = 0; index < neighbor._possibleTileIDs.Count; index++)
                    {
                        var tileID = neighbor._possibleTileIDs[index];

                        String cardinalSocket = TileIds.GetTileCompatibility(_tileID).GetSocketID(cardinal);
                        String otherSocket = TileIds.GetTileCompatibility(tileID).GetSocketID(cardinal.Opposite());
                        bool isCompatible = CheckForSocketCompatibility(
                            cardinalSocket,
                            otherSocket);
                        
                        
                        //Debug.Log("Result of " + cardinalSocket + " : " + otherSocket + " is " +isCompatible);
                    
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
                
                
                //Debug.Log("Result : Coord " + Coord + ": " + neighbor._possibleTileIDs.Count);
                
            }
            else
            {
                List<String> validSocketsForCardinal = new List<String>();
                List<int> toRemove = new List<int>();
                
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
                neighbor._possibleTileIDs.RemoveAll(i => toRemove.Contains(i));
            }
        }

        private bool CheckForCompatibilityInList(List<String> possibleSockets, String checkedSocket)
        {
            return possibleSockets.Count(socket => CheckForSocketCompatibility(socket, checkedSocket)) > 0;
        }
        
        public void TestCompat()
        {
            List<String> Sockets = new List<String>(){"-1","0","1","1s","2","2s","3","3s"};
            
            for (var i = 0; i < Sockets.Count; i++)
            {
                var idI = Sockets[i];

                for (var j = 0; j < Sockets.Count; j++)
                {
                    var idJ = Sockets[j];
                    
                    bool compat = CheckForSocketCompatibility(idI.ToString(), idJ.ToString());
                    
                    Debug.Log(idI + ", " + idJ + " : " + compat);
                }
            }
        }
        
        private bool CheckForSocketCompatibility(String socketID, String checkedSocket)
        {
            return (checkedSocket == "-1" && socketID == "-1") ||
                   (checkedSocket == "0" && socketID == "0") ||
                   (checkedSocket + "s" == socketID) ||
                   (checkedSocket == socketID + "s");
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

        public void SetCellAsTile(int id)
        {
            _tileID = id;

            // for (int i = 0; i < _possibleTileIDs.Count; i++)
            // {
            //     if (_possibleTileIDs[i] != _tileID)
            //     {
            //         _possibleTileIDs.Remove(_possibleTileIDs[i]);
            //     }
            // }
            
            IsDone = true;
        }

        public int GetTileID()
        {
            return _tileID;
        }

        public int GetEntropy()
        {
            return _possibleTileIDs.Count;
        }

        public override string ToString()
        {
            return  $"IsDone : {IsDone}, coords : {Coord.ToString()}, Possible tile : {_possibleTileIDs}";
        }
    }
}
