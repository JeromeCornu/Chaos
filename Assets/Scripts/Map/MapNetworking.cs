using Mirror;
using UnityEngine;

namespace Map
{
    public class MapNetworking : NetworkBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;
        
        [SyncVar(hook = nameof(OnSeedChanged))]
        private int _seed;

        void Start()
        {
            if (isServer)
            {
                _seed = Random.Range(0, int.MaxValue);
                mapGenerator.CreateMap(_seed);
            }
        }
        
        void OnSeedChanged(int oldSeed, int newSeed)
        {
            mapGenerator.CreateMap(newSeed);
        }
    }
}