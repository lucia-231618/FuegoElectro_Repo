using UnityEngine;

public class AggressiveEnemyController : MonoBehaviour
{
    // Variables de movimiento
    public float speed = 2f; // Velocidad de movimiento
    public float range = 5f; // Rango de patrulla
    private Vector3 startPosition; // Posición inicial
    private bool movingRight = true; // Dirección de movimiento
    private bool isChasing = false; // Flag para saber si está persiguiendo

    // Animator y animaciones
    public Animator animator;
    [SerializeField] private string walkingAnim = "Walking"; // Bool para caminar

    // Referencia al SpriteRenderer para flippeo
    private SpriteRenderer spriteRenderer;

    // Referencia al jugador
    private Transform player;

    // Variables de detección y ataque
    public float visionRange = 10f; // Rango de visión
    public float attackRange = 2f; // Rango de ataque
    private bool isAttacking = false; // Flag para evitar ataques múltiples
    private float attackCooldown = 0f; // Cooldown entre ataques
    private bool isDead = false; // Flag para estado muerto

    // Salud
    [SerializeField] private int health = 100; // Editable en Inspector

    // Para rastrear flipX esperado
    private bool expectedFlipX;

    void Start()
    {
        // Guardar posición inicial
        startPosition = transform.position;

        // Obtener el SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en " + gameObject.name);
        }
        else
        {
            if (transform.localScale.x < 0)
            {
                Debug.LogWarning(gameObject.name + " tiene escala X negativa.");
            }
            expectedFlipX = !movingRight;
            spriteRenderer.flipX = expectedFlipX;
        }

        // Encontrar al jugador
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con tag 'Player'");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Decrementar cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Determinar si está persiguiendo
        isChasing = (distToPlayer <= visionRange && distToPlayer > attackRange);

        if (!isAttacking)
        {
            if (isChasing)
            {
                MoveTowardsPlayer();
            }
            else
            {
                Move();
            }
        }

        CheckForAttack(distToPlayer);
    }

    void LateUpdate()
    {
        if (spriteRenderer != null && spriteRenderer.flipX != expectedFlipX)
        {
            spriteRenderer.flipX = expectedFlipX;
        }
    }

    private void Move()
    {
        // Patrulla
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        transform.Translate(direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPosition.x) >= range)
        {
            movingRight = !movingRight;
            expectedFlipX = !movingRight;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = expectedFlipX;
            }
        }

        // Animación: siempre caminando
        animator.SetBool(walkingAnim, true);
    }

    private void CheckForAttack(float distToPlayer)
    {
        Debug.Log(gameObject.name + " - Distancia: " + distToPlayer + ", isAttacking: " + isAttacking + ", attackCooldown: " + attackCooldown);

        if (distToPlayer <= attackRange && attackCooldown <= 0)
        {
            Debug.Log(gameObject.name + " - Atacando (agresivo)");
            Attack();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        // Mover solo en X, ya que nunca vuela
        transform.Translate(new Vector3(dir.x, 0, 0) * speed * Time.deltaTime);

        expectedFlipX = dir.x < 0;
        spriteRenderer.flipX = expectedFlipX;

        // Animación: siempre caminando
        animator.SetBool(walkingAnim, true);
    }

    private void Attack()
    {
        if (!isAttacking && attackCooldown <= 0)
        {
            isAttacking = true;
            attackCooldown = 2f; // Cooldown de 2 segundos entre ataques
            animator.SetTrigger("Attack");
            Debug.Log(gameObject.name + " - Trigger 'Attack' seteado. Animator: " + (animator != null ? "OK" : "NULL"));
        }
    }

    // Método para Animation Event: daña durante la animación
    public void DealDamage()
    {
        if (player != null)
        {
            PlayerController playerScript = player.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(10);
                Debug.Log(gameObject.name + " - Daño infligido al jugador durante animación");
            }
        }
    }

    // Método para Animation Event: resetea el ataque al final de la animación
    public void ResetAttack()
    {
        isAttacking = false;
        Debug.Log(gameObject.name + " - ResetAttack llamado, isAttacking = false");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log(gameObject.name + " - Recibió " + damage + " daño, salud: " + health);

        // Agresivos no hacen animación de Hurt ni responden al daño
        // Solo pierden vida

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        Destroy(gameObject, 1f);
    }
}