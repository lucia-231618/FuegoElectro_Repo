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
    private bool isClimbing = false;
    private bool onLadder = false;

    [Header("Ground")]
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isHurt = false;
    private bool isDead = false;

    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int swordDamage = 10;
    [SerializeField] private int thunderDamage = 15;

    private GameObject currentHitbox; // Referencia a la hitbox activa

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
        // Nota: Si hay hitboxes sueltas en la escena, bórralas manualmente de la jerarquía
    }

    void Update()
    {
        horizontalInput =
            (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0) +
            (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0);

        verticalInput =
            (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 :
            (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : 0));

        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);

        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    private void HandleMovement()
    {
        if (isDead || isHurt || isClimbing) return;

        playerRb.linearVelocity = new Vector2(horizontalInput * speed, playerRb.linearVelocity.y);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        else if (horizontalInput < 0 && isFacingRight) Flip();

        anim.SetBool("Running", horizontalInput != 0);
    }

    private void HandleJump()
    {
        if (isDead || isClimbing || isHurt) return;

        anim.SetBool("Jumping", !isGrounded);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void HandleAttack()
    {
        if (isDead || isHurt || isClimbing) return;

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
    public void CreateHitbox()
    {
        if (hitboxPrefab == null || attackPoint == null)
        {
            Debug.LogError("hitboxPrefab o attackPoint no asignados!");
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

        Debug.Log("Hitbox activada al inicio de Attack_Sword");
    }

    public void DestroyHitbox()
    {
        if (currentHitbox != null)
        {
            Destroy(currentHitbox);
            currentHitbox = null;
            Debug.Log("Hitbox desactivada al final de Attack_Sword");
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
        Debug.Log("Jugador recibió " + damage + " de daño. Salud: " + currentHealth);

        Invoke("ResetHurt", 0.5f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ResetHurt()
    {
        isHurt = false;
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Death");
        Debug.Log("Jugador muerto");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
            onLadder = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
            onLadder = false;
    }
}