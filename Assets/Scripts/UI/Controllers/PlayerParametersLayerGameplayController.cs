using Arenar.Character;
using Arenar.Services.PlayerInputService;
using UnityEngine;
using Arenar.Services.LevelsService;


namespace Arenar.Services.UI
{
    public class PlayerParametersLayerGameplayController : CanvasWindowController
    {
        private CharacterSpawnController characterSpawnController;
        private ICharacterLiveComponent playerCharacterLiveComponent;
        private ICharacterProgressionComponent playerCharacterProgressionComponent;
        
        private ICharacterEntity playerCharacterController;
        private GameplayPlayerParametersWindowLayer gameplayPlayerParametersWindowLayer;
        private PlayerCharacterSkillUpgradeService playerCharacterSkillUpgradeService;
        private ILevelsService levelsService;


        private ICharacterEntity Character
        {
            get
            {
                if (playerCharacterController == null)
                {
                    playerCharacterController = characterSpawnController.PlayerCharacter;
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
        
        
        public PlayerParametersLayerGameplayController(CharacterSpawnController characterSpawnController,
                                                       IPlayerInputService playerInputService,
                                                       ILevelsService levelsService,
                                                       PlayerCharacterSkillUpgradeService playerCharacterSkillUpgradeService) : base(playerInputService)
        {
            this.characterSpawnController = characterSpawnController;
            this.levelsService = levelsService;
            this.playerCharacterSkillUpgradeService = playerCharacterSkillUpgradeService;
        }

        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            gameplayPlayerParametersWindowLayer = base.canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<GameplayPlayerParametersWindowLayer>();

            gameplayPlayerParametersWindowLayer.GetComponent<Canvas>().enabled = true;

            characterSpawnController.OnCreatePlayerCharacter += OnInstallNewCharacter;
            
            gameplayPlayerParametersWindowLayer.OpenUpgradeSkillsMenuButton.onClick.AddListener(OpenUpgradeSkillsMenuButtonHandler);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            if (levelsService.CurrentLevelContext.GameMode != GameMode.Survival)
            {
                gameplayPlayerParametersWindowLayer.OpenUpgradeSkillsMenuButton.gameObject.SetActive(false);
                return;
            }
                
            gameplayPlayerParametersWindowLayer.OpenUpgradeSkillsMenuButton.gameObject.SetActive(true);
            OpenUpgradeSkillsMenuButtonHandler();
            playerCharacterSkillUpgradeService.OnUpgradeScoreCountChange += UpdateScoreCountHandler;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            playerCharacterSkillUpgradeService.OnUpgradeScoreCountChange -= UpdateScoreCountHandler;
        }

        private void OnInstallNewCharacter(ICharacterEntity characterController)
        {
            playerCharacterController = characterController;
            PlayerCharacterLiveComponent.OnCharacterChangeHealthValue += CharacterChangeHealthValueHandler;

            if (PlayerCharacterLiveComponent.HealthContainer != null)
            {
                CharacterChangeHealthValueHandler(
                    PlayerCharacterLiveComponent.HealthContainer.Health,
                    PlayerCharacterLiveComponent.HealthContainer.HealthMax
                );
            }
        }
        
        private void OpenUpgradeSkillsMenuButtonHandler()
        {
            gameplayPlayerParametersWindowLayer.UpgradeSkillsCountText.text = playerCharacterSkillUpgradeService.UpgradeScore.ToString();
            gameplayPlayerParametersWindowLayer.OpenUpgradeSkillsMenuButton.interactable = playerCharacterSkillUpgradeService.UpgradeScore > 0;
        }
        
        private void UpdateScoreCountHandler()
        {
            canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        GameplayCanvasWindow,
                        UpgradeCharacterWindow>
                    (false, false, null);
        }

        private void CharacterChangeHealthValueHandler(int health, int healthMax) =>
            gameplayPlayerParametersWindowLayer.UpdatePlayerHealth(health, healthMax);
    }
}