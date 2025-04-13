using Arenar.Services.LevelsService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class SGTargetMovementComponent : ICharacterMovementComponent
    {
        private const float STOP_VELOCITY_VALUE = 0.6f;
        
        
        private ICharacterEntity _character;
        private ILevelsService _levelsService;
        private ClearLocationLevelInfoCollection clearLocationLevelInfoCollection;

        private EnemyCharacterParameters _parameters;
        private SGTargetPhysicalDataStorage _targetPhysicalData;
        private float _speedAcceleration;
        private float _speedAccelerationMultiply;

        private Vector3 _lastDirection = Vector3.zero;
        

        private float SpeedAcceleration
        {
            get => _speedAcceleration;
            set => _speedAcceleration = Mathf.Clamp01(value);
        }
        
        public MovementContainer MovementContainer { get; set; }


        [Inject]
        public void Construct(ICharacterEntity character,
                              ILevelsService levelsService,
                              ICharacterDataStorage<EnemyCharacterDataStorage> enemyCharacterDataStorage,
                              ICharacterDataStorage<SGTargetPhysicalDataStorage> characterPhysicsDataStorage)
        {
            _character = character;
            _levelsService = levelsService;
            _parameters = enemyCharacterDataStorage.Data.EnemyCharacterParameters;;
            _targetPhysicalData = characterPhysicsDataStorage.Data;
        }


        public void Initialize()
        {
            MovementContainer = new MovementContainer();
            MovementContainer.MovementSpeed = _parameters.BaseSpeed[_levelsService.CurrentLevelContext.LevelDifficult];
            MovementContainer.RotationSpeed = _parameters.BaseRotationSpeed[_levelsService.CurrentLevelContext.LevelDifficult];
            _speedAccelerationMultiply =
                _parameters.BaseAccelerationSpeedMultiply[_levelsService.CurrentLevelContext.LevelDifficult];
        }

        public void DeInitialize() { }

        public void OnActivate()
        {

            SpeedAcceleration = 0;
        }

        public void OnDeactivate()
        {
            _targetPhysicalData.CharacterModelRigidbody.velocity = Vector3.zero;
        }

        public void Move(Vector2 direction, bool isRunning)
        {
            if (direction == Vector2.zero)
            {
                SpeedAcceleration -= _speedAccelerationMultiply * Time.deltaTime;
                _targetPhysicalData.CharacterModelRigidbody.velocity = Vector2.zero;
                return;
            }

            SpeedAcceleration += _speedAccelerationMultiply * Time.deltaTime;
            direction = direction.normalized;
            
            if (Vector3.Angle(direction, _lastDirection) > STOP_VELOCITY_VALUE)
                _targetPhysicalData.CharacterModelRigidbody.velocity = Vector3.zero;

            _lastDirection = direction;
            _targetPhysicalData.CharacterModelRigidbody.AddForce(direction * MovementContainer.MovementSpeed * SpeedAcceleration, ForceMode.Impulse);
        }
        
        public void JumpAndGravity(bool isJumpAction)
        {
            return;
        }
        
        public void Rotation(Vector2 direction)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            
            _targetPhysicalData.CharacterTransform.rotation =
                Quaternion.RotateTowards(_targetPhysicalData.CharacterTransform.rotation, 
                    targetRotation, 
                    MovementContainer.RotationSpeed * Time.deltaTime);
        }
    }
}