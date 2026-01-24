using UnityEngine;
using UnityEngine.InputSystem;

public class AttackSystem : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private float radioGolpe = 0.5f;
    [SerializeField] private int dañoGolpe = 1;
    [SerializeField] private float tiempoEntreAtaques = 0.5f;

    private float tiempoSiguienteAtaque;
    private Animator animator;
    private PlayerController player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (tiempoSiguienteAtaque > 0)
            tiempoSiguienteAtaque -= Time.deltaTime;

        // Ataque básico (botón izquierdo)
        if (Mouse.current.leftButton.wasPressedThisFrame && tiempoSiguienteAtaque <= 0)
        {
            Golpe();
            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
    }

    private void Golpe()
    {
        // Seguridad extra
        if (player == null) return;

        animator.SetTrigger("Attack_Sword");

        Collider2D[] objetos = Physics2D.OverlapCircleAll(
            controladorGolpe.position,
            radioGolpe
        );

        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemy"))
            {
                EnemyHealth enemy = colisionador.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(dañoGolpe);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (controladorGolpe == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
    }
}

