using UnityEngine;

public class StoryController : MonoBehaviour
{
    // Duración de la animación en segundos
    public float storyDuration = 10f; // Ajusta al tiempo de tu animación

    private void Start()
    {
        // Inicia la corutina que espera a que termine la historia
        StartCoroutine(PlayStory());
    }

    private System.Collections.IEnumerator PlayStory()
    {
        // Aquí podrías reproducir un Animator o Timeline si quieres
        // Ejemplo:
        // Animator anim = GetComponent<Animator>();
        // anim.Play("StoryAnimation");

        // Espera la duración de la animación
        yield return new WaitForSeconds(storyDuration);

        // Pasar a Level_01
        SceneController.Instance.LoadScene("Level_01");

        // Cargar overlay UI
        SceneController.Instance.LoadOverlay("OverlayUI");
    }
}