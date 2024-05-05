using Arenar.Character;
using Arenar.Services.PlayerInputService;
using UnityEngine;


namespace Arenar.Services.UI
{
    public class PlayerHealthVisualGameplayController : CanvasWindowController
    {
        private CharacterSpawnController _characterSpawnController;
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
                    playerCharacterController = _characterSpawnController.PlayerCharacter;
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
        
        
        public PlayerHealthVisualGameplayController(CharacterSpawnController characterSpawnController,
            IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            this._characterSpawnController = characterSpawnController;
        }

        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            gameplayPlayerParametersWindowLayer = base._canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<GameplayPlayerParametersWindowLayer>();

            gameplayPlayerParametersWindowLayer.GetComponent<Canvas>().enabled = true;

            _characterSpawnController.OnCreatePlayerCharacter += OnInstallNewCharacter;
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            //throw new System.NotImplementedException();
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            //throw new System.NotImplementedException();
        }

        private void OnInstallNewCharacter(ComponentCharacterController characterController)
        {
            playerCharacterController = characterController;
            PlayerCharacterLiveComponent.OnCharacterChangeHealthValue += OnCharacterChangeHealthValue;

            OnCharacterChangeHealthValue(PlayerCharacterLiveComponent.Health, PlayerCharacterLiveComponent.HealthMax);
        }

        private void OnCharacterChangeHealthValue(int health, int healthMax) =>
            gameplayPlayerParametersWindowLayer.UpdatePlayerHealth(health, healthMax);
    }
}