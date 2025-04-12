using Arenar.Character;
using Arenar.Services.PlayerInputService;
using I2.Loc;
using UnityEngine;
using Zenject;


namespace Arenar.Services.UI
{
    public class GameplayGameInformationWindowController : CanvasWindowController, ITickable
    {
        private GameplayCanvasWindow _gameplayCanvasWindow;
        
        private CharacterSpawnController _characterSpawnController;
        private GameplayInformationLayer _gameplayInformationLayer;
        private ICharacterRayCastComponent _playerCharacterRaycastComponent;

        private ComponentCharacterController _characterOnCross;
        private ICharacterDescriptionComponent _descriptionComponent = null;
        private ICharacterEntity _playerCharacter;
        
        private ICharacterLiveComponent _enemyCharacterLiveComponent = null;
        

        private ICharacterRayCastComponent PlayerCharacterRaycastComponent
        {
            get
            {
                if (_playerCharacter == null)
                    return null;
                _playerCharacter.TryGetCharacterComponent(out ICharacterRayCastComponent playerCharacterRaycastComponent);
                return playerCharacterRaycastComponent;
            }
        }

        private ICharacterAimComponent PlayerCharacterAimComponent
        {
            get
            {
                if (_playerCharacter == null)
                    return null;
                _playerCharacter.TryGetCharacterComponent(out ICharacterAimComponent characterAimComponent);
                return characterAimComponent;
            }
        }

        private ICharacterAttackComponent PlayerCharacterAttackComponent
        {
            get
            {
                if (_playerCharacter == null)
                    return null;
                _playerCharacter.TryGetCharacterComponent(out ICharacterAttackComponent characterAttackComponent);
                return characterAttackComponent;
            }
        }
        
        
        public GameplayGameInformationWindowController(TickableManager tickableManager,
            CharacterSpawnController characterSpawnController,
            IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            _characterSpawnController = characterSpawnController;
            _playerInputService = playerInputService;
            
            tickableManager.Add(this);
        }

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _gameplayCanvasWindow = base.canvasService
                .GetWindow<GameplayCanvasWindow>();
            
            _gameplayInformationLayer = 
                _gameplayCanvasWindow.GetWindowLayer<GameplayInformationLayer>();

            _gameplayInformationLayer.GetComponent<Canvas>().enabled = true;
            _gameplayInformationLayer.EnemyTargetInformationPanel.UnsetEnemy();

            _gameplayCanvasWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _gameplayCanvasWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);

            _characterSpawnController.OnCreatePlayerCharacter += OnCreatePlayerCharacter;

            DisableProgressSlider();
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            DisableProgressSlider();

            var playerAttackComponent = PlayerCharacterAttackComponent;
            return;
            playerAttackComponent.onReloadStart += EnableProgressSlider;
            playerAttackComponent.onReloadEnd += DisableProgressSlider;
            playerAttackComponent.onReloadProgress += UpdateProgressSlider;
            playerAttackComponent.onUpdateWeaponClipSize += OnUpdateWeaponClipSize;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            var playerAttackComponent = PlayerCharacterAttackComponent;
            playerAttackComponent.onReloadStart -= EnableProgressSlider;
            playerAttackComponent.onReloadEnd -= DisableProgressSlider;
            playerAttackComponent.onReloadProgress -= UpdateProgressSlider;
            playerAttackComponent.onUpdateWeaponClipSize -= OnUpdateWeaponClipSize;
        }

        public void Tick()
        {
            if (_playerCharacter == null)
                return;
            
            TickUpdateTextMessage();
            TickUpdateEnemyTargetInfoPanel();
        }

        private void EnableProgressSlider()
        {
            _gameplayInformationLayer.ProgressBarController.SetProgressBarActive(true);
        }

        private void DisableProgressSlider()
        {
            _gameplayInformationLayer.ProgressBarController.SetProgressBarActive(false);
        }

        private void UpdateProgressSlider(float value, float valueMax)
        {
            _gameplayInformationLayer.ProgressBarController.SetBarValuePercent(value, valueMax);
        }

        private void OnUpdateWeaponClipSize(int clipSize, int clipSizeMax)
        {
            bool isPercent = false;
            if (isPercent)
                _gameplayInformationLayer.WeaponInfoPanel.UpdateWeaponClipSizeInfoPercent(clipSize, clipSizeMax);
            else
                _gameplayInformationLayer.WeaponInfoPanel.UpdateWeaponClipSizeInfo(clipSize, clipSizeMax);
        }

        private void TickUpdateTextMessage()
        {
            InteractableElement interactableElement = PlayerCharacterRaycastComponent.InteractableElementsOnCross;
            _gameplayInformationLayer.InformationText.enabled = interactableElement != null;
        }

        private void TickUpdateEnemyTargetInfoPanel()
        {
            if (!PlayerCharacterAimComponent.IsAim)
            {
                OnEnemyCharacterInfoDisable();
                return;
            }

            ComponentCharacterController enemyCharacterController = PlayerCharacterRaycastComponent.CharacterControllerOnCross;
            if (_characterOnCross == enemyCharacterController)
                return;

            if (_characterOnCross == null)
            {
                
                bool isEnemyOnCross = (enemyCharacterController != null
                    && enemyCharacterController.TryGetCharacterComponent(out _descriptionComponent)
                    && enemyCharacterController.TryGetCharacterComponent(out _enemyCharacterLiveComponent));

                if (isEnemyOnCross && _enemyCharacterLiveComponent.IsAlive)
                {
                    var locName = LocalizationManager.GetTranslation(_descriptionComponent.CharacterName);
                    var locDesc = LocalizationManager.GetTranslation(_descriptionComponent.CharacterDescription);
                    _gameplayInformationLayer.EnemyTargetInformationPanel.SetEnemy(locName, locDesc);
                    OnEnemyCharacterChangeHealthValue(_enemyCharacterLiveComponent.HealthContainer.Health,
                        _enemyCharacterLiveComponent.HealthContainer.HealthMax);
                    
                    _enemyCharacterLiveComponent.OnCharacterChangeHealthValue += OnEnemyCharacterChangeHealthValue;
                    _enemyCharacterLiveComponent.OnCharacterDie += OnEnemyCharacterInfoDisable;
                }
                else
                {
                    OnEnemyCharacterInfoDisable();
                }
            }
            else
            {
                OnEnemyCharacterInfoDisable();
            }
            
            _characterOnCross = enemyCharacterController;
        }

        private void OnEnemyCharacterChangeHealthValue(int health, int healthMax) =>
            _gameplayInformationLayer.EnemyTargetInformationPanel.UpdateEnemyHealth(health, healthMax, false);

        private void OnEnemyCharacterInfoDisable(ICharacterEntity characterEntity)
        {
            OnEnemyCharacterInfoDisable();
        }
        
        private void OnEnemyCharacterInfoDisable()
        {
            _gameplayInformationLayer.EnemyTargetInformationPanel.UnsetEnemy();
            
            if (_characterOnCross == null)
                return;
            if (_enemyCharacterLiveComponent == null)
                _characterOnCross.TryGetCharacterComponent<ICharacterLiveComponent>(out _enemyCharacterLiveComponent);
            
            _enemyCharacterLiveComponent.OnCharacterChangeHealthValue -= OnEnemyCharacterChangeHealthValue;
            _enemyCharacterLiveComponent.OnCharacterDie -= OnEnemyCharacterInfoDisable;

            _enemyCharacterLiveComponent = null;
            _characterOnCross = null;
        }

        private void OnCreatePlayerCharacter(ICharacterEntity playerCharacterController)
        {
            _playerCharacter = playerCharacterController;
        }
    }
}