using Arenar.Character;
using Arenar.UI;
using UnityEngine;
using Zenject;


namespace Arenar.Services.UI
{
    public class PlayerHealthVisualGameplayController : CanvasWindowController
    {
        private TestCharacterSpawnController testCharacterSpawnController;
        private ICharacterLiveComponent playerCharacterLiveComponent;
        private ICharacterProgressionComponent playerCharacterProgressionComponent;
        
        private ComponentCharacterController playerCharacterController;
        private GameplayPlayerParametersWindowLayer gameplayPlayerParametersWindowLayer;


        private ComponentCharacterController Character
        {
            get
            {
                if (playerCharacterController == null)
                {
                    playerCharacterController = testCharacterSpawnController.PlayerCharacter;
                    if (playerCharacterController == null)
                        return null;
                }

                return playerCharacterController;
            }
        }

        private ICharacterLiveComponent PlayerCharacterLiveComponent
        {
            get
            {
                if (playerCharacterLiveComponent == null)
                {
                    if (Character == null)
                        return null;
                    Character.TryGetCharacterComponent<ICharacterLiveComponent>(out playerCharacterLiveComponent);
                }

                return playerCharacterLiveComponent;
            }
        }

        private ICharacterProgressionComponent PlayerCharacterProgressionComponent
        {
            get
            {
                if (playerCharacterProgressionComponent == null)
                {
                    if (Character == null)
                        return null;
                    Character.TryGetCharacterComponent<ICharacterProgressionComponent>(out playerCharacterProgressionComponent);
                }

                return playerCharacterProgressionComponent;
            }
        }
        
        
        [Inject]
        public void Construct(TestCharacterSpawnController testCharacterSpawnController)
        {
            this.testCharacterSpawnController = testCharacterSpawnController;
        }
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            gameplayPlayerParametersWindowLayer = base.canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<GameplayPlayerParametersWindowLayer>();

            gameplayPlayerParametersWindowLayer.GetComponent<Canvas>().enabled = true;

            testCharacterSpawnController.OnCreatePlayerCharacter += OnInstallNewTestCharacter;
        }

        private void OnInstallNewTestCharacter(ComponentCharacterController characterController)
        {
            playerCharacterController = characterController;
            
            PlayerCharacterLiveComponent.OnCharacterChangeHealthValue += OnCharacterChangeHealthValue;
            PlayerCharacterProgressionComponent.OnUpdateExperience += OnUpdateExperience;
            PlayerCharacterProgressionComponent.OnUpdateLevel += OnUpdateLevel;
            
            OnCharacterChangeHealthValue(PlayerCharacterLiveComponent.Health, PlayerCharacterLiveComponent.HealthMax);
            OnUpdateExperience(PlayerCharacterProgressionComponent.Experience, PlayerCharacterProgressionComponent.ExperienceMax);
            OnUpdateLevel(PlayerCharacterProgressionComponent.Level);
        }

        private void OnCharacterChangeHealthValue(int health, int healthMax) =>
            gameplayPlayerParametersWindowLayer.UpdatePlayerHealth(health, healthMax);

        private void OnUpdateExperience(int currentExp, int maxExp) =>
            gameplayPlayerParametersWindowLayer.UpdatePlayerExperience(currentExp, maxExp);

        private void OnUpdateLevel(int level) =>
            gameplayPlayerParametersWindowLayer.UpdatePlayerLevel(level);
    }
}