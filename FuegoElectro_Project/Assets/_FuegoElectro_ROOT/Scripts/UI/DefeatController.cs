using UnityEngine;

public class DefeatController : MonoBehaviour
{
    // Botón Reintentar
    public void Retry()
    {
        GameManager.Instance.ResetGame();
        SceneController.Instance.ReloadCurrentScene();
    }
}