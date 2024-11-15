using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [SerializeField] public float amplitude = 0.5f; // Высота качания
    [SerializeField] private float speed = 2f; // Скорость качания

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnDisable()
    {
        transform.position = startPosition;
    }
}