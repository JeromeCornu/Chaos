using DefaultNamespace.UI;
using GameState;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Map.MapObstacles
{
    public class ObstacleCard : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private MapObstacleManager _mapObstacleManager;
        private int _id;
        private Vector3 _previousPosition;
        
        
        [SerializeField] private Image obsIcon;
        [SerializeField] private TextMeshProUGUI obsName;
        
        public void Init(int id, string obstacleName, Sprite sprite, MapObstacleManager mapObstacleManager)
        {
            _id = id;
            obsName.text = obstacleName;
            obsIcon.sprite = sprite;
            
            _mapObstacleManager = mapObstacleManager;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
            transform.SetParent(UIManager.Instance.transform);
            UIManager.Instance.HideMapObstacle();
            
            _mapObstacleManager.Input.PlayerInputs.Cancel.performed += OnCanceledPerformed;
            _mapObstacleManager.SetActiveVisualTileMap(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("OnDrag");
            //make the obstacle follow mouse
            Vector3 worldPosition = _mapObstacleManager.cam.ScreenToWorldPoint(Input.mousePosition);
            if (worldPosition == _previousPosition) return;
            _previousPosition = worldPosition;
            _mapObstacleManager.ClearVisualMap();
            _mapObstacleManager.PlaceObstacleToServer(_id, worldPosition, true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
            _mapObstacleManager.Input.PlayerInputs.Cancel.performed -= OnCanceledPerformed; 

            Vector3 worldPosition = _mapObstacleManager.cam.ScreenToWorldPoint(Input.mousePosition);
            
            _mapObstacleManager.ClearVisualMap();
            _mapObstacleManager.SetActiveVisualTileMap(false);
            _mapObstacleManager.PlaceObstacleToServer(_id, worldPosition, false);
            
            ((MapEditing)GameManager.Instance.CurrentGameState).playerOneIsDone();
            
            Destroy(gameObject);
        }

        private void OnCanceledPerformed(InputAction.CallbackContext ctx)
        {
            _mapObstacleManager.SetActiveVisualTileMap(true);
            UIManager.Instance.ShowMapObstacle(
                ((MapEditing)GameManager.Instance.CurrentGameState).CanInteract()
            );
        }
        
    }
}