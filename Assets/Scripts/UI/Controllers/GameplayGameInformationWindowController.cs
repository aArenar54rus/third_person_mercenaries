using Arenar.Character;
using Arenar.Services.PlayerInputService;
using UnityEngine;
using Zenject;


namespace Arenar.Services.UI
{
    public class GameplayGameInformationWindowController : CanvasWindowController, ITickable
    {
        private TestCharacterSpawnController _testCharacterSpawnController;
        private GameplayInformationLayer gameplayInformationLayer;
        private ICharacterRayCastComponent playerCharacterRaycastComponent;

        private ComponentCharacterController characterOnCross;
        private ICharacterDescriptionComponent descriptionComponent = null;
        private ICharacterLiveComponent characterLiveComponent = null;
        private ICharacterAimComponent characterAimComponent = null;
        private ComponentCharacterController playerCharacter;
        

        private ICharacterRayCastComponent PlayerCharacterRaycastComponent
        {
            get
            {
                if (playerCharacterRaycastComponent == null)
                    playerCharacter.TryGetCharacterComponent(out playerCharacterRaycastComponent);

                return playerCharacterRaycastComponent;
            }
        }

        private ICharacterAimComponent PlayerCharacterAimComponent
        {
            get
            {
                if (characterAimComponent == null)
                    playerCharacter.TryGetCharacterComponent(out characterAimComponent);

                return characterAimComponent;
            }
        }
        
        
        public GameplayGameInformationWindowController(TickableManager tickableManager,
            TestCharacterSpawnController testCharacterSpawnController,
            IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            _testCharacterSpawnController = testCharacterSpawnController;
            _playerInputService = playerInputService;
            
            tickableManager.Add(this);
        }

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            gameplayInformationLayer = _canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<GameplayInformationLayer>();

            gameplayInformationLayer.GetComponent<Canvas>().enabled = true;
            gameplayInformationLayer.EnemyTargetInformationPanel.UnsetEnemy();

            _testCharacterSpawnController.OnCreatePlayerCharacter += OnCreatePlayerCharacter;
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            return;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            return;
        }

        public void Tick()
        {
            if (playerCharacter == null)
                return;
            
            TickUpdateTextMessage();
            TickUpdateEnemyTargetInfoPanel();
        }

        private void TickUpdateTextMessage()
        {
            InteractableElement interactableElement = PlayerCharacterRaycastComponent.InteractableElementsOnCross;
            gameplayInformationLayer.InformationText.enabled = interactableElement != null;
        }

        private void TickUpdateEnemyTargetInfoPanel()
        {
            if (!PlayerCharacterAimComponent.IsAim)
            {
                OnEnemyCharacterInfoDisable();
                return;
            }

            ComponentCharacterController enemyCharacterController = PlayerCharacterRaycastComponent.CharacterControllerOnCross;
            if (characterOnCross == enemyCharacterController)
                return;

            if (characterOnCross == null)
            {
                bool isEnemyOnCross = (enemyCharacterController != null
                                       && enemyCharacterController.TryGetCharacterComponent(out descriptionComponent)
                                       && enemyCharacterController.TryGetCharacterComponent(out characterLiveComponent));

                if (isEnemyOnCross && characterLiveComponent.IsAlive)
                {
                    gameplayInformationLayer.EnemyTargetInformationPanel.SetEnemy(descriptionComponent.CharacterName, descriptionComponent.CharacterDescription);
                    OnEnemyCharacterChangeHealthValue(characterLiveComponent.Health, characterLiveComponent.HealthMax);
                    
                    characterLiveComponent.OnCharacterChangeHealthValue += OnEnemyCharacterChangeHealthValue;
                    characterLiveComponent.OnCharacterDie += OnEnemyCharacterInfoDisable;
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
            
            characterOnCross = enemyCharacterController;
        }

        private void OnEnemyCharacterChangeHealthValue(int health, int healthMax) =>
            gameplayInformationLayer.EnemyTargetInformationPanel.UpdateEnemyHealth(health, healthMax);

        private void OnEnemyCharacterInfoDisable()
        {
            gameplayInformationLayer.EnemyTargetInformationPanel.UnsetEnemy();
            
            if (characterOnCross == null)
                return;
            
            characterLiveComponent.OnCharacterChangeHealthValue -= OnEnemyCharacterChangeHealthValue;
            characterLiveComponent.OnCharacterDie -= OnEnemyCharacterInfoDisable;
            characterOnCross = null;
        }

        private void OnCreatePlayerCharacter(ComponentCharacterController playerCharacterController)
        {
            playerCharacter = playerCharacterController;
        }
    }
}