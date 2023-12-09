using System;
using UnityEngine;
using UnityEngine.UI;
using Arenar.Services.InventoryService;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


namespace Arenar.Services.UI
{
    public class InventoryCellController : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<int> onCellSelected;
        public Action<int> onCellDeselected;
        public Action<int> onCellClicked;
        
        
        [SerializeField] protected Button _cellButton;
        [SerializeField] protected RectTransform rectTransform;

        [Header("Cell")]
        [SerializeField] protected Image _containerImage = default;
        [SerializeField] protected Image _lightImage = default;
        [SerializeField] protected Image _iconImage = default;
        [SerializeField] protected RectTransform _lockIconTransform;

        [Space(10), Header("Data")]
        [SerializeField] protected ItemRarityColorData _colorsData;
        
        protected PlayerInput _playerInput;
        private bool _isCellEmpty;

        
        public int CellIndex { get; private set; }
        
        public PlayerInput PlayerInput
        {
            get
            {
                _playerInput ??= new PlayerInput();
                return _playerInput;
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
            
            _cellButton.onClick.AddListener(() => onCellClicked?.Invoke(CellIndex));
        }

        public virtual void SetItem(InventoryItemCellData inventoryItemCellData)
        {
            _isCellEmpty = false;
            _iconImage.enabled = true;
            _iconImage.sprite = inventoryItemCellData.itemInventoryData.Icon;
            _lightImage.color = _colorsData.ItemRarityColors[inventoryItemCellData.itemInventoryData.ItemRarity];
            _lockIconTransform.gameObject.SetActive(inventoryItemCellData.IsLocked);
        }

        public virtual void SetEmpty()
        {
            _isCellEmpty = true;
            _iconImage.enabled = false;
            _lightImage.color = Color.clear;
            _lockIconTransform.gameObject.SetActive(false);
        }

        public void Select() => _cellButton.Select();
        
        private void Update()
        {
            Vector2 mousePosition = PlayerInput.Player.MousePosition.ReadValue<Vector2>();
            Vector2 mousePosition2 = Mouse.current.position.ReadValue();
            // Debug.Log(mousePosition + "  " + mousePosition2);
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition))
            {
                Debug.Log($"Курсор мыши находится над UI элементом {gameObject.name}!");
                onCellSelected?.Invoke(CellIndex);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_isCellEmpty)
                return;
            Debug.LogError($"ON SELECT - ITEM INDEX {CellIndex}");
            onCellSelected?.Invoke(CellIndex);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Debug.LogError($"ON DESELECT - ITEM INDEX {CellIndex}");
            onCellDeselected?.Invoke(CellIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isCellEmpty)
                return;
            Debug.LogError($"ON POINTER INTER - ITEM INDEX {CellIndex}");
            onCellSelected?.Invoke(CellIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.LogError($"ON POINTER EXIT - ITEM INDEX {CellIndex}");
            onCellDeselected?.Invoke(CellIndex);
        }
    }
}