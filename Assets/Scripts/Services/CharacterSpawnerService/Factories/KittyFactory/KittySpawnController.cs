using CatSimulator.Character;
using UnityEngine;


namespace CatSimulator
{
    public class KittySpawnController 
    {
        private Transform kittyContainer;
        private PlayerCharacterController _playerCharacterPrefabs;
        
        private ICharacterEntityFactory<PlayerCharacterController> kittyFactory;

        private bool canSpawn = false;
        private float spawnTimer = default;


        private Transform KittyContainer
        {
            get
            {
                if (kittyContainer == null)
                {
                    kittyContainer = GameObject.Instantiate(new GameObject()).transform;
                    kittyContainer.gameObject.name = "Kitty Container";
                }

                return kittyContainer;
            }
        }


        public KittySpawnController(ICharacterEntityFactory<PlayerCharacterController> kittyFactory,
                                    PlayerCharacterController playerCharacterPrefabs)
        {
            this.kittyFactory = kittyFactory;
            this._playerCharacterPrefabs = playerCharacterPrefabs;
        }
        
        
        public PlayerCharacterController CreateKitty()
        {
            PlayerCharacterController playerCharacter = kittyFactory.Create(_playerCharacterPrefabs, kittyContainer);
            playerCharacter.gameObject.transform.SetParent(KittyContainer);
            return playerCharacter;
        }
    }
}