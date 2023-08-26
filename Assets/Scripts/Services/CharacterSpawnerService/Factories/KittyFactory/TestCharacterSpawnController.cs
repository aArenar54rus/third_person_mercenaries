using System;
using Arenar.Character;
using UnityEngine;


namespace Arenar
{
    public class TestCharacterSpawnController
    {
        public event Action<ComponentCharacterController> OnCreatePlayerCharacter;
        
        
        private Transform playerContainer;
        private ComponentCharacterController componentCharacterPrefabs;
        private PuppetComponentCharacterController puppetComponentCharacterPrefab;
        private ICharacterEntityFactory<ComponentCharacterController> playerFactory;
        
        private ComponentCharacterController playerCharacter;
        
        private bool canSpawn = false;
        private float spawnTimer = default;


        public ComponentCharacterController PlayerCharacter => playerCharacter;


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


        public TestCharacterSpawnController(ICharacterEntityFactory<ComponentCharacterController> playerFactory,
                                    ComponentCharacterController componentCharacterPrefabs,
                                    PuppetComponentCharacterController puppetComponentCharacterPrefab)
        {
            this.playerFactory = playerFactory;
            this.componentCharacterPrefabs = componentCharacterPrefabs;
            this.puppetComponentCharacterPrefab = puppetComponentCharacterPrefab;
        }
        
        
        public ComponentCharacterController CreateCharacter()
        {
            ComponentCharacterController componentCharacter = playerFactory.Create(componentCharacterPrefabs, PlayerContainer);
            componentCharacter.gameObject.transform.SetParent(PlayerContainer);
            playerCharacter = componentCharacter;
            OnCreatePlayerCharacter?.Invoke(playerCharacter);
            return componentCharacter;
        }

        public PuppetComponentCharacterController CreatePuppet()
        {
            PuppetComponentCharacterController componentCharacter =
                (PuppetComponentCharacterController)playerFactory.Create(puppetComponentCharacterPrefab, PlayerContainer);
            
            componentCharacter.gameObject.transform.SetParent(PlayerContainer);
            componentCharacter.gameObject.transform.position = new Vector3(2, 0, 0);
            return componentCharacter;
        }
    }
}