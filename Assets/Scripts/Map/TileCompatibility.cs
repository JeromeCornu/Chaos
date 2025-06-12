using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(fileName = "TileCompatibility", menuName = "ScriptableObjects/Map/TileCompat", order = 1)]
    public class TileCompatibility : ScriptableObject
    {
        [SerializeField] public int TileID;
        
        [SerializeField] private List<int> PossibleTileNorth;
        [SerializeField] private List<int> PossibleTileEast;
        [SerializeField] private List<int> PossibleTileSouth;
        [SerializeField] private List<int> PossibleTileWest;


        public List<int> GetCompatibleTilesInDirection(Cardinals direction)
        {
            switch (direction)
            {
                case Cardinals.North:
                    return PossibleTileNorth;
                    break;
                case Cardinals.East:
                    return PossibleTileEast;
                    break;
                case Cardinals.South:
                    return PossibleTileSouth;
                    break;
                case Cardinals.West:
                    return PossibleTileWest;
                    break;
                default:
                    return null;
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