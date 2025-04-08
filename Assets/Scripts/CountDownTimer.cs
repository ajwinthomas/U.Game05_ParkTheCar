using System.Collections;
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

    public GameObject LosePanel;
    public GameObject car;
    private CarController controller;
    public AudioSource  gameAudio1; 
    public AudioSource gameAudio2;



    private void Start()
    {
        remainingTime = startMinutes * 60;
        isRunning = true;
        if (LosePanel != null)
        {
            LosePanel.SetActive(false);
            Time.timeScale = 1f;
        }
        if(car != null)
        {
            controller = car.GetComponent<CarController>();   
        }
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
            Debug.Log("Gameover");
            Lose();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void Lose()
    {
        LosePanel.SetActive(true);

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (gameAudio1 != null)
              gameAudio1.Stop();
        if (gameAudio2 != null)
            gameAudio2.Stop();


        StartCoroutine(freezeDelay(1f));
    }


    IEnumerator freezeDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f;

    }
}
