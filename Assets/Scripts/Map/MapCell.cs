using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Map
{
    public class MapCell
    {
        public bool IsDone;
        
        private int GameObjectID;
        private List<int> possibleGameObjectIDs;
        
        private MapCellCompatibility compatibility;

        public MapCell(List<int> possibleGameObjectIDs)
        {
            this.possibleGameObjectIDs = possibleGameObjectIDs;
        }

        public void SetRandomObject()
        {
            int index = (int)Random.value * possibleGameObjectIDs.Count;
            
            GameObjectID = possibleGameObjectIDs[index];
            
            IsDone = true;
        }
        
        
        
    }
}