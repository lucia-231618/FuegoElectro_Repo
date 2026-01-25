using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float speed = 2f;          // Velocidad del movimiento
    public float distance = 5f;       // Distancia máxima de desplazamiento
    private Vector3 startPosition;    // Posición inicial

    void Start()
    {
        startPosition = transform.position;  // Guarda la posición inicial
    }

    void Update()
    {
        // Calcula el desplazamiento usando PingPong para oscilar
        float offset = Mathf.PingPong(Time.time * speed, distance);

        // Aplica el movimiento en el eje X (cambia a Z si es necesario)
        transform.position = startPosition + new Vector3(offset, 0f, 0f);
    }
}