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

            // Verificar si es un enemigo agresivo
            AggressiveEnemyController aggressiveEnemy = collision.GetComponent<AggressiveEnemyController>();
            if (aggressiveEnemy != null)
            {
                aggressiveEnemy.TakeDamage(damage);
                hasDamaged = true;
                return;
            }

            // Verificar si es un enemigo defensivo
            DefensiveEnemyController defensiveEnemy = collision.GetComponent<DefensiveEnemyController>();
            if (defensiveEnemy != null)
            {
                defensiveEnemy.TakeDamage(damage);
                hasDamaged = true;
                return;
            }

            // Si no tiene ninguno, log de error (opcional)
            Debug.LogWarning("El objeto " + collision.name + " tiene tag 'Enemy' pero no tiene AggressiveEnemyController ni DefensiveEnemyController.");
        }
    }

    // Método para resetear el flag al inicio de un nuevo ataque
    public void ResetDamage()
    {
        hasDamaged = false;
    }
}