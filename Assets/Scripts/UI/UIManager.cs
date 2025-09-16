using Mirror;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace DefaultNamespace.UI
{
    public class UIManager : NetworkBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject MapObstacle;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ShowLoadingScreen()
        {
            loadingScreen.SetActive(true);
        }

        public void HideLoadingScreen()
        {
            loadingScreen.SetActive(false);
        }
        
        [Command(requiresAuthority = false)]
        public void ShowMapObstacle(bool canInteract)
        {
            ShowMapObstacleRPC(canInteract);
        }
        
        [ClientRpc]
        public void ShowMapObstacleRPC(bool canInteract)
        {
            MapObstacle.SetActive(true);
            MapObstacle.TryGetComponent(out GraphicRaycaster graphicRaycaster);
            graphicRaycaster.enabled = canInteract;
        }
        
        [Command(requiresAuthority = false)]
        public void HideMapObstacle()
        {
            HideMapObstacleRPC();
        }
        [ClientRpc]
        public void HideMapObstacleRPC()
        {
            MapObstacle.SetActive(false);
        }

        public GameObject GetCardGrid()
        {
            return MapObstacle.GetComponentInChildren<GridLayoutGroup>().gameObject;
        }
        
    }
}