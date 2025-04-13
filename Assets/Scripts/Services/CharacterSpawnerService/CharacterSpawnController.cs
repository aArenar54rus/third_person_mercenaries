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
        private Dictionary<CharacterTypeKeys, Queue<ICharacterEntity>> createdHumanoidCharacters = new();


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
            createdHumanoidCharacters = new Dictionary<CharacterTypeKeys, Queue<ICharacterEntity>>();
        }
        
        
        public ICharacterEntity GetCharacter(CharacterTypeKeys characterType)
        {
            ICharacterEntity characterEntity = GetHumanoidCharacter(characterType);
            
            if (!_activeHumanoidCharacters.ContainsKey(characterType))
                _activeHumanoidCharacters.Add(characterType, new List<ICharacterEntity>());
            _activeHumanoidCharacters[characterType].Add(characterEntity);
            
            if (characterType == CharacterTypeKeys.Player)
                OnCreatePlayerCharacter?.Invoke(characterEntity);
            else OnCreateEnemyCharacter?.Invoke(characterEntity);
            
            characterEntity.EntityObjectTransform.gameObject.SetActive(true);
            return characterEntity;
        }
        
        public void ReturnHumanoidCharacter(ICharacterEntity characterEntity)
        {
            if (!createdHumanoidCharacters.ContainsKey(characterEntity.CharacterType))
                createdHumanoidCharacters.Add(characterEntity.CharacterType, new Queue<ICharacterEntity>());
            createdHumanoidCharacters[characterEntity.CharacterType].Enqueue(characterEntity);

            if (_activeHumanoidCharacters.ContainsKey(characterEntity.CharacterType))
                _activeHumanoidCharacters[characterEntity.CharacterType].Remove(characterEntity);
            
            characterEntity.DeActivate();
            characterEntity.DeInitialize();
            characterEntity.EntityObjectTransform.gameObject.SetActive(false);
        }
        
        private ICharacterEntity GetHumanoidCharacter(CharacterTypeKeys characterType)
        {
            ICharacterEntity componentCharacter = null;

            if (createdHumanoidCharacters.ContainsKey(characterType)
                && createdHumanoidCharacters[characterType].Count > 0)
            {
                componentCharacter = createdHumanoidCharacters[characterType].Dequeue();
            }
            else
            {
                componentCharacter = _physicsHumanoidCharacterFactory.Create(characterType);
                componentCharacter.EntityObjectTransform.SetParent(CharactersContainer);
            }
            
            componentCharacter.Initialize();
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
            return;
            
            foreach (var characters in createdHumanoidCharacters)
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