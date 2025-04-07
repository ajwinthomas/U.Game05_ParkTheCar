using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    [Header("Timer settings")]
    public int startMinutes = 1;

    [Header("UI Reference")]
    public TextMeshProUGUI timerText;

    private float remainingTime;
    private bool isRunning = false;


    private void Start()
    {
        remainingTime = startMinutes * 60;
        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning) return;

        if(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if(remainingTime < 0)
            {
                remainingTime = 0;
            }

            UpdateTimerUI();
        }
        else
        {
            isRunning = false;
            Debug.Log("Count Down Finished");
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
