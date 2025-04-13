using Arenar.Services.InventoryService;
using Zenject;


namespace Arenar.Character
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
        
        
        private ICharacterEntity character;
        private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        private PlayerCharacterParametersData playerCharacterParameters;
        private IInventoryService inventorySystem;
        private TickableManager tickableManager;
        private bool isInteractible;
        
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation,
            CharacterAnimationComponent.AnimationValue> _characterAnimationComponent;
        private ICharacterLiveComponent _liveComponent;
        private ICharacterRayCastComponent _rayCastComponent;
        private ICharacterInputComponent _inputComponent;


        public bool CanInteract { get; private set; }


        [Inject]
        public void Construct(ICharacterEntity kittyController,
            ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
            PlayerCharacterParametersData playerCharacterParameters,
            TickableManager tickableManager,
            IInventoryService inventorySystem)
        {
            this.characterPhysicsDataStorage = characterPhysicsDataStorage.Data;
            this.character = kittyController;
            this.playerCharacterParameters = playerCharacterParameters;
            this.inventorySystem = inventorySystem;
            this.tickableManager = tickableManager;
        }
        
        public void Initialize()
        {
            character.TryGetCharacterComponent<ICharacterLiveComponent>(out _liveComponent);
            character.TryGetCharacterComponent<ICharacterRayCastComponent>(out _rayCastComponent);
            character.TryGetCharacterComponent<ICharacterInputComponent>(out _inputComponent);
            
            if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animationComponent))
            {
                if (animationComponent is ICharacterAnimationComponent<CharacterAnimationComponent.Animation,
                    CharacterAnimationComponent.AnimationValue> neededComponent)
                {
                    _characterAnimationComponent = neededComponent;
                }
            }
        }

        public void DeInitialize()
        {
            
        }

        public void OnActivate()
        {
            isInteractible = true;
            tickableManager.Add(this);
            CanInteract = true;
        }

        public void OnDeactivate()
        {
            tickableManager.Remove(this);
            CanInteract = false;
        }

        public void Tick()
        {
            if (!_liveComponent.IsAlive)
                return;

            InteractableElement element = _rayCastComponent.InteractableElementsOnCross;

            /*
            if (element != null && _inputComponent.InteractAction)
                InteractWithInteractObject(element);*/
        }
        
        public void InteractWithInteractObject(InteractableElement element)
        {
            if (element.InteractableElementType != "Item")
                return;

            if (element is not ItemInteractableElement itemElement)
                return;

            if (!inventorySystem.TryAddItems(itemElement.ItemInventoryData, itemElement.Count,
                    out InventoryItemCellData inventoryItemData))
                return;

            if (inventoryItemData != null)
            {
                itemElement.SetItem(inventoryItemData.itemInventoryData, inventoryItemData.elementsCount);
            }
            else
            {
                element.gameObject.SetActive(false);
            }
        }
    }
}