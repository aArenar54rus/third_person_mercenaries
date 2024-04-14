using Arenar.CameraService;
using Arenar.Character;
using UnityEngine;
using Zenject;


namespace Arenar
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Inject] private CharacterSpawnController _characterSpawnController;
        [Inject] private ICameraService cameraService;

        private ComponentCharacterController componentCharacter;


        private void Start()
        {
            componentCharacter = _characterSpawnController.CreatePlayerCharacter(Vector3.zero, Quaternion.identity);
            cameraService.SetCinemachineVirtualCamera(CinemachineCameraType.DefaultTPS);
        }
        
        public void OnDrawGizmos()
        {
            if (componentCharacter == null)
                return;
            
            Transform characterTransform = componentCharacter.CharacterTransform;
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