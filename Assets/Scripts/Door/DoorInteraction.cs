using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Prompt")]
    public GameObject interactionUI;

    private DoorBase currentDoor = null;

    private void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void Update()
    {
        if (currentDoor != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                currentDoor.Open();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DoorBase door = other.GetComponent<DoorBase>();
        if (door != null)
        {
            currentDoor = door;

            if (interactionUI != null)
                interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DoorBase door = other.GetComponent<DoorBase>();
        if (door != null && door == currentDoor)
        {
            currentDoor = null;

            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
    }
}
