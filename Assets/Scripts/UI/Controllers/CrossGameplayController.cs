using Arenar.Character;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace Arenar.Services.UI
{
    public class CrossGameplayController : CanvasWindowController, ITickable
    {
        private TickableManager tickableManager;
        private TestCharacterSpawnController testCharacterSpawnController;
        
        private Image crossImage;
        private RectTransform crossRT;

        private CrossCanvasWindowLayer crossGameplayCanvasLayer;

        private ComponentCharacterController playerCharacter;
        private ICharacterAimComponent characterAimComponent;


        private ICharacterAimComponent CharacterAimComponent
        {
            get
            {
                if (characterAimComponent == null)
                {
                    if (playerCharacter == null)
                        return null;
                    playerCharacter.TryGetCharacterComponent<ICharacterAimComponent>(out characterAimComponent);
                }

                return characterAimComponent;
            }
        }


        [Inject]
        public void Construct(TestCharacterSpawnController testCharacterSpawnController,
                              TickableManager tickableManager)
        {
            this.testCharacterSpawnController = testCharacterSpawnController;
            this.tickableManager = tickableManager;
        }

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            crossGameplayCanvasLayer = base._canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<CrossCanvasWindowLayer>();

            crossImage = crossGameplayCanvasLayer.CrossImage;
            crossRT = crossGameplayCanvasLayer.CrossRT;

            testCharacterSpawnController.OnCreatePlayerCharacter += OnCreatePlayerCharacter;
            tickableManager.Add(this);
        }

        private void SetCrossVisibleStatus(bool status) =>
            crossImage.gameObject.SetActive(status);

        public void Tick()
        {
            if (playerCharacter == null)
                return;
            
            if (CharacterAimComponent != null)
                SetCrossVisibleStatus(CharacterAimComponent.IsAim);
        }

        private void OnCreatePlayerCharacter(ComponentCharacterController playerCharacter)
        {
            this.playerCharacter = playerCharacter;
        }
    }
}