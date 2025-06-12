using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    [CreateAssetMenu(fileName = "TileCompatibility", menuName = "ScriptableObjects/Map/TileCompat", order = 1)]
    public class TileCompatibility : ScriptableObject
    {
        [SerializeField] public TileBase TileID;
        
        [SerializeField] private int SocketNorth;
        [SerializeField] private int SocketEast;
        [SerializeField] private int SocketSouth;
        [SerializeField] private int SocketWest;

        public int GetSocketID(Cardinals cardinal)
        {
            switch(cardinal)
            {
                case Cardinals.North:
                    return SocketNorth;
                    break;
                case Cardinals.East:
                    return SocketEast;
                    break;
                case Cardinals.South:
                    return SocketSouth;
                    break;
                case Cardinals.West:
                    return SocketWest;
                    break;
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
}