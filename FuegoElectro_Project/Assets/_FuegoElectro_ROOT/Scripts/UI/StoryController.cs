using UnityEngine;
using UnityEngine.SceneManagement; // Para manejar escenas

public class StoryController : MonoBehaviour
{
    // Duración de la animación en segundos
    public float storyDuration = 10f; // Ajusta al tiempo de tu animación

    private bool skipped = false; // Para evitar que se ejecute dos veces

    private void Start()
    {
        // Inicia la corutina que espera a que termine la historia
        StartCoroutine(PlayStory());
    }

    private System.Collections.IEnumerator PlayStory()
    {
        float timer = 0f;

        // Mientras no se haya saltado y el tiempo no se haya acabado
        while (timer < storyDuration && !skipped)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Lógica para cambiar de escena y cargar overlay
        SkipToNextScene();
    }

    // Función que se llama desde el botón "Skip"
    public void Skip()
    {
        skipped = true; // Detiene la espera de la corutina
    }

    // Función que maneja el cambio de escena
    private void SkipToNextScene()
    {
        // Evitar que se ejecute varias veces
        if (skipped)
            skipped = false;

        // Aquí pasamos a Level_01 usando tu SceneController
        SceneController.Instance.LoadScene("Level_01");

        // Cargar overlay UI
        SceneController.Instance.LoadOverlay("OverlayUI");
    }
}
