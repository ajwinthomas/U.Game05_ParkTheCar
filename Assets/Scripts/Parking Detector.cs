using UnityEngine;
using System.Collections;

public class ParkingDetector : MonoBehaviour
{
    // the time that car should be in the parking zone.
    public float requiredTimeInzone = 3f;

    public GameObject winPanel;

    public GameObject car;
    private CarController controller;
    public AudioSource gameAudio1;
    public AudioSource gameAudio2;

    private float timer = 0f;
    private bool carInZone = false;

    private void Start()
    {
        winPanel.SetActive(false);
        if (car != null)
            controller = car.GetComponent<CarController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarSensor"))
        {
            Debug.Log("Car entered the parking zone");
            carInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CarSensor"))
        {
            Debug.Log("Car left from the parking zone");
            carInZone = false;
            timer = 0f;//reset the time if car is out of zone
        }
    }

    private void Update()
    {
        if (carInZone)
        {
            timer += Time.deltaTime;

            if (timer >= requiredTimeInzone)
            {
                Win();
            }

        }
    }

    void Win()
    {
        Debug.Log("Car parked ");

        if (winPanel != null)
        {
            winPanel.SetActive(true);

            carInZone = false;
            timer = 0f;
        }

        if (controller != null)
            controller.enabled = false;

        if (gameAudio2 != null)
            gameAudio2.Stop();

        if (gameAudio1 != null)
            gameAudio1.Stop();

        StartCoroutine(freezeDelay(1f));
    }

    IEnumerator freezeDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f;

    }
}