using System;
using UnityEngine;


namespace Arenar
{
    [Serializable]
    public class ItemRarityColorData
    {
        [SerializeField] private SerializableDictionary<ItemRarity, Color> itemRarityColors;


        public SerializableDictionary<ItemRarity, Color> ItemRarityColors => itemRarityColors;
    }
}