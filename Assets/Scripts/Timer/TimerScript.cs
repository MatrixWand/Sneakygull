using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public float time;
    public TextMeshProUGUI timerText;

    private bool hasLost = false;

    private void Start()
    {
        time = 300f;
    }

    void Update()
    {
        if (hasLost) return;

        if (time > 0)
        {
            time -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";

            if (time <= 10f)
            {
                timerText.color = Color.red;
            }
        }
        else
        {
            time = 0;
            timerText.text = "00:00";

            hasLost = true;

           
            GameManager gm = FindObjectOfType<GameManager>();

            if (gm != null)
            {
                gm.LoseGame();
            }
            else
            {
                Debug.LogError("GameManager not found in scene!");
            }
        }
    }
}