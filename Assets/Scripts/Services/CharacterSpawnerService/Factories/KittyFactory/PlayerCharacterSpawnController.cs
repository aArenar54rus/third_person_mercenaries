using System;
using Arenar.Character;
using UnityEngine;


namespace Arenar
{
    public class PlayerCharacterSpawnController
    {
        public event Action<PlayerCharacterController> OnCreatePlayerCharacter;
        
        
        private Transform playerContainer;
        private PlayerCharacterController _playerCharacterPrefabs;
        
        private ICharacterEntityFactory<PlayerCharacterController> playerFactory;

        private bool canSpawn = false;
        private float spawnTimer = default;

        private PlayerCharacterController player;


        public PlayerCharacterController Player => player;


        private Transform PlayerContainer
        {
            get
            {
                if (playerContainer == null)
                {
                    playerContainer = GameObject.Instantiate(new GameObject()).transform;
                    playerContainer.gameObject.name = "Player Container";
                }

                return playerContainer;
            }
        }


        public PlayerCharacterSpawnController(ICharacterEntityFactory<PlayerCharacterController> playerFactory,
                                    PlayerCharacterController playerCharacterPrefabs)
        {
            this.playerFactory = playerFactory;
            this._playerCharacterPrefabs = playerCharacterPrefabs;
        }
        
        
        public PlayerCharacterController CreateCharacter()
        {
            PlayerCharacterController playerCharacter = playerFactory.Create(_playerCharacterPrefabs, playerContainer);
            playerCharacter.gameObject.transform.SetParent(PlayerContainer);
            player = playerCharacter;
            OnCreatePlayerCharacter?.Invoke(player);
            return playerCharacter;
        }
    }
}