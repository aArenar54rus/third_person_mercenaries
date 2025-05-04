using System;
using UnityEngine;
using UnityEngine.UI;
using Arenar.Services.InventoryService;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


namespace Arenar.Services.UI
{
    public abstract class InventoryCellVisual : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
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
        private bool isCellEmpty;

        
        public int CellIndex { get; private set; }
        
        public PlayerInput PlayerInput
        {
            get
            {
                _playerInput ??= new PlayerInput();
                return _playerInput;
            }
        }
        
        
        public virtual void Initialize(int cellIndex, InventoryCellData inventoryCellData)
        {
            rectTransform ??= gameObject.GetComponent<RectTransform>();
            PlayerInput.Player.Enable();
            CellIndex = cellIndex;
            
            isCellEmpty = false;
            _iconImage.enabled = true;
            
            if (inventoryCellData.itemData != null)
            {
                _iconImage.sprite = inventoryCellData.itemData.Icon;
                _lightImage.color = _colorsData.ItemRarityColors[inventoryCellData.itemData.ItemRarity];
            }
            
            _lockIconTransform.gameObject.SetActive(inventoryCellData.IsLocked);
            
            /*
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
            */
            
            _cellButton.onClick.AddListener(() => onCellClicked?.Invoke(CellIndex));
        }

        public virtual void SetEmpty()
        {
            isCellEmpty = true;
            _iconImage.enabled = false;
            _lightImage.color = Color.clear;
            _cellButton.onClick.RemoveAllListeners();
            _lockIconTransform.gameObject.SetActive(false);
        }
        
        public void Select()
        {
            _cellButton.Select();
        }
        
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
            if (isCellEmpty)
                return;

            onCellSelected?.Invoke(CellIndex);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            onCellDeselected?.Invoke(CellIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isCellEmpty)
                return;

            onCellSelected?.Invoke(CellIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onCellDeselected?.Invoke(CellIndex);
        }
    }
}