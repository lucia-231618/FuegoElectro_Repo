using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 10;
    private bool hasDamaged = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDamaged) return;

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("¡Golpeó a: " + collision.name + "!");
            collision.GetComponent<EnemyController>()?.TakeDamage(damage);
            hasDamaged = true; // Solo una vez por ataque
        }
    }

    // Método para resetear el flag al inicio de un nuevo ataque
    public void ResetDamage()
    {
        hasDamaged = false;
    }
}