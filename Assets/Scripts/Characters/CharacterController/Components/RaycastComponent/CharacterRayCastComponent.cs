using Arenar.CameraService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterRayCastComponent : ICharacterRayCastComponent, ITickable
    {
        private ICharacterEntity _characterEntity;
        private ICharacterLiveComponent characterLiveComponent;
        
        private Camera _camera;
        private bool _isGrounded = true;
        private PlayerCharacterParametersData _playerCharacterParametersData;
        private Transform _characterCenterPoint;
        private TickableManager _tickableManager;
        private float _maxSqrDistance;
   
        
        public bool IsGrounded { get; private set; }
        
        public Vector3 RaycastPoint { get; private set; }

        public Transform ObjectOnCross { get; private set; }
        
        public InteractableElement InteractableElementsOnCross { get; private set; }
        
        public ComponentCharacterController CharacterControllerOnCross { get; private set; }

        private Transform characterTransform => _characterEntity.CharacterTransform;


        [Inject]
        public void Construct(ICharacterEntity characterEntity,
            ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
            PlayerCharacterParametersData playerCharacterParametersData,
            ICameraService cameraService,
            TickableManager tickableManager)
        {
            _characterEntity = characterEntity;
            _playerCharacterParametersData = playerCharacterParametersData;
            _characterCenterPoint = characterPhysicsDataStorage.Data.CharacterCenterPoint;
            _maxSqrDistance = playerCharacterParametersData.InteractElementDistance
                              * playerCharacterParametersData.InteractElementDistance;
            _camera = cameraService.GameCamera;
            _tickableManager = tickableManager;
        }
        
        public void Initialize()
        {
            _characterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out characterLiveComponent);
            _tickableManager.Add(this);
        }

        public void DeInitialize()
        {
            _tickableManager.Remove(this);
        }

        public void OnStart() { }
        
        public void Tick()
        {
            if (!characterLiveComponent.IsAlive)
            {
                IsGrounded = false;
                RaycastPoint = Vector3.zero;
                ObjectOnCross = null;
                InteractableElementsOnCross = null;
                CharacterControllerOnCross = null;
                return;
            }

            IsGrounded = IsGroundedCheck();
            InteractableElementsOnCross = GetInteractableElementsOnCross();

            ObjectOnCross = TryGetObjectOnCross(out Transform objectOnCross, out Vector3 raycastPoint)
                ? objectOnCross
                : null;
            
            // RaycastPoint = Vector3.zero;
            RaycastPoint = Vector3.Lerp(RaycastPoint, raycastPoint, 100);

            CharacterControllerOnCross = GetComponentCharacterController();
        }

        private bool IsGroundedCheck()
        {
            Vector3 spherePosition = new Vector3(characterTransform.position.x, 
                characterTransform.position.y + _playerCharacterParametersData.GroundedOffset,
                characterTransform.position.z);
            _isGrounded = Physics.CheckSphere(spherePosition, _playerCharacterParametersData.GroundedRadius, _playerCharacterParametersData.GroundedLayers,
                QueryTriggerInteraction.Ignore);

            return _isGrounded;
        }
        
        private bool TryGetObjectOnCross(out Transform objectTransform)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                objectTransform = null;
                return false;
            }
            
            objectTransform = hit.transform;
            return true;
        }

        /*private Vector3 GetAimRaycastPoint()
        {
            Vector2 screenCenter = new (Screen.width / 2, Screen.height / 2);
            Ray ray = _camera.ScreenPointToRay(screenCenter);
            //Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                return hit.point;
            
            
        }*/
        
        private bool TryGetObjectOnCross(out Transform objectTransform, out Vector3 raycastPoint)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                objectTransform = null;
                raycastPoint =  _camera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f)) + Vector3.forward * 1000f;
                return false;
            }
            
            objectTransform = hit.transform;
            raycastPoint = hit.point;
            return true;
        }

        private InteractableElement GetInteractableElementsOnCross()
        {
            if (!TryGetObjectOnCross(out Transform objectHit))
                return null;
            
            if (!objectHit.gameObject.TryGetComponent<InteractableElement>(out InteractableElement element))
                return null;
            
            float heading = (objectHit.position - _characterCenterPoint.position).sqrMagnitude;
            if (heading > _maxSqrDistance)
                return null;
            
            return element;
        }

        private ComponentCharacterController GetComponentCharacterController()
        {
            if (!TryGetObjectOnCross(out Transform objectHit))
                return null;
            
            if (!objectHit.gameObject.TryGetComponent<ComponentCharacterController>(out ComponentCharacterController element))
                return null;

            return element;
        }
    }
}