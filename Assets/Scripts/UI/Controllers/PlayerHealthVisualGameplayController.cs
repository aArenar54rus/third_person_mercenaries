using Arenar.Character;
using Arenar.UI;
using UnityEngine;
using Zenject;


namespace Arenar.Services.UI
{
    public class PlayerHealthVisualGameplayController : CanvasWindowController
    {
        private PlayerCharacterSpawnController playerCharacterSpawnController;
        private ICharacterLiveComponent playerCharacterLiveComponent;
        private ComponentCharacterController component;
        private GameplayPlayerParametersWindowLayer gameplayPlayerParametersWindowLayer;


        private ComponentCharacterController Component
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

        private ICharacterLiveComponent PlayerCharacterLiveComponent
        {
            get
            {
                if (playerCharacterLiveComponent == null)
                {
                    if (Component == null)
                        return null;
                    Component.TryGetCharacterComponent<ICharacterLiveComponent>(out playerCharacterLiveComponent);
                }

                return playerCharacterLiveComponent;
            }
        }
        
        
        [Inject]
        public void Construct(PlayerCharacterSpawnController playerCharacterSpawnController)
        {
            this.playerCharacterSpawnController = playerCharacterSpawnController;
        }


        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            gameplayPlayerParametersWindowLayer = base.canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<GameplayPlayerParametersWindowLayer>();

            gameplayPlayerParametersWindowLayer.GetComponent<Canvas>().enabled = true;

            playerCharacterSpawnController.OnCreatePlayerCharacter += OnInstallNewPlayerCharacter;
        }

        private void OnInstallNewPlayerCharacter(ComponentCharacterController componentCharacter)
        {
            PlayerCharacterLiveComponent.OnCharacterChangeHealthValue += OnCharacterChangeHealthValue;
            OnCharacterChangeHealthValue(PlayerCharacterLiveComponent.Health, PlayerCharacterLiveComponent.HealthMax);
        }

        private void OnCharacterChangeHealthValue(int health, int healthMax) =>
            gameplayPlayerParametersWindowLayer.UpdatePlayerHealth(health, healthMax);
    }
}