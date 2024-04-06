using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;


namespace Arenar.Character
{
    [Serializable]
    public class AddressablesCharacters
    {
        [SerializeField] private AssetReference _addressablesPlayer;
        [SerializeField] private SerializableDictionary<NpcType, CharacterData> _npcCharacterDatas;


        public AssetReference AddressablesPlayer => _addressablesPlayer;
        public SerializableDictionary<NpcType, CharacterData> NpcCharacterDatas => _npcCharacterDatas;



        [Serializable]
        public class CharacterData
        {
            [SerializeField] private AssetReference _addressablesCharacter;


            public AssetReference AddressablesCharacter => _addressablesCharacter;
        }
    }
}