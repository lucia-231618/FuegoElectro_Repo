using UnityEngine;

public class DefensiveEnemyController : MonoBehaviour
{
    // Variables de movimiento
    public float speed = 2f; // Velocidad de movimiento
    public float range = 10f; // Rango de patrulla
    private Vector3 startPosition; // Posición inicial
    private bool movingRight = true; // Dirección de movimiento
    private bool isChasing = false; // Flag para saber si está persiguiendo (solo después de ser atacado)
    private bool isAggressive = false; // Flag para saber si ya cambió a agresivo

    // Animator y animaciones
    public Animator animator;
    private string flyingAnim = "Flying"; // Bool para volar

    // Referencia al jugador
    private Transform player;

    // Variables de detección y ataque
    public float visionRange = 5f; // Rango de visión (solo cuando es agresivo)
    public float attackRange = 5f; // Rango de ataque
    private bool isAttacking = false; // Flag para evitar ataques múltiples
    private float attackCooldown = 1f; // Cooldown entre ataques
    private bool isDead = false; // Flag para estado muerto
    private bool hasBeenAttacked = false; // Flag para saber si ya fue atacado

    private bool isFacingRight = false;  // El sprite mira a la izquierda inicialmente

    // Salud
    [SerializeField] private int health = 150; // Editable en Inspector

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;  // Asigna el prefab en el Inspector
    [SerializeField] private Transform projectileSpawnPoint;  // Punto de spawn (ej. un child vacío en el enemigo, como la boca o el centro)

    void Start()
    {
        // Guardar posición inicial
        startPosition = transform.position;

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

        // Determinar si está persiguiendo (solo si es agresivo)
        isChasing = (isAggressive && distToPlayer <= visionRange && distToPlayer > attackRange);

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

        if (isAggressive)
        {
            Debug.Log(gameObject.name + " - Agresivo: isChasing=" + isChasing + ", dist=" + Vector3.Distance(transform.position, player.position) + ", attackCooldown=" + attackCooldown);
        }

        CheckForAttack(distToPlayer);
    }

    private void Move()
    {
        // Patrulla (mover en X, ya que range es horizontal)
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        transform.Translate(direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPosition.x) >= range)
        {
            movingRight = !movingRight;
        }

        // Animación: siempre volando
        animator.SetBool(flyingAnim, true);
    }

    private void CheckForAttack(float distToPlayer)
    {
        Debug.Log(gameObject.name + " - Distancia: " + distToPlayer + ", isAttacking: " + isAttacking + ", hasBeenAttacked: " + hasBeenAttacked + ", isAggressive: " + isAggressive + ", attackCooldown: " + attackCooldown);

        if (isAggressive)
        {
            // Comportamiento agresivo: ataca si está en rango
            if (distToPlayer <= attackRange && attackCooldown <= 0)
            {
                Debug.Log(gameObject.name + " - Atacando (agresivo)");
                Attack();
            }
        }
        else if (hasBeenAttacked && distToPlayer <= attackRange && attackCooldown <= 0)
        {
            // Comportamiento defensivo: ataca solo si fue atacado y está en rango
            Debug.Log(gameObject.name + " - Atacando (defensivo)");
            Attack();
            // Cambiar a agresivo después de atacar
            isAggressive = true;
            Debug.Log(gameObject.name + " - Cambiado a agresivo tras atacar");
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        Debug.Log(gameObject.name + " - Flip ejecutado, nuevo isFacingRight: " + isFacingRight);
    }

    private void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        Vector3 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);

        // Flip solo si la dirección horizontal es clara (umbral para evitar flips por movimientos pequeños)
        float threshold = 0.1f;  // Ajusta según necesidad (ej. 0.1f para evitar flips por 0.05f)
        if (dir.x > threshold && !isFacingRight)
        {
            Flip();
        }
        else if (dir.x < -threshold && isFacingRight)
        {
            Flip();
        }

        animator.SetBool(flyingAnim, true);

        Debug.Log(gameObject.name + " - Dir.x: " + dir.x + ", isFacingRight: " + isFacingRight + ", threshold: " + threshold);
    }

    private void Attack()
    {
        if (!isAttacking && attackCooldown <= 0)
        {
            isAttacking = true;
            attackCooldown = 1f;  // Ajusta según tu cooldown
            animator.SetTrigger("Attack");

            // Flip adicional si es necesario (opcional, ya que MoveTowardsPlayer lo hace)
            Vector3 dir = (player.position - transform.position).normalized;
            if (dir.x > 0 && !isFacingRight) Flip();
            else if (dir.x < 0 && isFacingRight) Flip();

            Debug.Log(gameObject.name + " - Trigger 'Attack' seteado. Animator: " + (animator != null ? "OK" : "NULL"));
        }
    }

    // Método para lanzar el proyectil (llámalo desde Animation Event)
    public void LaunchProjectile()
    {
        Debug.Log(gameObject.name + " - LaunchProjectile llamado");
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            Debug.Log(gameObject.name + " - Instanciando proyectil en: " + projectileSpawnPoint.position);
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Projectile prefab o spawn point no asignados en " + gameObject.name);
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

        if (!hasBeenAttacked)
        {
            hasBeenAttacked = true;
            isAggressive = true;  // Se vuelve agresivo inmediatamente al primer daño
            Debug.Log(gameObject.name + " - hasBeenAttacked seteado a TRUE y isAggressive = TRUE (ahora persigue y ataca proactivamente)");

            // Devuelve el ataque si está en rango (opcional, pero ahora es agresivo de todos modos)
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= attackRange && !isAttacking && attackCooldown <= 0)
            {
                Debug.Log(gameObject.name + " - Defensivo devuelve el ataque inicial");
                Attack();
            }
        }

        // Solo defensivos hacen la animación de Hurt
        animator.SetTrigger("Hurt");
        Invoke("TryDefensiveAttack", 0.5f);  // Delay de 0.5 segundos para que Hurt se reproduzca primero

        if (health <= 0)
        {
            Die();
        }
    }
    private void TryDefensiveAttack()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer <= attackRange && !isAttacking && attackCooldown <= 0)
        {
            Debug.Log(gameObject.name + " - Defensivo devuelve el ataque tras Hurt");
            Attack();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        Destroy(gameObject, 1f);
    }
}