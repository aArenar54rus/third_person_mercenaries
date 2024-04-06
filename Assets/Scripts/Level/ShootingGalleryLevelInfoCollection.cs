using UnityEngine;


namespace Arenar.Services.LevelsService
{
    [CreateAssetMenu(menuName = "LevelsData/ShootingGalleryLevelInfoCollection")]
    public class ShootingGalleryLevelInfoCollection : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<int, ShootingGalleryTargetNode[]> _shootingGalleriesInfos;


        public SerializableDictionary<int, ShootingGalleryTargetNode[]> ShootingGalleriesInfos =>
            _shootingGalleriesInfos;
    }
}