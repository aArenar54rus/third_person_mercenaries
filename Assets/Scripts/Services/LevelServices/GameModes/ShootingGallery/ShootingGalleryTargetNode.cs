using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar.Services.LevelsService
{
    [Serializable]
    public class ShootingGalleryTargetNode
    {
        [FormerlySerializedAs("_startTarget")] [SerializeField] private float activateTime;
        [SerializeField] private int _pathPointIndex;


        public float ActivateTime => activateTime;
        public int PathPointIndex => _pathPointIndex;
    }
}