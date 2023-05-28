using CatSimulator.CameraService;
using CatSimulator.Character;
using UnityEngine;
using Zenject;


namespace CatSimulator
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Inject] private KittySpawnController kittySpawnController;
        [Inject] private ICameraService cameraService;

        private PlayerCharacterController _playerCharacter;


        private void Start()
        {
            _playerCharacter = kittySpawnController.CreateKitty();
            
            cameraService.SetCameraState<CameraStateThirdPerson>(_playerCharacter.CameraTransform , _playerCharacter.CharacterTransform);
        }
        
        public void OnDrawGizmos()
        {
            if (_playerCharacter == null)
                return;
            
            Transform characterTransform = _playerCharacter.CharacterTransform;
            Vector3 spherePosition = new Vector3(characterTransform.position.x, 
                characterTransform.position.y - 0.05f,
                characterTransform.position.z);
            
            Debug.DrawLine(new Vector3(characterTransform.position.x, characterTransform.position.y, characterTransform.position.z),
                spherePosition,
                Color.red);
            
            Gizmos.DrawSphere(spherePosition, 0.1f);
        }
    }
}