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
        private ICharacterLiveComponent _characterLiveComponent = null;
        private ICharacterAimComponent _characterAimComponent = null;
        private ICharacterAttackComponent _characterAttackComponent;
        private ComponentCharacterController _playerCharacter;
        

        private ICharacterRayCastComponent PlayerCharacterRaycastComponent
        {
            get
            {
                if (_playerCharacterRaycastComponent == null)
                    _playerCharacter.TryGetCharacterComponent(out _playerCharacterRaycastComponent);

                return _playerCharacterRaycastComponent;
            }
        }

        private ICharacterAimComponent PlayerCharacterAimComponent
        {
            get
            {
                if (_characterAimComponent == null)
                    _playerCharacter.TryGetCharacterComponent(out _characterAimComponent);

                return _characterAimComponent;
            }
        }

        private ICharacterAttackComponent PlayerCharacterAttackComponent
        {
            get
            {
                if (_characterAttackComponent == null)
                    _playerCharacter.TryGetCharacterComponent(out _characterAttackComponent);

                return _characterAttackComponent;
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

            _gameplayCanvasWindow = _canvasService
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
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            /*PlayerCharacterAttackComponent.onReloadStart -= EnableProgressSlider;
            PlayerCharacterAttackComponent.onReloadEnd -= DisableProgressSlider;
            PlayerCharacterAttackComponent.onReloadProgress -= UpdateProgressSlider;
            
            PlayerCharacterAttackComponent.onUpdateWeaponClipSize -= OnUpdateWeaponClipSize;*/
            
            return;
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

        private void OnUpdateWeaponClipSize(int clipSize, int clipSizeMax, bool isPercent)
        {
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
                                       && enemyCharacterController.TryGetCharacterComponent(out _characterLiveComponent));

                if (isEnemyOnCross && _characterLiveComponent.IsAlive)
                {
                    var locName = LocalizationManager.GetTranslation(_descriptionComponent.CharacterName);
                    var locDesc = LocalizationManager.GetTranslation(_descriptionComponent.CharacterDescription);
                    _gameplayInformationLayer.EnemyTargetInformationPanel.SetEnemy(locName, locDesc);
                    OnEnemyCharacterChangeHealthValue(_characterLiveComponent.Health, _characterLiveComponent.HealthMax);
                    
                    _characterLiveComponent.OnCharacterChangeHealthValue += OnEnemyCharacterChangeHealthValue;
                    _characterLiveComponent.OnCharacterDie += OnEnemyCharacterInfoDisable;
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

        private void OnEnemyCharacterChangeHealthValue(int health, int healthMax)
        {
            Debug.LogError($"{health} / {healthMax}");
            _gameplayInformationLayer.EnemyTargetInformationPanel.UpdateEnemyHealth(health, healthMax, false);
        }

        private void OnEnemyCharacterInfoDisable()
        {
            _gameplayInformationLayer.EnemyTargetInformationPanel.UnsetEnemy();
            
            if (_characterOnCross == null)
                return;
            
            _characterLiveComponent.OnCharacterChangeHealthValue -= OnEnemyCharacterChangeHealthValue;
            _characterLiveComponent.OnCharacterDie -= OnEnemyCharacterInfoDisable;
            _characterOnCross = null;
        }

        private void OnCreatePlayerCharacter(ComponentCharacterController playerCharacterController)
        {
            _playerCharacter = playerCharacterController;
            
            PlayerCharacterAttackComponent.onReloadStart += EnableProgressSlider;
            PlayerCharacterAttackComponent.onReloadEnd += DisableProgressSlider;
            PlayerCharacterAttackComponent.onReloadProgress += UpdateProgressSlider;

            PlayerCharacterAttackComponent.onUpdateWeaponClipSize += OnUpdateWeaponClipSize;
        }
    }
}