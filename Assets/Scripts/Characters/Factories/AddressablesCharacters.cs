using System;
using UnityEngine;

namespace Arenar.Character
{
    [Serializable]
    public class AddressablesCharacters
    {
        [SerializeField] private SerializableDictionary<CharacterTypeKeys, CharacterData> _resourcesPhysicalCharacterPrefabs;


        public SerializableDictionary<CharacterTypeKeys, CharacterData> ResourcesPhysicsHumanoidPrefab => _resourcesPhysicalCharacterPrefabs;


        [Serializable]
        public class CharacterData
        {
            [SerializeField] private string characterPrefabResources;

            
            public string CharacterPrefabResources => characterPrefabResources;
        }
    }
}