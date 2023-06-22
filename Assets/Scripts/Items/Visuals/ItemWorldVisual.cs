using System;
using UnityEngine;


namespace Arenar
{
    [Serializable]
    public class ItemWorldVisual
    {
        [SerializeField] private Mesh mesh = default;
        [SerializeField] private float mass = default;
        [SerializeField] private Material[] materials = default;


        public float Mass => mass;

        public Mesh Mesh => mesh;

        public Material[] Materials => materials;
    }
}