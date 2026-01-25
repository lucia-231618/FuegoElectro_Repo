using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    private void Start()
    {
        totalhealthBar.fillAmount = GameManager.Instance.playerHealth / GameManager.Instance.maxHealth;
    }
    private void Update()
    {
        currenthealthBar.fillAmount = GameManager.Instance.playerHealth / GameManager.Instance.maxHealth;
    }
}