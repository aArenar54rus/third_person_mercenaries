using Arenar;
using Arenar.CameraService;
using UnityEngine;
using Zenject;


public class TestScript : MonoBehaviour
{
    [Inject] private PlayerCharacterSpawnController _playerCharacterSpawnController;
    [Inject] private ICameraService cameraService;
    
    
    void Start()
    {
        var player = _playerCharacterSpawnController.CreateKitty();
        cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform, player.CharacterTransform);
    }
}
