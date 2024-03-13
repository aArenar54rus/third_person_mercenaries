using DG.Tweening;
using UnityEngine;

namespace Arenar.Services.LevelsService
{
    public class ShootingGalleryGameModeController : GameModeController
    {
        private const int GAME_WAIT_TIME = 5;
        
        
        private int _progressIndex = 0;
        private float _gameTime = 0.0f;
        private bool _isGameWork = false;

        private CharacterSpawnController _сharacterSpawnController;
        private ShootingGalleryTargetNode[] _shootingGalleryTargets;

        private Tween _tween;

        
        public ShootingGalleryGameModeController(ShootingGalleryTargetNode[] shootingGalleryTargets,
            CharacterSpawnController сharacterSpawnController)
        {
            _progressIndex = 0;
            _gameTime = 0.0f;
            _isGameWork = false;
            
            _shootingGalleryTargets = shootingGalleryTargets;
            _сharacterSpawnController = сharacterSpawnController;
        }
        
        
        public override void StartGame()
        {
            _tween = DOVirtual.DelayedCall(GAME_WAIT_TIME, () => _isGameWork = true)
                .OnComplete(KillTween);
        }

        public override void EndGame()
        {
            KillTween();
        }

        public void OnUpdate()
        {
            if (!_isGameWork)
                return;
            
            _gameTime += Time.deltaTime;
            
            if (_progressIndex >= _shootingGalleryTargets.Length)
            {
                if (_gameTime < GAME_WAIT_TIME)
                    return;

                _isGameWork = false;
                onGameComplete?.Invoke();

                return;
            }
            
            if (_shootingGalleryTargets[_progressIndex].ActivateTime < _gameTime)
                return;

            ActivateShootTarget(_shootingGalleryTargets[_progressIndex]);
            _progressIndex++;

            if (_progressIndex >= _shootingGalleryTargets.Length)
                _gameTime = 0;
        }

        private void ActivateShootTarget(ShootingGalleryTargetNode targetNode)
        {
            foreach (var entity in CharacterEntities)
            {
                ShootingGalleryTargetCharacterController
                    targetEntity = (ShootingGalleryTargetCharacterController)entity;
                if (targetEntity.gameObject.activeSelf)
                    continue;

                Spawn(targetEntity);
                return;
            }
            
            ShootingGalleryTargetCharacterController target =
                _сharacterSpawnController.CreateShootingGalleryTarget();

            CharacterEntities.Add(target);
        }

        public void Spawn(ShootingGalleryTargetCharacterController target)
        {
            target.ReInitialize();
        }

        private void KillTween()
        {
            _tween?.Kill(false);
            _tween = null;
        }
    }
}