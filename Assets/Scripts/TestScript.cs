using Arenar;
using Arenar.CameraService;
using Arenar.Character;
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
        ComponentCharacterController characterController = testCharacterSpawnController.CreateCharacter();
        if (characterController is PlayerComponentCharacterController player)
            cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform, characterController.CharacterTransform);

        testCharacterSpawnController.CreatePuppet();
    }
}
