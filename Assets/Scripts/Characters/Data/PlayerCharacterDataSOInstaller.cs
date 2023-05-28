using CatSimulator.Character;
using UnityEngine;
using Zenject;


namespace CatSimulator
{
    [CreateAssetMenu(fileName = "Kitty", menuName = "Kitties Data", order = 51)]
    public class PlayerCharacterDataSOInstaller : ScriptableObjectInstaller<PlayerCharacterDataSOInstaller>
    {
        [SerializeField] private PlayerCharacterController playerCharacterPrefab = default;
        [SerializeField] private PlayerCharacterParametersData playerCharacterParametersData = default;
        [SerializeField] private KittyColorRangeData kittyColorRangeData = default;


        public override void InstallBindings()
        {
            Container.BindInstance(playerCharacterPrefab).AsSingle();
            Container.BindInstance(playerCharacterParametersData).AsSingle();
            Container.BindInstance(kittyColorRangeData).AsSingle();
        }
    }
}