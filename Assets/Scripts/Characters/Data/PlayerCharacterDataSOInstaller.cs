using UnityEngine;
using Zenject;

namespace Arenar
{
    [CreateAssetMenu(fileName = "Kitty", menuName = "Kitties Data", order = 51)]
    public class PlayerCharacterDataSOInstaller : ScriptableObjectInstaller<PlayerCharacterDataSOInstaller>
    {
        // [SerializeField] private ComponentCharacterController componentCharacterPrefab = default;
        // [SerializeField] private PuppetComponentCharacterController puppetComponentCharacterPrefab = default;
        [SerializeField] private PlayerCharacterParametersData playerCharacterParametersData = default;


        public override void InstallBindings()
        {
            // Container.BindInstance(componentCharacterPrefab).AsSingle();
            // Container.BindInstance(puppetComponentCharacterPrefab).AsSingle();
            Container.BindInstance(playerCharacterParametersData).AsSingle();
        }
    }
}