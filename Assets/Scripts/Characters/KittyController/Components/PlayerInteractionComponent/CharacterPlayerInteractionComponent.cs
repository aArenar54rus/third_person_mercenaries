using UnityEngine;
using Zenject;


namespace CatSimulator.Character
{
    public class CharacterPlayerInteractionComponent : ICharacterPlayerInteractionComponent, ITickable
    {
        private const float CLICK_OFFSET = 0.15f;
        
        
        private enum ClickPosition
        {
            Up = 0,
            Middle = 1,
            Low = 2,
        }
        

        private ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation,
            CharacterAnimationComponent.KittyAnimationValue> _characterAnimationComponent;

        private ICharacterEntity kittyController;
        private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        private PlayerCharacterParametersData playerCharacterParameters;
        private TickableManager tickableManager;
        private Camera camera;
        private bool isInteractible;


        private ICharacterLiveComponent characterLiveComponent =>
            kittyController.TryGetCharacterComponent<ICharacterLiveComponent>(out bool isSuccess);
        

        private ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation,
            CharacterAnimationComponent.KittyAnimationValue> CharacterAnimationComponent
        {
            get
            {
                if (_characterAnimationComponent != null)
                    return _characterAnimationComponent;

                var component =
                    kittyController.TryGetCharacterComponent<ICharacterAnimationComponent>(out bool isSuccess);
                if (isSuccess)
                {
                    if (component is ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation,
                        CharacterAnimationComponent.KittyAnimationValue> animationComponent)
                    {
                        _characterAnimationComponent = animationComponent;
                        return animationComponent;
                    }
                }
                
                return null;
            }
        }


        private Transform kittyTransform =>
            characterPhysicsDataStorage.CharacterTransform;
        
        
        [Inject]
        public void Construct(ICharacterEntity kittyController,
            ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
            PlayerCharacterParametersData playerCharacterParameters,
            TickableManager tickableManager,
            Camera camera)
        {
            this.characterPhysicsDataStorage = characterPhysicsDataStorage.Data;
            this.kittyController = kittyController;
            this.playerCharacterParameters = playerCharacterParameters;
            this.tickableManager = tickableManager;
            this.camera = camera;
        }
        
        public void Initialize()
        {
            tickableManager.Add(this);
        }

        public void DeInitialize()
        {
            tickableManager.Remove(this);
        }

        public void OnStart()
        {
            isInteractible = true;
        }

        public void OnUpdate()
        {

        }

        public void Tick()
        {
            if (!characterLiveComponent.IsAlive)
                return;

            OnUpdate();
        }
    }
}