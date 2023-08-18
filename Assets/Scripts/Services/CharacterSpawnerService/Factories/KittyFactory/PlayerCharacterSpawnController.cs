using System;
using Arenar.Character;
using UnityEngine;


namespace Arenar
{
    public class PlayerCharacterSpawnController
    {
        public event Action<ComponentCharacterController> OnCreatePlayerCharacter;
        
        
        private Transform playerContainer;
        private ComponentCharacterController componentCharacterPrefabs;
        
        private ICharacterEntityFactory<ComponentCharacterController> playerFactory;

        private bool canSpawn = false;
        private float spawnTimer = default;

        private ComponentCharacterController component;


        public ComponentCharacterController Component => component;


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


        public PlayerCharacterSpawnController(ICharacterEntityFactory<ComponentCharacterController> playerFactory,
                                    ComponentCharacterController componentCharacterPrefabs)
        {
            this.playerFactory = playerFactory;
            this.componentCharacterPrefabs = componentCharacterPrefabs;
        }
        
        
        public ComponentCharacterController CreateCharacter()
        {
            ComponentCharacterController componentCharacter = playerFactory.Create(componentCharacterPrefabs, playerContainer);
            componentCharacter.gameObject.transform.SetParent(PlayerContainer);
            component = componentCharacter;
            OnCreatePlayerCharacter?.Invoke(component);
            return componentCharacter;
        }
    }
}