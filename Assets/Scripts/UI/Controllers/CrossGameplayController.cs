using Arenar.Character;
using Arenar.Services.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace Arenar.UI
{
    public class CrossGameplayController : CanvasWindowController, ITickable
    {
        private TickableManager tickableManager;
        private PlayerCharacterSpawnController playerCharacterSpawnController;
        
        private Image crossImage;
        private RectTransform crossRT;

        private ComponentCharacterController component;
        private ICharacterAimComponent characterAimComponent;

        private CrossCanvasWindowLayer crossGameplayCanvasLayer;


        private ComponentCharacterController Character
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

        private ICharacterAimComponent CharacterAimComponent
        {
            get
            {
                if (characterAimComponent == null)
                {
                    if (Character == null)
                        return null;
                    Character.TryGetCharacterComponent<ICharacterAimComponent>(out characterAimComponent);
                }

                return characterAimComponent;
            }
        }


        [Inject]
        public void Construct(PlayerCharacterSpawnController playerCharacterSpawnController,
                              TickableManager tickableManager)
        {
            this.playerCharacterSpawnController = playerCharacterSpawnController;
            this.tickableManager = tickableManager;
        }

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            crossGameplayCanvasLayer = base.canvasService
                .GetWindow<GameplayCanvasWindow>()
                .GetWindowLayer<CrossCanvasWindowLayer>();

            crossImage = crossGameplayCanvasLayer.CrossImage;
            crossRT = crossGameplayCanvasLayer.CrossRT;
            
            tickableManager.Add(this);
        }

        private void SetCrossScale(float scale) =>
            crossRT.localScale = new Vector3(scale, scale, 1);

        private void SetCrossVisibleStatus(bool status) =>
            crossImage.gameObject.SetActive(status);

        public void Tick()
        {
            if (CharacterAimComponent != null)
                SetCrossVisibleStatus(CharacterAimComponent.IsAim);
        }
    }
}