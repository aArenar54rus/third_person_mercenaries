using System;
using UnityEngine;


namespace Arenar.Services.LevelsService
{
    [Serializable]
    public class ShootingGalleryTargetNode
    {
        [SerializeField] private float activateTime;
        [SerializeField] private int _pathPointIndex;


        public float ActivateTime => activateTime;
        public int PathPointIndex => _pathPointIndex;
    }
}