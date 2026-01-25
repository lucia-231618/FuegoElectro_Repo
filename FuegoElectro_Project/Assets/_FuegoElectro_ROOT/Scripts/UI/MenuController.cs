using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Botón Jugar
    public void PlayGame()
    {
        SceneController.Instance.LoadScene("StoryIntro");
    }

    // Botón Salir
    public void ExitGame()
    {
        Application.Quit();
    }
}
