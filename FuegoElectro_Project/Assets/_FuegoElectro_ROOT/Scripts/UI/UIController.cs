using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider healthBar;
    public Text pointsText;

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            healthBar.value = GameManager.Instance.playerHealth / GameManager.Instance.maxHealth;
            pointsText.text = GameManager.Instance.playerPoints.ToString();
        }
    }
}
