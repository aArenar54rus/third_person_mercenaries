using System;
using UnityEngine;


namespace Arenar
{
    [Serializable]
    public class BlobColorData
    {
        [SerializeField] private int colorIndex = default;
        [SerializeField] private string colorName = default;
        [SerializeField] private Color color = default;


        public int ColorIndex => colorIndex;
        public string ColorName => colorName;
        public Color Color => color;
    }
}