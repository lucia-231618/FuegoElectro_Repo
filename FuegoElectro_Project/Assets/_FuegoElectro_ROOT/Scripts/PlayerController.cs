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
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck;
    [SerializeField] LayerMask groundLayer;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(
    groundCheck.transform.position,
    0.1f,
    groundLayer
);

        Movement();
        Jump();
    }

    void Movement()
    {
        if (Keyboard.current == null) return;

        horizontalInput =
            (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0) +
            (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0);

        playerRb.linearVelocity = new Vector2(
            horizontalInput * speed,
            playerRb.linearVelocity.y
        );
    }

    void Jump()
    {
        if ((Keyboard.current.spaceKey.wasPressedThisFrame) && isGrounded)
        {
            playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}

