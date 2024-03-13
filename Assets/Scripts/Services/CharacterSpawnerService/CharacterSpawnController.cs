using System;
using Arenar.Character;
using Arenar.Services.LevelsService;
using UnityEngine;


namespace Arenar
{
    public class CharacterSpawnController
    {
        public event Action<ComponentCharacterController> OnCreatePlayerCharacter;
        
        
        private Transform _charactersContainer;
        private ComponentCharacterController _componentCharacterPrefabs;
        private PuppetComponentCharacterController _puppetComponentCharacterPrefab;
        
        private ICharacterEntityFactory<ComponentCharacterController> _playerFactory;
        private ICharacterEntityFactory<ShootingGalleryTargetCharacterController> _shootingGalleryTargetFactory;
        
        private ComponentCharacterController playerCharacter;
        
        private bool canSpawn = false;
        private float spawnTimer = default;


        public ComponentCharacterController PlayerCharacter => playerCharacter;


        private Transform CharactersContainer
        {
            get
            {
                if (_charactersContainer == null)
                {
                    _charactersContainer = GameObject.Instantiate(new GameObject()).transform;
                    _charactersContainer.gameObject.name = "Characters Container";
                }

                return _charactersContainer;
            }
        }


        public CharacterSpawnController(ICharacterEntityFactory<ComponentCharacterController> playerFactory,
                                    ICharacterEntityFactory<ShootingGalleryTargetCharacterController> shootingGalleryTargetFactory,
                                    ComponentCharacterController componentCharacterPrefabs,
                                    PuppetComponentCharacterController puppetComponentCharacterPrefab)
        {
            _playerFactory = playerFactory;
            _componentCharacterPrefabs = componentCharacterPrefabs;
            _puppetComponentCharacterPrefab = puppetComponentCharacterPrefab;
            _shootingGalleryTargetFactory = shootingGalleryTargetFactory;
        }
        
        
        public ComponentCharacterController CreateCharacter()
        {
            ComponentCharacterController componentCharacter = _playerFactory.Create(CharactersContainer);
            componentCharacter.gameObject.transform.SetParent(CharactersContainer);
            playerCharacter = componentCharacter;
            OnCreatePlayerCharacter?.Invoke(playerCharacter);
            return componentCharacter;
        }

        public PuppetComponentCharacterController CreatePuppet()
        {
            PuppetComponentCharacterController componentCharacter =
                (PuppetComponentCharacterController)_playerFactory.Create(CharactersContainer);
            
            componentCharacter.gameObject.transform.SetParent(CharactersContainer);
            componentCharacter.gameObject.transform.position = new Vector3(2, 0, 0);
            return componentCharacter;
        }

        public ShootingGalleryTargetCharacterController CreateShootingGalleryTarget()
        {
            ShootingGalleryTargetCharacterController target =
                _shootingGalleryTargetFactory.Create(CharactersContainer);

            return target;
        }
    }
}