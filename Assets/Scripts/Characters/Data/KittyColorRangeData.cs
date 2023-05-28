using System;
using UnityEngine;


namespace CatSimulator
{
    [Serializable]
    public class KittyColorRangeData
    {
        [SerializeField] private SerializableDictionary<int, Color[]> defaultColorsByLevel = default;
        [SerializeField] private ColorRange[] colorsList = default;


        public ColorRange[] ColorsList => colorsList;

        public SerializableDictionary<int, Color[]> DefaultColorsByLevel => defaultColorsByLevel;


        [Serializable]
        public class ColorRange
        {
            [SerializeField] private Color minimalRangeColor = default;
            [SerializeField] private Color maximumRangeColor = default;


            public Color MinimalRangeColor => minimalRangeColor;
            public Color MaximumRangeColor => maximumRangeColor;
        }
    }
}