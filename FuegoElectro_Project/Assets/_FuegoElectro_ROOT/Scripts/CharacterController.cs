using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 5f;          // Velocidad horizontal
    public float jumpForce = 10f;     // Fuerza del salto
    public LayerMask groundLayer;     // Capa del suelo para raycast

    private Rigidbody2D rb;
    private bool isGrounded;
    private float groundCheckDistance = 0.1f;  // Distancia para detectar suelo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("El GameObject necesita un Rigidbody2D.");
        }
    }

    void Update()
    {
        // Movimiento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Detección de suelo con raycast (más precisa que OnCollision)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    // Opcional: Dibujar el raycast en el editor para depuración
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
