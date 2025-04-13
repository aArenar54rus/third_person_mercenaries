using System;
using System.Collections.Generic;
using Arenar.Character;
using UnityEngine;

namespace Arenar
{
    public class CharacterSpawnController
    {
        public event Action<ICharacterEntity> OnCreatePlayerCharacter;
        public event Action<ICharacterEntity> OnCreateEnemyCharacter;
        
        
        private Transform _charactersContainer;
        
        private ICharacterEntityFactory<PhysicalHumanoidComponentCharacterController> _physicsHumanoidCharacterFactory;
        private ICharacterEntityFactory<ShootingGalleryTargetCharacterController> _shootingGalleryTargetFactory;
        
        private Dictionary<CharacterTypeKeys, List<ICharacterEntity>> _activeHumanoidCharacters = new();
        private Dictionary<CharacterTypeKeys, Queue<ICharacterEntity>> _createdHumanoidCharacters = new();


        public ICharacterEntity PlayerCharacter => _activeHumanoidCharacters[CharacterTypeKeys.Player][0];
        
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


        public CharacterSpawnController(ICharacterEntityFactory<PhysicalHumanoidComponentCharacterController> physicsHumanoidCharacterFactory,
                                        ICharacterEntityFactory<ShootingGalleryTargetCharacterController> shootingGalleryTargetFactory)
        {
            _physicsHumanoidCharacterFactory = physicsHumanoidCharacterFactory;
            _shootingGalleryTargetFactory = shootingGalleryTargetFactory;
            _createdHumanoidCharacters = new Dictionary<CharacterTypeKeys, Queue<ICharacterEntity>>();
        }
        
        
        public ICharacterEntity GetCharacter(CharacterTypeKeys characterType)
        {
            ICharacterEntity componentCharacter = GetHumanoidCharacter(characterType);
            
            if (characterType == CharacterTypeKeys.Player)
                OnCreatePlayerCharacter?.Invoke(componentCharacter);
            else OnCreateEnemyCharacter?.Invoke(componentCharacter);
            
            return componentCharacter;
        }
        
        private ICharacterEntity GetHumanoidCharacter(CharacterTypeKeys characterType)
        {
            ICharacterEntity componentCharacter = null;

            if (_createdHumanoidCharacters.ContainsKey(characterType)
                && _createdHumanoidCharacters[CharacterTypeKeys.DefaultKnight].Count > 0)
            {
                componentCharacter = _createdHumanoidCharacters[characterType].Dequeue();
            }
            else
            {
                if (!_createdHumanoidCharacters.ContainsKey(characterType))
                    _createdHumanoidCharacters.Add(characterType, new Queue<ICharacterEntity>());
                
                componentCharacter = _physicsHumanoidCharacterFactory.Create(characterType);
                _createdHumanoidCharacters[characterType].Enqueue(componentCharacter);

                componentCharacter.EntityObjectTransform.SetParent(CharactersContainer);
                componentCharacter.Initialize();
            }

            return componentCharacter;
        }

       public ShootingGalleryTargetCharacterController CreateShootingGalleryTarget()
       {
           /*if (!_createdCharacters.ContainsKey(typeof(ShootingGalleryTargetCharacterController)))
               _createdCharacters.Add(typeof(ShootingGalleryTargetCharacterController), new List<ComponentCharacterController>());

           foreach (var createdTarget in _createdCharacters[typeof(ShootingGalleryTargetCharacterController)])
           {
               if (createdTarget.gameObject.activeSelf)
                   continue;

               return (ShootingGalleryTargetCharacterController)createdTarget;
           }

           ShootingGalleryTargetCharacterController newTarget = _shootingGalleryTargetFactory.Create(CharactersContainer, null);
           _createdCharacters[typeof(ShootingGalleryTargetCharacterController)].Add(newTarget);
           newTarget.Initialize();

           return newTarget;*/

            return default;
        }

        public void DisableAllCharacters()
        {
            foreach (var characters in _createdHumanoidCharacters)
            {
                foreach (var character in characters.Value)
                {
                    character.DeActivate();
                    character.DeInitialize();
                    character.CharacterTransform.gameObject.SetActive(false);
                }
            }
        }
    }
}