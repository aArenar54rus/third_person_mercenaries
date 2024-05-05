using System;
using System.Collections.Generic;
using Arenar.Character;
using Arenar.Services.LevelsService;
using UnityEngine;


namespace Arenar
{
    public class CharacterSpawnController
    {
        public event Action<ComponentCharacterController> OnCreatePlayerCharacter;
        
        
        private Transform _charactersContainer;
        
        private ICharacterEntityFactory<ComponentCharacterController> _playerFactory;
        private ICharacterEntityFactory<ShootingGalleryTargetCharacterController> _shootingGalleryTargetFactory;
        
        private ComponentCharacterController playerCharacter;

        private Dictionary<Type, List<ComponentCharacterController>> _createdCharacters;


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
                                    ICharacterEntityFactory<ShootingGalleryTargetCharacterController> shootingGalleryTargetFactory)
        {
            _playerFactory = playerFactory;
            _shootingGalleryTargetFactory = shootingGalleryTargetFactory;
            _createdCharacters = new Dictionary<Type, List<ComponentCharacterController>>();
        }
        
        
        public ComponentCharacterController CreatePlayerCharacter(Vector3 position, Quaternion rotation)
        {
            ComponentCharacterController componentCharacter = null;
            if (_createdCharacters.ContainsKey(typeof(ComponentCharacterController)))
            {
                componentCharacter = _createdCharacters[typeof(ComponentCharacterController)][0];
            }
            else
            {
                _createdCharacters.Add(typeof(ComponentCharacterController), new List<ComponentCharacterController>());

                componentCharacter = _playerFactory.Create(CharactersContainer);
                componentCharacter.gameObject.transform.SetParent(CharactersContainer);
                playerCharacter = componentCharacter;

                _createdCharacters[typeof(ComponentCharacterController)].Add(componentCharacter);
                componentCharacter.Initialize();
            }

            playerCharacter.CharacterTransform.position = position;
            playerCharacter.CharacterTransform.rotation = rotation;
            OnCreatePlayerCharacter?.Invoke(playerCharacter);
            
            return componentCharacter;
        }

        /*public PuppetComponentCharacterController CreatePuppet()
        {
            PuppetComponentCharacterController componentCharacter =
                (PuppetComponentCharacterController)_playerFactory.Create(CharactersContainer);
            
            componentCharacter.gameObject.transform.SetParent(CharactersContainer);
            componentCharacter.gameObject.transform.position = new Vector3(2, 0, 0);
            return componentCharacter;
        }*/

        public ShootingGalleryTargetCharacterController CreateShootingGalleryTarget()
        {
            if (!_createdCharacters.ContainsKey(typeof(ShootingGalleryTargetCharacterController)))
                _createdCharacters.Add(typeof(ShootingGalleryTargetCharacterController), new List<ComponentCharacterController>());
            
            foreach (var createdTarget in _createdCharacters[typeof(ShootingGalleryTargetCharacterController)])
            {
                if (createdTarget.gameObject.activeSelf)
                    continue;
                
                return (ShootingGalleryTargetCharacterController)createdTarget;
            }
            
            ShootingGalleryTargetCharacterController newTarget = _shootingGalleryTargetFactory.Create(CharactersContainer);
            _createdCharacters[typeof(ShootingGalleryTargetCharacterController)].Add(newTarget);
            newTarget.Initialize();

            return newTarget;
        }

        public void DisableAllCharacters()
        {
            foreach (var characters in _createdCharacters)
            {
                foreach (var character in characters.Value)
                {
                    character.DeActivate();
                    character.gameObject.SetActive(false);
                }
            }
        }
    }
}