using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Character
{
    [Serializable]
    public class AddressablesCharacters
    {
        [SerializeField] private string _addressablesPlayerPath;
        [SerializeField] private SerializableDictionary<NpcType, CharacterData> _npcCharacterDatas;


        public string AddressablesPlayerPath => _addressablesPlayerPath;
        public SerializableDictionary<NpcType, CharacterData> NpcCharacterDatas => _npcCharacterDatas;



        [Serializable]
        public class CharacterData
        {
            [SerializeField] private string _addressablesCharacterPath;


            public string AddressablesCharacterPath => _addressablesCharacterPath;
        }
    }
}