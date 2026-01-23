using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Variables de referencia
    private Rigidbody2D playerRb;
    private Animator anim;
    private float horizontalInput;

    //Variables de estadística del player
    public float speed;
    public float jumpForce;
    private bool isFacingRight = true;
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck;
    [SerializeField] LayerMask groundLayer;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool isClimbing = false;

    [SerializeField] private LayerMask ladderLayer; // Usar capas para triggers
    private bool onLadder = false;
    private float verticalInput;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isHurt = false;


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            onLadder = false;
            Debug.Log("Salió de ladder: onLadder = " + onLadder);
        }
    }

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);

        verticalInput =
        (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) ? 1 :
        (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) ? -1 : 0;

        Debug.Log("Update: onLadder = " + onLadder + " | velocity = " + playerRb.linearVelocity + " | isClimbing = " + isClimbing);

        float rayLength = 0.5f;
        Vector2 rayOrigin = groundCheck.transform.position;
        bool centerHit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);
        bool leftHit = Physics2D.Raycast(rayOrigin + Vector2.left * 0.2f, Vector2.down, rayLength, groundLayer);
        bool rightHit = Physics2D.Raycast(rayOrigin + Vector2.right * 0.2f, Vector2.down, rayLength, groundLayer);
        isGrounded = centerHit || leftHit || rightHit;

        Movement();
        Jump();
        Attack();
        //Climb();
    }

    void Movement()
    {
        if (Keyboard.current == null) return;

        horizontalInput =
            (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0) +
            (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0);

        playerRb.linearVelocity = new Vector2(horizontalInput * speed,playerRb.linearVelocity.y);

        if (isAttacking || isDead || isClimbing || isHurt) return;

        //Flip: si el valor del input es diferente a 0
        if (horizontalInput > 0) 
        {
            anim.SetBool("Running", true);
            if (!isFacingRight)
            { 
                Flip();
            }
        }
        if (horizontalInput < 0)
        {
            anim.SetBool("Running", true);
            if (isFacingRight)
            {
                Flip();
            }
        }
        if(horizontalInput == 0)
        {
            anim.SetBool("Running", false);
        }
    }

    void Jump()
    {
        anim.SetBool("Jumping", !isGrounded);
        if ((Keyboard.current.spaceKey.wasPressedThisFrame) && isGrounded)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (isDead || isClimbing || isHurt) return;
    }

    void Flip()
    { 
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1; 
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    
    }
    void Attack()
    {
        if (isAttacking || isDead || isClimbing) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isAttacking = true;
            anim.SetTrigger("Attack_Sword");
            Invoke(nameof(ResetAttack), 0.5f); // 0.5 = duración aproximada de la animación
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            isAttacking = true;
            anim.SetTrigger("Attack_Thunder");
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    //void Climb()
    //{
    //    int playerLayer = LayerMask.NameToLayer("Player");
    //    int groundLayerIndex = LayerMask.NameToLayer("Ground");

    //    if (onLadder)
    //    {
    //        isClimbing = true;
    //        playerRb.gravityScale = 0;

    //        if (playerLayer != -1 && groundLayerIndex != -1)
    //        {
    //            Physics2D.IgnoreLayerCollision(playerLayer, groundLayerIndex, true);
    //        }

    //        if (verticalInput != 0)
    //        {
    //            playerRb.linearVelocity = new Vector2(0, verticalInput * speed);
    //            anim.SetBool("Climbing", true);
    //        }
    //        else
    //        {
    //            // Fuerza reset completo de velocidad para evitar pegado
    //            playerRb.linearVelocity = Vector2.zero;
    //            anim.SetBool("Climbing", false);
    //        }
    //    }
    //    else
    //    {
    //        isClimbing = false;
    //        playerRb.gravityScale = 1;

    //        if (playerLayer != -1 && groundLayerIndex != -1)
    //        {
    //            Physics2D.IgnoreLayerCollision(playerLayer, groundLayerIndex, false);
    //        }

    //        anim.SetBool("Climbing", false);
    //    }
    //    if (isAttacking || isDead || isHurt) return;
    //}

    public void TakeDamage(int damage)
    {
        if (isDead || isHurt) return;

        currentHealth -= damage;
        isHurt = true;

        anim.SetTrigger("hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke(nameof(ResetHurt), 0.4f); // duración anim hurt
        }
    }

    void ResetHurt()
    {
        isHurt = false;
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("death");

        playerRb.linearVelocity = Vector2.zero;
        playerRb.bodyType = RigidbodyType2D.Static;
    }
}

