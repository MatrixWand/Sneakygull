using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int remainingTargets;
    private bool gameEnded = false;

    [SerializeField] private TextMeshProUGUI targetsText; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Damageable[] targets = FindObjectsByType<Damageable>(FindObjectsSortMode.None);
        remainingTargets = targets.Length;

        UpdateUI(); 
    }

    public void TargetDestroyed()
    {
        if (gameEnded) return;

        remainingTargets--;

        UpdateUI(); 

        if (remainingTargets <= 0)
        {
            WinGame();
        }
    }

    private void UpdateUI()
    {
        if (targetsText != null)
        {
            targetsText.text = "Targets Left: " + remainingTargets;
        }
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        UIManager.Instance.ShowLose();
    }

    private void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        UIManager.Instance.ShowWin();
    }
}