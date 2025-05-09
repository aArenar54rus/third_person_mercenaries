using UnityEngine;


namespace Arenar.Services.LevelsService
{
    [CreateAssetMenu(menuName = "LevelsData/ClearLocationLevelInfoCollection")]
    public class ClearLocationLevelInfoCollection : ScriptableObject
    {
        [SerializeField]
        private SerializableDictionary<int, ShootingGalleryTargetNode[]> _shootingGalleriesInfos;
        [SerializeField]
        private float _levelTime = -1.0f;

        
        public SerializableDictionary<int, ShootingGalleryTargetNode[]> ShootingGalleriesInfos =>
            _shootingGalleriesInfos;

        public float LevelTime => _levelTime;
    }
}