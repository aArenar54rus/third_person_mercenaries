using Arenar.CameraService;
using Arenar.Character;
using DG.Tweening;
using UnityEngine;

namespace Arenar.Services.LevelsService
{
    public class ClearLocationGameModeController : GameModeController
    {
        private const int GAME_WAIT_START_TIME = 5;
        private const int GAME_WAIT_END_TIME = 3;
        
        
        private int _progressIndex = 0;
        private float _gameTime = 0.0f;
        private bool _isGameWork = false;

        private CharacterSpawnController _сharacterSpawnController;
        private ICameraService _cameraService;
        private ShootingGalleryTargetNode[] _shootingGalleryTargets;

        private int _killedEnemyCounter = 0;
        private Tween _tween;

        
        public ClearLocationGameModeController(CharacterSpawnController сharacterSpawnController, ICameraService cameraService)
        {
            _progressIndex = 0;
            _gameTime = 0.0f;
            _killedEnemyCounter = 0;
            _isGameWork = false;
            
            _сharacterSpawnController = сharacterSpawnController;
            _cameraService = cameraService;
        }


        public void Initialize(ShootingGalleryTargetNode[] shootingGalleryTargets)
        {
            _shootingGalleryTargets = shootingGalleryTargets;
        }

        public override void StartGame()
        {
            var playerCharacter = _сharacterSpawnController.GetCharacter(CharacterTypeKeys.Player);
            playerCharacter.CharacterTransform.position = new Vector3(3, 0, 0); 
            playerCharacter.CharacterTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            
            if (playerCharacter is PhysicalHumanoidComponentCharacterController player)
            {
                _cameraService.SetCameraState<CameraStateThirdPerson>(player.CameraTransform,
                    player.CharacterTransform);
                _cameraService.SetCinemachineVirtualCamera(CinemachineCameraType.DefaultTPS);
            }
            
            playerCharacter.Activate();

            _tween = DOVirtual.DelayedCall(GAME_WAIT_END_TIME, () => _isGameWork = true)
                .OnComplete(KillTween);
        }

        public override void EndGame()
        {
            KillTween();
        }

        public override void OnUpdate()
        {
            if (!_isGameWork)
                return;
            
            _gameTime += Time.deltaTime;

            if (_shootingGalleryTargets == null || _shootingGalleryTargets.Length == 0)
                return;
            
            if (_shootingGalleryTargets[_progressIndex].ActivateTime > _gameTime)
                return;

            ActivateShootTarget();
            _progressIndex++;

            if (_progressIndex >= _shootingGalleryTargets.Length)
                _isGameWork = false;
        }

        private void ActivateShootTarget()
        {
            var shootingGalleryTargetNpc = _сharacterSpawnController.CreateShootingGalleryTarget();
            shootingGalleryTargetNpc.InitializeShooterGalleryTarget(_progressIndex, GetRandomEnemyLevel());
            shootingGalleryTargetNpc.Activate();

            if (shootingGalleryTargetNpc.TryGetCharacterComponent<ICharacterLiveComponent>(out ICharacterLiveComponent characterLiveComponent))
            {
                characterLiveComponent.OnCharacterDie += OnEnemyCharacterDie;
            }
        }

        private void OnEnemyCharacterDie(ICharacterEntity enemyEntity)
        {
            if (enemyEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out ICharacterLiveComponent characterLiveComponent))
            {
                characterLiveComponent.OnCharacterDie -= OnEnemyCharacterDie;
            }

            _killedEnemyCounter++;
            if (_killedEnemyCounter >= _shootingGalleryTargets.Length)
            {
                _isGameWork = false;
                _tween = DOVirtual.DelayedCall(GAME_WAIT_END_TIME, () => { onGameComplete?.Invoke(); });
            }
        }

        private void KillTween()
        {
            _tween?.Kill(false);
            _tween = null;
        }
    }
}