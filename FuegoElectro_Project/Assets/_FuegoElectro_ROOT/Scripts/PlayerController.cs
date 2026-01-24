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

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Detectar inputs
        horizontalInput =
            (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0) +
            (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0);

        verticalInput =
            (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 :
            (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : 0));

        // Detectar suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);

        // Llamar a funciones de movimiento
        HandleMovement();
        HandleJump();
        //HandleClimb();
        HandleAttack();
    }

    private void HandleMovement()
    {
        if (isDead || isHurt || isClimbing) return;

        playerRb.linearVelocity = new Vector2(horizontalInput * speed, playerRb.linearVelocity.y);

        // Flip
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

    //private void HandleClimb()
    //{
    //    if (!onLadder)
    //    {
    //        isClimbing = false;
    //        anim.SetBool("Climbing", false);
    //        return;
    //    }

    //    isClimbing = true;
    //    playerRb.linearVelocity = new Vector2(horizontalInput * speed, verticalInput * speed);
    //    anim.SetBool("Climbing", verticalInput != 0);

    //    if (onLadder)
    //        playerRb.gravityScale = 0;
    //    else
    //        playerRb.gravityScale = 1;

    //}

    private void HandleAttack()
    {
        if (isDead || isHurt || isClimbing) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            anim.SetTrigger("Attack_Sword");
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            anim.SetTrigger("Attack_Thunder");
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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
