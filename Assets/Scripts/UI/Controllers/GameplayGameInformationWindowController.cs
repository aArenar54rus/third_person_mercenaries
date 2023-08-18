using Arenar.Character;
using Arenar.UI;
using UnityEngine;
using Zenject;


namespace Arenar.Services.UI
{
    public class GameplayGameInformationWindowController : CanvasWindowController, ITickable
    {
        private PlayerCharacterSpawnController playerCharacterSpawnController;
        private GameplayInformationLayer gameplayInformationLayer;
        private ICharacterRayCastComponent playerCharacterRaycastComponent;
        private ComponentCharacterController component;

        private ComponentCharacterController characterOnCross;
        private ICharacterDescriptionComponent descriptionComponent = null;
        private ICharacterLiveComponent characterLiveComponent = null;
        
        
        private ComponentCharacterController PlayerCharacter
        {
            get
            {
                if (component == null)
                {
                    component = playerCharacterSpawnController.Component;
                    if (component == null)
                        return null;
                }

                return component;
            }
        }

        private ICharacterRayCastComponent PlayerCharacterRaycastComponent
        {
            get
            {
                if (playerCharacterRaycastComponent == null)
                    PlayerCharacter.TryGetCharacterComponent(out playerCharacterRaycastComponent);

                return playerCharacterRaycastComponent;
            }
        }
        
        
        [Inject]
        public void Construct(TickableManager tickableManager,
            PlayerCharacterSpawnController playerCharacterSpawnController)
        {
            this.playerCharacterSpawnController = playerCharacterSpawnController;
            tickableManager.Add(this);
        }

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            gameplayInformationLayer = base.canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<GameplayInformationLayer>();

            gameplayInformationLayer.GetComponent<Canvas>().enabled = true;
            gameplayInformationLayer.EnemyTargetInformationPanel.UnsetEnemy();
        }
        
        public void Tick()
        {
            InteractableElement interactableElement = PlayerCharacterRaycastComponent.InteractableElementsOnCross;
            gameplayInformationLayer.InformationText.enabled = interactableElement != null;

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
                    characterLiveComponent.OnCharacterDie += OnEnemyCharacterDie;
                }
                else
                {
                    OnEnemyCharacterDie();
                }
            }
            else
            {
                OnEnemyCharacterDie();
            }
        }

        private void OnEnemyCharacterChangeHealthValue(int health, int healthMax) =>
            gameplayInformationLayer.EnemyTargetInformationPanel.UpdateEnemyHealth(health, healthMax);

        private void OnEnemyCharacterDie()
        {
            gameplayInformationLayer.EnemyTargetInformationPanel.UnsetEnemy();
            
            if (characterOnCross == null)
                return;
            
            characterLiveComponent.OnCharacterChangeHealthValue -= OnEnemyCharacterChangeHealthValue;
            characterLiveComponent.OnCharacterDie -= OnEnemyCharacterDie;
            characterOnCross = null;
        }
    }
}