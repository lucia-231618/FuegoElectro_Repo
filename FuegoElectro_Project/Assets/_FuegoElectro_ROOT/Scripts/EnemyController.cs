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

    // Animator y animaciones
    public Animator animator;
    [SerializeField] private string walkingAnim = "Walking"; // Bool para caminar
    [SerializeField] private string flyingAnim = "Flying"; // Bool para volar
    public bool isFlying = false; // Si es volador

    // Referencia al SpriteRenderer para flippeo
    private SpriteRenderer spriteRenderer;

    // Referencia al jugador
    private Transform player;

    // Variables de detección y ataque
    public float visionRange = 10f; // Rango de visión para agresivos
    public float attackRange = 2f; // Rango de ataque
    private bool isAttacking = false; // Flag para evitar ataques múltiples
    private bool isDead = false; // Flag para estado muerto
    private bool hasBeenAttacked = false; // Flag para defensivos

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

        if (!isAttacking)
        {
            Move();
        }

        CheckForAttack();
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

        // Animación
        animator.SetBool("Walking", !isFlying);
    }

    private void CheckForAttack()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log(gameObject.name + " - Distancia: " + distToPlayer + ", Tipo: " + enemyType + ", isAttacking: " + isAttacking + ", hasBeenAttacked: " + hasBeenAttacked);

        if (enemyType == EnemyType.Aggressive)
        {
            if (distToPlayer <= attackRange)
            {
                Debug.Log(gameObject.name + " - Atacando (agresivo)");
                Attack();
            }
            else if (distToPlayer <= visionRange && !isAttacking)
            {
                MoveTowardsPlayer();
            }
        }
        else if (enemyType == EnemyType.Defensive && hasBeenAttacked && distToPlayer <= attackRange)
        {
            Debug.Log(gameObject.name + " - Atacando (defensivo)");
            Attack();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.Translate(new Vector3(dir.x, 0, 0) * speed * Time.deltaTime);

        expectedFlipX = dir.x < 0;
        spriteRenderer.flipX = expectedFlipX;

        animator.SetBool("Walking", true);
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            Debug.Log(gameObject.name + " - Trigger 'Attack' seteado. Animator: " + (animator != null ? "OK" : "NULL"));

            // NO dañar aquí; se hace desde Animation Event
            Invoke("ResetAttack", 1f);
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

    private void ResetAttack()
    {
        isAttacking = false;
        Debug.Log(gameObject.name + " - ResetAttack llamado, isAttacking = false");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        animator.SetTrigger("Hurt");
        Debug.Log(gameObject.name + " - Recibió " + damage + " daño, salud: " + health);

        if (enemyType == EnemyType.Defensive)
        {
            hasBeenAttacked = true;
            Debug.Log(gameObject.name + " - hasBeenAttacked seteado a TRUE (defensivo activado)");

            // Devuelve el ataque si está en rango
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= attackRange && !isAttacking)
            {
                Debug.Log(gameObject.name + " - Defensivo devuelve el ataque");
                Attack();
            }
        }

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