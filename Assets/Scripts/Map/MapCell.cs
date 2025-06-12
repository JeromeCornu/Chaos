using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class MapCell
    {
        public bool IsDone;
        
        private int TileID;
        private Dictionary<int, TileCompatibility> possibleGameObjectIDs;

        public MapCell(Dictionary<int, TileCompatibility> possibleGameObjectIDs)
        {
            this.possibleGameObjectIDs = possibleGameObjectIDs;
        }

        public void SetRandomObject()
        {
            TileID = (int)(Random.value * possibleGameObjectIDs.Count);
            
            IsDone = true;
        }

        public void SetCellAsTile(int ID)
        {
            TileID = ID;
            
            IsDone = true;
        }
    }
}
