using Arenar.Services.LevelsService;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Arenar.Character
{
    public class CharacterNavMeshMovementComponent : ICharacterMovementComponent
    {
        private ILevelsService levelsService;
        
        private ICharacterEntity characterOwner;
        private ICharacterLiveComponent characterLiveComponent;
        
        private NavMeshAgent navMeshAgent;
        
        private EnemyCharacterDataStorage enemyCharacterDataStorage;
        private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> characterAnimationComponent;
        
        private float animationBlend = 0;
        
        
        public MovementContainer MovementContainer { get; set; }

        
        [Inject]
        public void Construct(ICharacterEntity character, 
                              ILevelsService levelsService,
                              ICharacterDataStorage<EnemyCharacterDataStorage> enemyCharacterDataStorage,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage)
        {
            characterOwner = character;
            this.levelsService = levelsService;
            this.enemyCharacterDataStorage = enemyCharacterDataStorage.Data;
            this.characterPhysicsDataStorage = characterPhysicsDataStorage.Data;
        }

        public void Initialize()
        {
            navMeshAgent = characterPhysicsDataStorage.NavMeshAgent;
            if (navMeshAgent == null)
            {
                Debug.LogError("NavMeshAgent не найден у объекта персонажа");
                return;
            }

            navMeshAgent.enabled = false;
            
            if (characterOwner.TryGetCharacterComponent(out ICharacterAnimationComponent animComponent))
                characterAnimationComponent = animComponent as CharacterAnimationComponent;
        }

        public void DeInitialize() { }

        public void OnActivate()
        {
            if (characterOwner.TryGetCharacterComponent(out characterLiveComponent))
            {
                characterLiveComponent.OnCharacterDie += CharacterDieHandler;
            }
            
            MovementContainer = new MovementContainer();

            MovementContainer.MovementSpeed = MovementContainer.SprintSpeed =
                enemyCharacterDataStorage.EnemyCharacterParameters.BaseSpeed[levelsService.CurrentLevelContext.LevelDifficult];
            
            MovementContainer.RotationSpeed = enemyCharacterDataStorage.EnemyCharacterParameters.BaseRotationSpeed[levelsService.CurrentLevelContext.LevelDifficult];

            if (navMeshAgent != null)
            {
                navMeshAgent.speed = MovementContainer.MovementSpeed;
                
                navMeshAgent.acceleration = MovementContainer.RotationSpeed;
                navMeshAgent.stoppingDistance = 0.5f;

                navMeshAgent.enabled = true;
            }
        }
        
        public void OnDeactivate()
        {
            if (navMeshAgent != null)
                navMeshAgent.enabled = false;
        }
        
        public void Move(Vector3 targetPosition, bool isSprint)
        {
            if (targetPosition == Vector3.zero)
            {
                characterAnimationComponent?.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Speed, 0);
                return;
            }
            
            if (navMeshAgent == null)
            {
                return;
            }

            navMeshAgent.SetDestination(targetPosition);
            characterAnimationComponent?.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Speed, 0.5f);
        }

        public void Rotation(Vector2 direction)
        {
            if (direction == Vector2.zero || navMeshAgent == null)
                return;

            float targetRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, targetRotation, 0);
            characterOwner.CharacterTransform.rotation = Quaternion.Slerp(
                characterOwner.CharacterTransform.rotation, 
                rotation, 
                Time.deltaTime * MovementContainer.RotationSpeed);
        }

        public void JumpAndGravity(bool jumpAction)
        {
            
        }

        private void CharacterDieHandler(ICharacterEntity character)
        {
            characterLiveComponent.OnCharacterDie -= CharacterDieHandler;
            navMeshAgent.ResetPath();
        }
    }
}