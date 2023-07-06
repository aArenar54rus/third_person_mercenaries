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

        private PlayerCharacterController player;
        private IAimComponent aimComponent;

        private CrossCanvasWindowLayer crossGameplayCanvasLayer;


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

        private IAimComponent AimComponent
        {
            get
            {
                if (aimComponent == null)
                {
                    if (Player == null)
                        return null;

                    aimComponent = Player.TryGetCharacterComponent<IAimComponent>(out bool success);

                    if (!success)
                        return null;
                }

                return aimComponent;
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
            if (AimComponent != null)
                SetCrossVisibleStatus(AimComponent.IsAim);
        }
    }
}