using Arenar;
using Arenar.CameraService;
using UnityEngine;
using Zenject;


public class TestScript : MonoBehaviour
{
    [Inject] private TestCharacterSpawnController testCharacterSpawnController;
    [Inject] private ICameraService cameraService;
    
    
    void Start()
    {
        var player = testCharacterSpawnController.CreateCharacter();
        cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform, player.CharacterTransform);

        testCharacterSpawnController.CreatePuppet();
    }
}
