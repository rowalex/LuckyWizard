using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f; // Высота качания
    [SerializeField] private float speed = 2f; // Скорость качания

    private Vector3 startPosition;

    private void Start()
    {
        // Сохраняем начальную позицию объекта
        startPosition = transform.position;
    }

    private void Update()
    {
        // Вычисляем новую позицию с помощью синусоиды
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}