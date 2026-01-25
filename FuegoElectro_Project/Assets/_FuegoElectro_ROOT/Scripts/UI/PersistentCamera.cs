using UnityEngine;

public class PersistentCamera : MonoBehaviour
{
    void Awake()
    {
        // Evita que este objeto se destruya al cargar nuevas escenas
        DontDestroyOnLoad(gameObject);
    }
}