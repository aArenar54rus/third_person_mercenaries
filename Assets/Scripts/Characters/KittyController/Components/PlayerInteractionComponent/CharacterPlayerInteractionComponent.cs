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
        
        private ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation,
            CharacterAnimationComponent.KittyAnimationValue> _characterAnimationComponent;
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
            tickableManager.Add(this);
            
            bool isSuccess = false;
            character.TryGetCharacterComponent<ICharacterLiveComponent>(out _liveComponent);
            character.TryGetCharacterComponent<ICharacterRayCastComponent>(out _rayCastComponent);
            character.TryGetCharacterComponent<ICharacterInputComponent>(out _inputComponent);
            
            if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animationComponent))
            {
                if (animationComponent is ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation,
                    CharacterAnimationComponent.KittyAnimationValue> neededComponent)
                {
                    _characterAnimationComponent = neededComponent;
                }
            }


            CanInteract = true;
        }

        public void DeInitialize()
        {
            tickableManager.Remove(this);
        }

        public void OnStart()
        {
            isInteractible = true;
        }

        public void Tick()
        {
            if (!_liveComponent.IsAlive)
                return;

            InteractableElement element = _rayCastComponent.InteractableElementsOnCross;

            if (element != null && _inputComponent.InteractAction)
                InteractWithInteractObject(element);
        }
        
        public void InteractWithInteractObject(InteractableElement element)
        {
            if (element.InteractableElementType == "Item")
            {
                if (element is ItemInteractableElement itemElement)
                {
                    if (!inventorySystem.TryAddItems(itemElement.ItemData, itemElement.Count,
                        out InventoryItemData inventoryItemData))
                        return;

                    if (inventoryItemData != null)
                    {
                        itemElement.SetItem(inventoryItemData.itemData, inventoryItemData.elementsCount);
                    }
                    else
                    {
                        element.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}