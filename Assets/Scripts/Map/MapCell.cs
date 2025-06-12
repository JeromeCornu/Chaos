using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapCell
    {
        public bool IsDone;
        
        private int TileID;
        private TileIds possibleGameObjectIDs;

        public MapCell(TileIds possibleGameObjectIDs)
        {
            
            //TODO : NEED TO MAKE A COPY OF THE OBJECT TO BE ABLE TO DELETE
            //(planing on duplicating the resulted dictionary (see if dictionary entries can be deleted))
            
            this.possibleGameObjectIDs = possibleGameObjectIDs;
        }

        public void SetRandomObject()
        {
            TileID = (int)(Random.value * possibleGameObjectIDs.GetTileCount());
            
            IsDone = true;
        }

        public void SetCellAsTile(int ID)
        {
            TileID = ID;
            
            IsDone = true;
        }
    }
}
