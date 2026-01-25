using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;  // Velocidad del proyectil
    public int damage = 10;   // Daño que inflige
    private Vector3 direction; // Dirección hacia el jugador
    private Transform player;  // Referencia al jugador

    void Start()
    {
        Debug.Log("Proyectil creado en posición: " + transform.position);

        // Encuentra al jugador
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            // Calcula la dirección hacia el jugador al inicio
            direction = (player.position - transform.position).normalized;
            Debug.Log("Dirección calculada: " + direction + ", magnitud: " + direction.magnitude);
        }
        else
        {
            Debug.LogError("No se encontró el jugador con tag 'Player'");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Mueve el proyectil en la dirección calculada
        transform.Translate(direction * speed * Time.deltaTime);
        Debug.Log("Proyectil moviéndose a: " + transform.position + ", dirección: " + direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Proyectil colisionó con: " + collision.name + ", tag: " + collision.tag + ", layer: " + collision.gameObject.layer);

        if (collision.CompareTag("Player"))
        {
            // Aplica daño al jugador
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
                Debug.Log("Proyectil aplicó daño al jugador: " + damage);
            }
            else
            {
                Debug.LogError("PlayerController no encontrado en el jugador");
            }

            // Destruye el proyectil
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            // Destruye al colisionar con paredes u obstáculos
            Debug.Log("Proyectil destruido al colisionar con: " + collision.tag);
            Destroy(gameObject);
        }
        // Ignora otras colisiones
    }
}