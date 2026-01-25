using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vida = 3;

    public void TakeDamage(int daño)
    {
        vida -= daño;

        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }
}