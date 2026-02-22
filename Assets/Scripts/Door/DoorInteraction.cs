using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public LayerMask doorLayerMask = ~0;

    [Header("UI Prompt")]
    public GameObject interactionUI;

    private DoorBase currentDoor = null;

    private void Start()
    {
        // 카메라가 비어 있으면 메인 카메라를 자동으로 사용
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    private void Update()
    {
        UpdateCurrentDoorByRaycast();

        if (currentDoor != null && Input.GetKeyDown(interactKey))
        {
            currentDoor.Open();
        }
    }

    private void UpdateCurrentDoorByRaycast()
    {
        DoorBase nextDoor = null;

        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            // 카메라 정면으로 쏜 레이캐스트에 맞은 DoorBase만 상호작용 대상으로 사용
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, doorLayerMask, QueryTriggerInteraction.Collide))
            {
                nextDoor = hit.collider.GetComponentInParent<DoorBase>();
            }
        }

        currentDoor = nextDoor;

        if (interactionUI != null)
            interactionUI.SetActive(currentDoor != null);
    }
}