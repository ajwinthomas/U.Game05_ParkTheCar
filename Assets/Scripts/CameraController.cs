using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Rigidbody playerRB;
    public Vector3 offset;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        playerRB.AddForce(offset);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerForward = (playerRB.linearVelocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position,
            player.position + player.transform.TransformVector(offset) + playerForward * (-5f),speed * Time.deltaTime);
        transform.LookAt(player);
    }
    //camera script . tommorow continued .....
}
