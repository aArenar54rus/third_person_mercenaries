using System;
using UnityEngine;

namespace Arenar.Character
{
    [Serializable]
    public class AddressablesCharacters
    {
        [SerializeField] private SerializableDictionary<CharacterTypeKeys, CharacterData> _resourcesPhysicalCharacterPrefabs;
        [SerializeField] private SerializableDictionary<NpcType, CharacterData> _npcCharacterDatas;


        public SerializableDictionary<CharacterTypeKeys, CharacterData> ResourcesPhysicsHumanoidPrefab => _resourcesPhysicalCharacterPrefabs;
        public SerializableDictionary<NpcType, CharacterData> NpcCharacterDatas => _npcCharacterDatas;



        [Serializable]
        public class CharacterData
        {
            [SerializeField] private string characterPrefabResources;

            
            public string CharacterPrefabResources => characterPrefabResources;
        }
    }
}