using UnityEngine;

public class EngineAudio : MonoBehaviour
{

    public AudioSource runningSound;
    public float runningMaxVolume;
    public float runningMaxPitch;
    public AudioSource idleSound;
    public float idleMaxVolume;
    private float SpeedRatio;

    private CarController carController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (carController)
        {
            SpeedRatio = carController.GetSpeedRatio();
        }
        idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume,SpeedRatio);
        runningSound.volume = Mathf.Lerp(0.3f,runningMaxVolume,SpeedRatio);
        runningSound.pitch = Mathf.Lerp(runningSound.pitch,Mathf.Lerp(0.3f, runningMaxPitch,SpeedRatio),Time.deltaTime);
    }
}
