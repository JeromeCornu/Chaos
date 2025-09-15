using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace DefaultNamespace.UI
{
    public class UIManager : MonoBehaviour
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

        [Button]
        public void ShowMapObstacle()
        {
            MapObstacle.SetActive(true);
        }

        public void HideMapObstacle()
        {
            MapObstacle.SetActive(false);
        }

        public GameObject GetCardGrid()
        {
            return MapObstacle.GetComponentInChildren<GridLayoutGroup>().gameObject;
        }
        
    }
}