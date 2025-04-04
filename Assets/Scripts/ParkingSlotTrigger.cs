using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ParkingSlotTrigger : MonoBehaviour
{
    public Transform centerLock;
    public TextMeshProUGUI parkingtext;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Lock"))
        {
            ParkingCompleted();
        }
    }

    void ParkingCompleted()
    {
        parkingtext.text = "Parking Completed";
    }
}
