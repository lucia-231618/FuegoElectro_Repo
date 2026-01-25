using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage = 10;
    private bool hasDamaged = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hitbox - OnTriggerEnter2D llamado con: " + collision.name + ", tag: " + collision.tag + ", layer: " + collision.gameObject.layer);  // Log agregado

        if (hasDamaged)
        {
            Debug.Log("Hitbox - hasDamaged es true, ignorando daño");  // Log agregado
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Hitbox - ¡Golpeó a: " + collision.name + "!");

            // Verificar si es un enemigo agresivo
            AggressiveEnemyController aggressiveEnemy = collision.GetComponent<AggressiveEnemyController>();
            if (aggressiveEnemy != null)
            {
                aggressiveEnemy.TakeDamage(damage);
                hasDamaged = true;
                Debug.Log("Hitbox - Daño aplicado a agresivo: " + collision.name);  // Log agregado
                return;
            }

            // Verificar si es un enemigo defensivo
            DefensiveEnemyController defensiveEnemy = collision.GetComponent<DefensiveEnemyController>();
            if (defensiveEnemy != null)
            {
                defensiveEnemy.TakeDamage(damage);
                hasDamaged = true;
                Debug.Log("Hitbox - Daño aplicado a defensivo: " + collision.name);  // Log agregado
                return;
            }

            // Si no tiene ninguno, log de error
            Debug.LogWarning("Hitbox - El objeto " + collision.name + " tiene tag 'Enemy' pero no tiene controlador de enemigo.");
        }
        else
        {
            Debug.Log("Hitbox - Colisión con objeto no 'Enemy': " + collision.name);  // Log agregado
        }
    }

    // Método para resetear el flag al inicio de un nuevo ataque
    public void ResetDamage()
    {
        hasDamaged = false;
    }
}