using Arenar.Character;
using Arenar.Services.UI;
using UnityEngine;
using Zenject;


namespace Arenar.UI
{
    public class PlayerHealthVisualGameplayController : CanvasWindowController
    {
        private PlayerCharacterSpawnController playerCharacterSpawnController;
        private ICharacterLiveComponent playerCharacterLiveComponent;
        private PlayerCharacterController player;
        private HealthGameplayWindowLayer healthGameplayWindowLayer;


        private PlayerCharacterController Player
        {
            get
            {
                if (player == null)
                {
                    player = playerCharacterSpawnController.Player;
                    if (player == null)
                        return null;
                }

                return player;
            }
        }

        private ICharacterLiveComponent PlayerCharacterLiveComponent
        {
            get
            {
                if (playerCharacterLiveComponent == null)
                {
                    if (Player == null)
                        return null;

                    playerCharacterLiveComponent = Player.TryGetCharacterComponent<ICharacterLiveComponent>(out bool success);

                    if (!success)
                        return null;
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

            healthGameplayWindowLayer = base.canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<HealthGameplayWindowLayer>();

            healthGameplayWindowLayer.GetComponent<Canvas>().enabled = true;

            playerCharacterSpawnController.OnCreatePlayerCharacter += OnInstallNewPlayerCharacter;
        }

        private void OnInstallNewPlayerCharacter(PlayerCharacterController playerCharacter)
        {
            PlayerCharacterLiveComponent.OnCharacterChangeHealthValue += OnCharacterChangeHealthValue;
            OnCharacterChangeHealthValue(PlayerCharacterLiveComponent.Health, PlayerCharacterLiveComponent.HealthMax);
        }

        private void OnCharacterChangeHealthValue(int health, int healthMax)
        {
            healthGameplayWindowLayer.HealthText.text = health + "/" + healthMax;

            healthGameplayWindowLayer.HealthSlider.maxValue = healthMax;
            healthGameplayWindowLayer.HealthSlider.value = health;
        }
    }
}