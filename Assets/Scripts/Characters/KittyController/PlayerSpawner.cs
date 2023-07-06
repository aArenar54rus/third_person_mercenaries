using Arenar.CameraService;
using Arenar.Character;
using UnityEngine;
using Zenject;


namespace Arenar
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Inject] private PlayerCharacterSpawnController _playerCharacterSpawnController;
        [Inject] private ICameraService cameraService;

        private PlayerCharacterController _playerCharacter;


        private void Start()
        {
            _playerCharacter = _playerCharacterSpawnController.CreateKitty();
            
            // cameraService.SetCameraState<CameraStateThirdPerson>(_playerCharacter.CameraTransform , _playerCharacter.CharacterTransform);
            cameraService.SetCinemachineVirtualCamera(CinemachineCameraType.DefaultTPS);
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