using System.Collections.Generic;
using Arenar;
using Arenar.CameraService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterRayCastComponent : ICharacterRayCastComponent
    {
        private ICharacterEntity _characterEntity;
        private Camera _camera;
        private bool _isGrounded = true;
        private PlayerCharacterParametersData _playerCharacterParametersData;
        private Transform _characterCenterPoint;
        private float _maxSqrDistance;
   

        private Transform characterTransform =>
            _characterEntity.CharacterTransform;


        [Inject]
        public void Construct(ICharacterEntity characterEntity, ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage, PlayerCharacterParametersData playerCharacterParametersData, ICameraService cameraService)
        {
            _characterEntity = characterEntity;
            _playerCharacterParametersData = playerCharacterParametersData;
            _characterCenterPoint = characterPhysicsDataStorage.Data.CharacterCenterPoint;
            _maxSqrDistance = playerCharacterParametersData.InteractElementDistance
                              * playerCharacterParametersData.InteractElementDistance;
            _camera = cameraService.GameCamera;
        }

        public bool IsGroundedCheck()
        {
            Vector3 spherePosition = new Vector3(characterTransform.position.x, 
                characterTransform.position.y + _playerCharacterParametersData.GroundedOffset,
                characterTransform.position.z);
            _isGrounded = Physics.CheckSphere(spherePosition, _playerCharacterParametersData.GroundedRadius, _playerCharacterParametersData.GroundedLayers,
                QueryTriggerInteraction.Ignore);

            return _isGrounded;
        }

        public InteractableElement GetInteractableElementsOnCross()
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (!Physics.Raycast(ray, out RaycastHit hit))
                return null;
            
            Transform objectHit = hit.transform;
            if (!objectHit.gameObject.TryGetComponent<InteractableElement>(out InteractableElement element))
                return null;
            
            float heading = (objectHit.position - _characterCenterPoint.position).sqrMagnitude;
            if (heading > _maxSqrDistance)
                return null;
            
            return element;
        }

        public void Initialize() { }

        public void DeInitialize() { }

        public void OnStart() { }
    }
}