using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Arenar.Services.InventoryService;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


namespace Arenar
{
    public class InventoryCellController : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        [Header("Cell")] [FormerlySerializedAs("containerImage")] [SerializeField]
        private Image _containerImage = default;

        [FormerlySerializedAs("iconImage")] [SerializeField]
        private Image _iconImage = default;

        [FormerlySerializedAs("countText")] [SerializeField]
        private TMP_Text _countText = default;
        
        private IInventoryService inventoryService;
        private PlayerInput playerInput;

        
        public int CellIndex { get; private set; }
        
        public PlayerInput PlayerInput
        {
            get
            {
                playerInput ??= new PlayerInput();
                return playerInput;
            }
        }

        
        public void Initialize(int cellIndex)
        {
            rectTransform ??= gameObject.GetComponent<RectTransform>();
            PlayerInput.Player.Enable();
            CellIndex = cellIndex;
            
            var devices =  InputSystem.devices;
            foreach (var device in devices)
            {
                Debug.Log("devices " + device);
                Debug.Log("devices " + device.lastUpdateTime);
            }

            var devices2 = InputUser.GetUnpairedInputDevices();
            foreach (var device in devices2)
            {
                Debug.Log("devices2 " + device);
                Debug.Log("devices2 " + device.lastUpdateTime);
            }
        }

        public void SetItem(InventoryItemData inventoryItemData)
        {
            _containerImage.raycastTarget = true;

            _iconImage.enabled = true;
            _iconImage.sprite = inventoryItemData.itemData.Icon;

            _countText.enabled = inventoryItemData.itemData.CanStack;
            _countText.text = $"{inventoryItemData.elementsCount}/{inventoryItemData.itemData.StackCountMax}";
        }

        public void SetEmpty()
        {
            _containerImage.raycastTarget = false;
            _iconImage.enabled = false;
            _countText.enabled = false;
        }
        
        private void Update()
        {
            Vector2 mousePosition = PlayerInput.Player.MousePosition.ReadValue<Vector2>();
            Vector2 mousePosition2 = Mouse.current.position.ReadValue();
            // Debug.Log(mousePosition + "  " + mousePosition2);
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition))
            {
                Debug.Log($"Курсор мыши находится над UI элементом {gameObject.name}!");
            }
        }
    }
}