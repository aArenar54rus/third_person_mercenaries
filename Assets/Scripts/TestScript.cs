using Arenar;
using Arenar.CameraService;
using UnityEngine;
using Zenject;


public class TestScript : MonoBehaviour
{
    [Inject] private KittySpawnController KittySpawnController;
    [Inject] private ICameraService cameraService;
    
    
    void Start()
    {
        var player = KittySpawnController.CreateKitty();
        cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform, player.CharacterTransform);
    }
}
