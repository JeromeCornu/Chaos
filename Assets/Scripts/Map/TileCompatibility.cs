using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    [CreateAssetMenu(fileName = "TileCompatibility", menuName = "ScriptableObjects/Map/TileCompat", order = 1)]
    public class TileCompatibility : ScriptableObject
    {
        [SerializeField] public TileBase Tile;
        
        [SerializeField] private String SocketNorth;
        [SerializeField] private String SocketEast;
        [SerializeField] private String SocketSouth;
        [SerializeField] private String SocketWest;

        public String GetSocketID(Cardinals cardinal)
        {
            switch(cardinal)
            {
                case Cardinals.North:
                    return SocketNorth;
                case Cardinals.East:
                    return SocketEast;
                case Cardinals.South:
                    return SocketSouth;
                case Cardinals.West:
                    return SocketWest;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null);
            }
        }
    }

    public enum Cardinals
    {
        North,
        East,
        South,
        West
    }
    
    public static class CardinalsExtensions
    {
        public static Cardinals Opposite(this Cardinals direction)
        {
            return direction switch
            {
                Cardinals.North => Cardinals.South,
                Cardinals.South => Cardinals.North,
                Cardinals.East  => Cardinals.West,
                Cardinals.West  => Cardinals.East,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), "Invalid direction")
            };
        }
    }
}