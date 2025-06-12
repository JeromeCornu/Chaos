using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(fileName = "MapTiles", menuName = "ScriptableObjects/Map/MapTiles", order = 2)]
    public class MapTiles : ScriptableObject
    {
        [SerializeField] List<TileCompatibility> mapTiles;
    }
}
