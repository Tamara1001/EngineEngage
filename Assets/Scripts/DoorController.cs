using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }   
    }

    private void ToggleDoor()
    {
        doorAnimator.SetBool("OpenDoor", !doorAnimator.GetBool("OpenDoor"));
    }

    private void OpenDoor()
    {
        doorAnimator.SetBool("OpenDoor", true);
    }

    private void CloseDoor()
    {
        doorAnimator.SetBool("OpenDoor", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseDoor();
        }
    }
}
