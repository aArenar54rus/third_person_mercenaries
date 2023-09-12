using UnityEngine;
using Zenject;


namespace Arenar.Services.LevelsService
{
    [CreateAssetMenu(menuName = "LevelsData")]
    public class LevelDataSOInstaller : ScriptableObjectInstaller<LevelDataSOInstaller>
    {
        [SerializeField] private LevelData[] _levelDatas;
        
        
        public override void InstallBindings()
        {
            Container.BindInstance(_levelDatas).AsSingle();
        }
    }
}