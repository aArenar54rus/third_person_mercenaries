using Arenar;
using Arenar.CameraService;
using UnityEngine;
using Zenject;


public class TestScript : MonoBehaviour
{
    private TestCharacterSpawnController testCharacterSpawnController;
    private ICameraService cameraService;

    
    [Inject]
    public void Construct(TestCharacterSpawnController testCharacterSpawnController, ICameraService cameraService)
    {
        this.testCharacterSpawnController = testCharacterSpawnController;
        this.cameraService = cameraService;
    }
    
    public void Start()
    {
        var player = testCharacterSpawnController.CreateCharacter();
        cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform, player.CharacterTransform);

        testCharacterSpawnController.CreatePuppet();
    }
}
