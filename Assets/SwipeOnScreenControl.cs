using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;



public class SwipeOnScreenControl : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    private float sensitivity = 1.0f;
    
    private string _controlPath = "<Touchscreen>/primaryTouch/delta";

    private Vector2 startPointerPosition; // Начальная позиция касания
    private bool isDragging = false; // Флаг активности взаимодействия


    protected override string controlPathInternal
    {
        get => _controlPath;
        set => _controlPath = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Устанавливаем начальную позицию при касании
        startPointerPosition = eventData.position;
        isDragging = true;

        Debug.Log($"SwipeOnScreenControl -> Pointer Down at: {startPointerPosition}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Текущая позиция пальца или мыши
        Vector2 currentPointerPosition = eventData.position;

        // Рассчитываем смещение от начальной точки
        Vector2 delta = currentPointerPosition - startPointerPosition;

        // Нормализуем ввод относительно размера экрана и регулируем чувствительностью
        Vector2 adjustedDelta = new Vector2(
            (delta.x / Screen.width) * sensitivity,
            (delta.y / Screen.height) * sensitivity
        );

        // Отправляем данные в Input System
        SendValueToControl(adjustedDelta);

        // Обновляем начальную точку для поддержки непрерывного движения
        startPointerPosition = currentPointerPosition;

        Debug.Log($"SwipeOnScreenControl -> Dragging with Delta: {adjustedDelta}");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Отправляем "нулевые" значения при завершении взаимодействия
        SendValueToControl(Vector2.zero);
        isDragging = false;

        Debug.Log("SwipeOnScreenControl -> Pointer Up - Input Reset");
    }
}