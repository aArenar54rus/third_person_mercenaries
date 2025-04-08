using Arenar.CameraService;
using Arenar.Character;
using UnityEngine;
using Zenject;

namespace Arenar
{
    public class PlayerSpawner : MonoBehaviour
    {
        private CharacterSpawnController characterSpawnController;
        private ICameraService cameraService;
        private PlayerSpawnPoint playerSpawnPoint;

        private ICharacterEntity playerEntity;


        [Inject]
        public void Construct(CharacterSpawnController characterSpawnController,
                              ICameraService cameraService,
                              PlayerSpawnPoint playerSpawnPoint)
        {
            this.characterSpawnController = characterSpawnController;
            this.cameraService = cameraService;
            this.playerSpawnPoint = playerSpawnPoint;
        }

        private void Start()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            playerEntity = characterSpawnController.GetCharacter(CharacterTypeKeys.Player);
            playerEntity.CharacterTransform.position = playerSpawnPoint.Position;
            playerEntity.CharacterTransform.rotation = playerSpawnPoint.Rotation;
            cameraService.SetCinemachineVirtualCamera(CinemachineCameraType.DefaultTPS);
        }
        
        public void OnDrawGizmos()
        {
            if (playerEntity == null)
                return;
            
            Transform characterTransform = playerEntity.CharacterTransform;
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