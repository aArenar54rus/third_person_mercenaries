using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;


namespace Arenar.Character
{
    [Serializable]
    public class AddressablesCharacters
    {
        [SerializeField] private string _resourcesPlayerPrefab;
        [SerializeField] private SerializableDictionary<NpcType, CharacterData> _npcCharacterDatas;


        public string ResourcesPlayerPrefab => _resourcesPlayerPrefab;
        public SerializableDictionary<NpcType, CharacterData> NpcCharacterDatas => _npcCharacterDatas;



        [Serializable]
        public class CharacterData
        {
            [SerializeField] private string characterPrefabResources;

            
            public string CharacterPrefabResources => characterPrefabResources;
        }
    }
}