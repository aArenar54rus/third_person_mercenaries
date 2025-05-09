using System.Collections.Generic;
using Arenar.Services.LevelsService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class SGTargetMoveState : AIState
    {
        private const float MINIMAL_DISTANCE_FOR_CHECK = 1.0f;
        
        
        private ClearLocationLevelInfoCollection clearLocationLevelInfoCollection;
        private ShootingGalleryTargetNode _currentCharacterPathNode;
        private List<ShootingGalleryTargetPoint> _pathPoints;
        private ILevelsService _levelsService;

        private ICharacterMovementComponent _characterMovementComponent;

        private int _progressPointIndex = 0;
        
        #if UNITY_EDITOR
        private TestLineRenderer _testLine;
        #endif


        private ShootingGalleryTargetPoint CurrentPathPoint =>
            _pathPoints[_currentCharacterPathNode.PathPointIndexes[_progressPointIndex]];

        private ICharacterMovementComponent CharacterMovementComponent
        {
            get
            {
                if (_characterMovementComponent == null)
                    character.TryGetCharacterComponent(out _characterMovementComponent);
                return _characterMovementComponent;
            }
        }


        [Inject]
        private void Construct(ILevelsService levelsService,
            ClearLocationLevelInfoCollection clearLocationLevelInfoCollection,
            List<ShootingGalleryTargetPoint> pathPoints)
        {
            _levelsService = levelsService;
            _pathPoints = pathPoints;
            this.clearLocationLevelInfoCollection = clearLocationLevelInfoCollection;
        }

        public override void Initialize(ICharacterEntity character)
        {
            base.Initialize(character);
            if (_pathPoints[0] == null)
                _pathPoints = GameObject.FindObjectOfType<ShootingGalleryTargetPointsCollection>().TargetPoints;
            
            _progressPointIndex = 0;
            if (base.character is ShootingGalleryTargetCharacterController SGTargetController)
            {
                int characterIndex = SGTargetController.TargetCharacterIndex;

                _currentCharacterPathNode =
                    clearLocationLevelInfoCollection
                        .ShootingGalleriesInfos[_levelsService.CurrentLevelContext.LevelData.LevelIndex][characterIndex];
            }

            base.character.CharacterTransform.position = CurrentPathPoint.Position;
        }

        public override void DeInitialize()
        {
            #if UNITY_EDITOR
            if (_testLine != null)
                GameObject.Destroy(_testLine.gameObject);
            #endif
        }

        public override void OnStateBegin()
        {
            _progressPointIndex = 0;
            if (character is ShootingGalleryTargetCharacterController SGTargetController)
            {
                int characterIndex = SGTargetController.TargetCharacterIndex;

                _currentCharacterPathNode =
                    clearLocationLevelInfoCollection
                        .ShootingGalleriesInfos[_levelsService.CurrentLevelContext.LevelData.LevelIndex][characterIndex];
            }
            
            character.CharacterTransform.position = CurrentPathPoint.Position;
            
            #if UNITY_EDITOR
            if (_testLine == null)
            {
                _testLine = GameObject
                    .Instantiate(new GameObject("LineRenderer"), null)
                    .AddComponent<TestLineRenderer>();
            }
            #endif
        }

        public override void OnStateSyncUpdate()
        {
            if (_progressPointIndex >= _currentCharacterPathNode.PathPointIndexes.Length)
            {
                aiStateMachineController.SwitchState<SGTargetSearchTargetState>();
                return;
            }

            if (Vector3.Distance(character.CharacterTransform.position, CurrentPathPoint.Position) < MINIMAL_DISTANCE_FOR_CHECK)
            {
                _progressPointIndex++;
                return;
            }
            
            #if UNITY_EDITOR
            if (_testLine != null)
            {
                _testLine.UpdateData(character.CharacterTransform.position, CurrentPathPoint.Position);
            }
            #endif
            
            Vector3 direction = CurrentPathPoint.Position - character.CharacterTransform.position;
            CharacterMovementComponent.Move(direction, false);
            CharacterMovementComponent.Rotation(direction);
        }

        public override void OnStateAsyncUpdate() { }

        public override void OnStateEnd() { }
    }
}