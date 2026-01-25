////using UnityEngine;

////public class AggressiveEnemyController : MonoBehaviour
////{
////    // Variables de movimiento
////    public float speed = 2f; // Velocidad de movimiento
////    public float range = 5f; // Rango de patrulla
////    private Vector3 startPosition; // Posición inicial
////    private bool movingRight = true; // Dirección de movimiento
////    private bool isChasing = false; // Flag para saber si está persiguiendo

////    // Animator y animaciones
////    public Animator animator;
////    [SerializeField] private string walkingAnim = "Walking"; // Bool para caminar

////    // Referencia al jugador
////    private Transform player;

////    // Variables de detección y ataque
////    public float visionRange = 5f; // Rango de visión
////    public float attackRange = 3f; // Rango de ataque (usado para persecución cercana)
////    private bool isAttacking = false; // Flag para evitar ataques múltiples
////    private float attackCooldown = 0f; // Cooldown entre ataques
////    private bool isDead = false; // Flag para estado muerto

////    // Salud
////    [SerializeField] private int health = 100; // Editable en Inspector
////    private bool isHurt = false; // Flag para estado herido (con invencibilidad breve)
////    private float hurtDuration = 0.5f; // Duración de la animación de Hurt

////    // Mejoras: Abandono de persecución
////    private float abandonChaseMultiplier = 1.5f; // Multiplicador para dejar de perseguir (visionRange * 1.5)

////    // Para flippeo consistente (usando localScale)
////    private bool isFacingRight = true; // Asumimos que el sprite mira a la derecha inicialmente

////    // Para hitbox de ataque
////    [Header("Attack Hitbox")]
////    [SerializeField] private GameObject attackHitboxPrefab; // Asigna el prefab de la hitbox en el Inspector (debe tener un collider trigger)
////    [SerializeField] private Transform attackHitboxSpawnPoint; // Punto de spawn (e.g., un child vacío en la mano o centro del enemigo)
////    private GameObject currentAttackHitbox; // Referencia a la hitbox activa

////    // Timeout para evitar parálisis
////    private float attackTimeout = 1f; // Máximo 1s en ataque antes de resetear
////    private float attackTimer = 0f;

////    void Start()
////    {
////        // Guardar posición inicial
////        startPosition = transform.position;

////        // Encontrar al jugador
////        player = GameObject.FindGameObjectWithTag("Player")?.transform;
////        if (player == null)
////        {
////            Debug.LogError("No se encontró un objeto con tag 'Player'");
////        }

////        // Asegurar escala inicial correcta
////        if (transform.localScale.x < 0)
////        {
////            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
////            isFacingRight = false;
////        }

////        // Limpiar hitbox residual al iniciar
////        if (currentAttackHitbox != null)
////        {
////            Destroy(currentAttackHitbox);
////            currentAttackHitbox = null;
////        }
////    }

////    void Update()
////    {
////        if (isDead || player == null || isHurt)
////        {
////            return;
////        }

////        // Decrementar cooldown
////        if (attackCooldown > 0)
////        {
////            attackCooldown -= Time.deltaTime;
////        }

////        // Timeout para ataque
////        if (isAttacking)
////        {
////            attackTimer += Time.deltaTime;
////            if (attackTimer >= attackTimeout)
////            {
////                Debug.LogWarning(gameObject.name + " - Timeout de ataque alcanzado (1s), reseteando");
////                ResetAttack();
////                attackTimer = 0f;
////            }
////        }

////        float distToPlayer = Vector3.Distance(transform.position, player.position);

////        // Determinar si está persiguiendo: ve al player (visionRange) pero no está lo suficientemente cerca para dejar de perseguir (attackRange)
////        isChasing = (distToPlayer <= visionRange && distToPlayer > attackRange);

////        if (!isAttacking)
////        {
////            if (isChasing)
////            {
////                MoveTowardsPlayer();
////            }
////            else
////            {
////                Move();
////            }
////        }

////        CheckForAttack(distToPlayer);
////    }

////    private void Move()
////    {
////        // Patrulla
////        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
////        transform.Translate(direction * speed * Time.deltaTime);

////        if (Mathf.Abs(transform.position.x - startPosition.x) >= range)
////        {
////            movingRight = !movingRight;
////            Flip(); // Voltear al cambiar dirección
////        }

////        // Animación: siempre caminando
////        animator.SetBool(walkingAnim, true);
////    }

////    private void CheckForAttack(float distToPlayer)
////    {
////        // CAMBIO: Ataca SIEMPRE que vea al player (en visionRange), no solo cerca
////        if (distToPlayer <= visionRange && attackCooldown <= 0)
////        {
////            Attack();
////        }
////    }

////    private void MoveTowardsPlayer()
////    {
////        Vector3 dir = (player.position - transform.position).normalized;

////        // Mover SOLO en X (horizontal)
////        Vector3 moveDir = new Vector3(dir.x, 0, 0);
////        transform.Translate(moveDir * speed * Time.deltaTime);

////        // Flip basado en dirección horizontal
////        if (dir.x > 0.1f && !isFacingRight)
////        {
////            Flip();
////        }
////        else if (dir.x < -0.1f && isFacingRight)
////        {
////            Flip();
////        }

////        // Animación: siempre caminando
////        animator.SetBool(walkingAnim, true);
////    }

////    private void Attack()
////    {
////        if (!isAttacking && attackCooldown <= 0)
////        {
////            isAttacking = true;
////            attackCooldown = 1f; // CAMBIO: Cooldown reducido a 1s para ataques más frecuentes
////            attackTimer = 0f; // Resetear timer
////            animator.SetTrigger("Attack");
////        }
////    }

////    // Método para Animation Event al INICIO de "Attack" - Crea la hitbox
////    public void CreateAttackHitbox()
////    {
////        if (attackHitboxPrefab == null || attackHitboxSpawnPoint == null)
////        {
////            Debug.LogError("attackHitboxPrefab o attackHitboxSpawnPoint no asignados en " + gameObject.name);
////            return;
////        }

////        // Destruye la hitbox anterior si existe
////        if (currentAttackHitbox != null)
////        {
////            Destroy(currentAttackHitbox);
////        }

////        currentAttackHitbox = Instantiate(attackHitboxPrefab, attackHitboxSpawnPoint.position, Quaternion.identity);
////        currentAttackHitbox.transform.parent = attackHitboxSpawnPoint;

////        Debug.Log(gameObject.name + " - Hitbox de ataque activada");
////    }

////    // Método para Animation Event al FINAL de "Attack" - Destruye la hitbox
////    public void DestroyAttackHitbox()
////    {
////        if (currentAttackHitbox != null)
////        {
////            Destroy(currentAttackHitbox);
////            currentAttackHitbox = null;
////            Debug.Log(gameObject.name + " - Hitbox de ataque desactivada");
////        }
////    }

////    // Método para Animation Event: resetea el ataque al final de la animación
////    public void ResetAttack()
////    {
////        isAttacking = false;
////        attackTimer = 0f;
////        Debug.Log(gameObject.name + " - ResetAttack llamado, isAttacking=false");
////    }

////    public void TakeDamage(int damage)
////    {
////        if (isDead || isHurt) return;

////        health -= damage;
////        isHurt = true;

////        // Knockback reducido
////        Vector3 knockbackDir = (transform.position - player.position).normalized;
////        transform.Translate(knockbackDir * 0.2f);

////        // Animación de Hurt
////        animator.SetTrigger("Hurt");

////        // Resetear Hurt
////        Invoke("ResetHurt", hurtDuration);

////        if (health <= 0)
////        {
////            Die();
////        }
////    }

////    private void ResetHurt()
////    {
////        isHurt = false;
////    }

////    private void Die()
////    {
////        isDead = true;
////        animator.SetTrigger("Death");
////        Destroy(gameObject, 1f);
////    }

////    private void Flip()
////    {
////        isFacingRight = !isFacingRight;
////        Vector3 scale = transform.localScale;
////        scale.x *= -1;
////        transform.localScale = scale;
////    }

////    // Gizmos para visualizar rangos
////    void OnDrawGizmosSelected()
////    {
////        Gizmos.color = Color.yellow;
////        Gizmos.DrawWireSphere(transform.position, visionRange);
////        Gizmos.color = Color.red;
////        Gizmos.DrawWireSphere(transform.position, attackRange);
////    }
////}