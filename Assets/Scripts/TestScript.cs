using Arenar;
using Arenar.CameraService;
using Arenar.Character;
using UnityEngine;
using Zenject;


public class TestScript : MonoBehaviour
{
    private CharacterSpawnController _characterSpawnController;
    private ICameraService cameraService;

    
    [Inject]
    public void Construct(CharacterSpawnController characterSpawnController, ICameraService cameraService)
    {
        this._characterSpawnController = characterSpawnController;
        this.cameraService = cameraService;
    }
    
    public void Start()
    {
        ComponentCharacterController characterController = _characterSpawnController.CreateCharacter();
        if (characterController is PlayerComponentCharacterController player)
            cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform, characterController.CharacterTransform);
    }
}
