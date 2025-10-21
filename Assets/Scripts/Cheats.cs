using System;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    public class Cheats : MonoBehaviour
    {
        private GameObject _player;
        private Health _playerHealth;
        
        private void Start()
        {
            _player = LobbyController.Instance.LocalPlayerObject;
            _playerHealth = _player.GetComponent<Health>();
        }

        [Button]
        public void DIE()
        {
            _playerHealth.Die();
        }
        
    }
}