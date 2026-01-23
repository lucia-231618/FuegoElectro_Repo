using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Enum para los tipos de enemigo
    public enum EnemyType { Aggressive, Defensive }

    // Tipo de enemigo (configurable en el Inspector)
    public EnemyType enemyType;

    // Variables de movimiento
    public float speed = 2f; // Velocidad de movimiento
    public float range = 5f; // Rango de patrulla
    private Vector3 startPosition; // Posición inicial
    private bool movingRight = true; // Dirección de movimiento

    // Animator y animaciones (ahora privadas, pero editables en Inspector con SerializeField)
    public Animator animator;
    private string walkingAnim = "Walking"; // Nombre del trigger para caminar
    private string flyingAnim = "Flying"; // Nombre del trigger para volar
    public bool isFlying = false; // Si es volador (cambia la animación de movimiento)

    // Referencia al SpriteRenderer para flippeo
    private SpriteRenderer spriteRenderer;

    // Referencia al jugador
    private Transform player;

    // Variables de detección y ataque
    public float visionRange = 10f; // Rango de visión para agresivos
    public float attackRange = 2f; // Rango de ataque
    private bool isAttacking = false; // Flag para evitar ataques múltiples
    private bool isDead = false; // Flag para estado muerto
    private bool hasBeenAttacked = false; // Flag para enemigos defensivos (atacan solo si son atacados)

    // Salud
    public int health = 100;

    void Start()
    {
        // Guardar posición inicial
        startPosition = transform.position;

        // Obtener el SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en " + gameObject.name + ". Asegúrate de que sea un sprite. El flippeo no funcionará.");
        }
        else
        {
            // Verificar escala inicial (debe ser positiva en X)
            if (transform.localScale.x < 0)
            {
                Debug.LogWarning(gameObject.name + " tiene escala X negativa. Esto puede invertir el flippeo. Corrígelo en el Inspector.");
            }

            // Flip inicial: Asumiendo que el sprite mira a la derecha por defecto
            spriteRenderer.flipX = !movingRight; // movingRight = true → flipX = false (derecha)
            Debug.Log(gameObject.name + " - Flip inicial: FlipX = " + spriteRenderer.flipX);
        }

        // Encontrar al jugador por tag (asegúrate de que el jugador tenga el tag "Player")
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con tag 'Player'. Asegúrate de etiquetarlo correctamente.");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Movimiento de patrulla
        Move();

        // Verificar si debe atacar
        CheckForAttack();
    }

    // Método de movimiento: camina en un rango y flippea al llegar al límite
    private void Move()
    {
        // Mover en la dirección actual
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        transform.Translate(direction * speed * Time.deltaTime);

        // Verificar si ha alcanzado el límite del rango
        if (Mathf.Abs(transform.position.x - startPosition.x) >= range)
        {
            // Cambiar dirección
            movingRight = !movingRight;

            // Flippear el sprite solo si SpriteRenderer existe
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !movingRight; // Asumiendo sprite mira a la derecha por defecto
                Debug.Log(gameObject.name + " - Flippeando en límite. Posición: " + transform.position.x + ", MovingRight: " + movingRight + ", FlipX: " + spriteRenderer.flipX);
            }
            else
            {
                Debug.LogWarning("SpriteRenderer es null en " + gameObject.name + ". No se puede flippear.");
            }
        }

        // Activar animación de movimiento (Walking o Flying)
        if (!isFlying)
        {
            animator.SetTrigger(walkingAnim);
        }
        else
        {
            animator.SetTrigger(flyingAnim);
        }
    }

    // Verificar si debe atacar basado en el tipo de enemigo (sin flippeo hacia el jugador)
    private void CheckForAttack()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Enemigos agresivos: atacan si ven al jugador dentro del rango de visión
        if (enemyType == EnemyType.Aggressive && distToPlayer <= visionRange)
        {
            Attack();
        }
        // Enemigos defensivos: atacan solo si han sido atacados y el jugador está en rango de ataque
        else if (enemyType == EnemyType.Defensive && hasBeenAttacked && distToPlayer <= attackRange)
        {
            Attack();
        }
    }

    // Método de ataque
    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            // Aquí puedes agregar lógica adicional, como infligir daño al jugador
            // Por ejemplo: player.GetComponent<PlayerController>().TakeDamage(10);
            // Resetear el flag después de la animación (asumiendo duración corta)
            Invoke("ResetAttack", 1f); // Ajusta el tiempo según la duración de la animación
        }
    }

    // Resetear el flag de ataque
    private void ResetAttack()
    {
        isAttacking = false;
    }

    // Método para recibir daño (llámalo desde el script del jugador o proyectiles)
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        animator.SetTrigger("Hurt");

        // Si es defensivo, marcar que ha sido atacado
        if (enemyType == EnemyType.Defensive)
        {
            hasBeenAttacked = true;
        }

        // Verificar si muere
        if (health <= 0)
        {
            Die();
        }
    }

    // Método de muerte
    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        // Destruir el objeto después de la animación (ajusta el tiempo según la duración)
        Destroy(gameObject, 1f);
    }
}