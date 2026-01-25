using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Animator anim;
    private float horizontalInput;
    private float verticalInput;

    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 10f;

    private bool isFacingRight = true;

    [Header("Ground")]
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    private bool wasGrounded; //Atterizaje (para las plataformas)

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isHurt = false;
    private bool isDead = false;

    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private UnityEngine.Transform attackPoint;
    [SerializeField] private int swordDamage = 10;
    [SerializeField] private int thunderDamage = 15;

    private GameObject currentHitbox; // Referencia a la hitbox activa

    // Variables para doble salto
    private int jumpCount = 0;
    private int maxJumps = 2; // Máximo de saltos (1 en suelo + 1 en aire)

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Cleanup: Destruye cualquier hitbox residual al iniciar
        if (currentHitbox != null)
        {
            Destroy(currentHitbox);
            currentHitbox = null;
        }
    }

    void Update()
    {
        horizontalInput =
            (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0) +
            (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0);

        verticalInput =
            (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 :
            (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : 0));

        // Actualiza wasGrounded antes de calcular isGrounded
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);

        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    private void HandleMovement()
    {
        if (isDead || isHurt) return;

        playerRb.linearVelocity = new Vector2(horizontalInput * speed, playerRb.linearVelocity.y);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        else if (horizontalInput < 0 && isFacingRight) Flip();

        anim.SetBool("Running", horizontalInput != 0);
    }

    private void HandleJump()
    {
        if (isDead || isHurt) return;

        // Resetea el contador de saltos SOLO cuando aterriza (pasa de no grounded a grounded)
        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
        }

        anim.SetBool("Jumping", !isGrounded);

        // Permite saltar si no ha alcanzado el máximo de saltos
        if (Keyboard.current.spaceKey.wasPressedThisFrame && jumpCount < maxJumps)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++; // Incrementa el contador
        }
    }

    private void HandleAttack()
    {
        if (isDead || isHurt) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            anim.SetTrigger("Attack_Sword");
            // La hitbox se crea desde el Animation Event al inicio
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            anim.SetTrigger("Attack_Thunder");
            // Lo mismo para trueno
        }
    }

    // Método para Animation Event al INICIO de "Attack_Sword"
    public void CreateHitboxSword()
    {
        if (hitboxPrefab == null || attackPoint == null)
        {
            return;
        }

        // Destruye la hitbox anterior si existe
        if (currentHitbox != null)
        {
            Destroy(currentHitbox);
        }

        currentHitbox = Instantiate(hitboxPrefab, attackPoint.position, Quaternion.identity);
        currentHitbox.transform.parent = attackPoint;

        Hitbox hitboxScript = currentHitbox.GetComponent<Hitbox>();
        if (hitboxScript != null)
        {
            hitboxScript.damage = swordDamage;
            hitboxScript.ResetDamage();
        }
    }

    // Método para Animation Event al INICIO de "Attack_Thunder"
    public void CreateHitboxThunder()
    {
        if (hitboxPrefab == null || attackPoint == null)
        {
            return;
        }

        // Destruye la hitbox anterior si existe
        if (currentHitbox != null)
        {
            Destroy(currentHitbox);
        }

        currentHitbox = Instantiate(hitboxPrefab, attackPoint.position, Quaternion.identity);
        currentHitbox.transform.parent = attackPoint;

        Hitbox hitboxScript = currentHitbox.GetComponent<Hitbox>();
        if (hitboxScript != null)
        {
            hitboxScript.damage = thunderDamage;
            hitboxScript.ResetDamage();
        }
    }

    // Método para Animation Event al FINAL de cualquier ataque (Sword or Thunder)
    public void DestroyHitbox()
    {
        if (currentHitbox != null)
        {
            Destroy(currentHitbox);
            currentHitbox = null;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        isHurt = true;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Método para Animation Event al final de la animación "Hurt"
    public void ResetHurt()
    {
        isHurt = false;
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Death");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si toca lava o pinchos (usando layer), muere
        if (collision.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            Die();
        }
    }
}