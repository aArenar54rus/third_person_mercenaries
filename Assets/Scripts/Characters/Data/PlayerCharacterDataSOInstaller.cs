using Arenar.Character;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;


namespace Arenar
{
    [CreateAssetMenu(fileName = "Kitty", menuName = "Kitties Data", order = 51)]
    public class PlayerCharacterDataSOInstaller : ScriptableObjectInstaller<PlayerCharacterDataSOInstaller>
    {
        [FormerlySerializedAs("playerCharacterPrefab")] [SerializeField] private ComponentCharacterController componentCharacterPrefab = default;
        [SerializeField] private PlayerCharacterParametersData playerCharacterParametersData = default;
        [SerializeField] private KittyColorRangeData kittyColorRangeData = default;


        public override void InstallBindings()
        {
            Container.BindInstance(componentCharacterPrefab).AsSingle();
            Container.BindInstance(playerCharacterParametersData).AsSingle();
            Container.BindInstance(kittyColorRangeData).AsSingle();
        }
    }
}