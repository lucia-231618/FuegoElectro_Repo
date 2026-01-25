using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("No hay GameManager!");
            return instance;
        }
    }

    public float playerHealth;
    public float maxHealth = 100;
    public int playerPoints;

    private bool isPlayerDead = false; // Para que solo se ejecute una vez

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicializar vida al máximo cuando empieza el juego
        playerHealth = maxHealth;
    }

    private void Update()
    {
        // Limitar la vida
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);

        // Detectar muerte
        if (playerHealth <= 0 && !isPlayerDead)
        {
            isPlayerDead = true;
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
    }

    // Método opcional para reiniciar variables al reintentar
    public void ResetGame()
    {
        playerHealth = maxHealth;
        playerPoints = 0;
        isPlayerDead = false;
        Time.timeScale = 1f;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "NivelJugable") // Reemplaza con el nombre de tu escena
        {
            // Desactiva Main Camera y activa PlayerCam
            Camera.main.gameObject.SetActive(false);
            GameObject playerCam = GameObject.Find("PlayerCam"); // O usa una referencia directa
            if (playerCam != null)
            {
                playerCam.SetActive(true);
            }
        }
    }
}