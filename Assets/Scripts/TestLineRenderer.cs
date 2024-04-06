using UnityEngine;

public class TestLineRenderer : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    
    public Color routeColor = Color.green;

    public void UpdateData(Vector3 startPoint, Vector3 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }
    
    private void OnDrawGizmos()
    {
        // Устанавливаем цвет линии
        Gizmos.color = routeColor;

        // Проверяем, существуют ли начальная и конечная точки
        if (startPoint != null && endPoint != null)
        {
            // Рисуем линию маршрута между начальной и конечной точками
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
