using UnityEngine;

public class DefensiveEnemyController : MonoBehaviour
{
    // Variables de movimiento
    public float speed = 2f; // Velocidad de movimiento
    public float range = 5f; // Rango de patrulla
    private Vector3 startPosition; // Posición inicial
    private bool movingRight = true; // Dirección de movimiento
    private bool isChasing = false; // Flag para saber si está persiguiendo (solo después de ser atacado)
    private bool isAggressive = false; // Flag para saber si ya cambió a agresivo

    // Animator y animaciones
    public Animator animator;
    [SerializeField] private string flyingAnim = "Flying"; // Bool para volar

    // Referencia al jugador
    private Transform player;

    // Variables de detección y ataque
    public float visionRange = 10f; // Rango de visión (solo cuando es agresivo)
    public float attackRange = 2f; // Rango de ataque
    private bool isAttacking = false; // Flag para evitar ataques múltiples
    private float attackCooldown = 0f; // Cooldown entre ataques
    private bool isDead = false; // Flag para estado muerto
    private bool hasBeenAttacked = false; // Flag para saber si ya fue atacado

    // Salud
    [SerializeField] private int health = 100; // Editable en Inspector

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

    private void MoveTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        // Mover en ambas direcciones (X e Y) ya que siempre vuela
        transform.Translate(dir * speed * Time.deltaTime);

        // Animación: siempre volando
        animator.SetBool(flyingAnim, true);
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

        if (!hasBeenAttacked)
        {
            hasBeenAttacked = true;
            Debug.Log(gameObject.name + " - hasBeenAttacked seteado a TRUE (defensivo activado)");

            // Devuelve el ataque si está en rango
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= attackRange && !isAttacking && attackCooldown <= 0)
            {
                Debug.Log(gameObject.name + " - Defensivo devuelve el ataque");
                Attack();
            }
        }

        // Solo defensivos hacen la animación de Hurt
        animator.SetTrigger("Hurt");

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