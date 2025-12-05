using UnityEngine;
using System.Collections.Generic;

public class PenaltyManager : MonoBehaviour
{
    public static PenaltyManager Instance;

    [Header("Penalty Counters")]
    public int LevelPenalty { get; private set; } = 0;
    public int GlobalPenalty { get; private set; } = 0; // reserved for future

    [Header("Penalty Settings")]
    public int penaltyThreshold = 3;     // Required penalties before conversion
    public int convertCount = 1;         // Number of doors to convert at threshold

    private List<FakeDoor> registeredFakeDoors = new List<FakeDoor>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Called by MazeRenderer as FakeDoors are spawned
    public void RegisterFakeDoor(DoorBase door)
    {
        FakeDoor fake = door as FakeDoor;
        if (fake != null && !registeredFakeDoors.Contains(fake))
        {
            registeredFakeDoors.Add(fake);
        }
    }

    public void AddLocalPenalty()
    {
        LevelPenalty++;
        Debug.Log("[PenaltyManager] Local penalty increased: " + LevelPenalty);

        if (LevelPenalty >= penaltyThreshold)
        {
            ConvertFakeDoorsToHorror();
            LevelPenalty = 0; // Reset for next cycle
        }
    }

    public void ResetLocalPenalty()
    {
        LevelPenalty = 0;
        Debug.Log("[PenaltyManager] Local penalty reset.");
    }

    private void ConvertFakeDoorsToHorror()
    {
        if (registeredFakeDoors.Count == 0)
        {
            Debug.LogWarning("[PenaltyManager] No FakeDoors available to convert.");
            return;
        }

        int count = Mathf.Min(convertCount, registeredFakeDoors.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, registeredFakeDoors.Count);
            FakeDoor fake = registeredFakeDoors[index];

            GameObject obj = fake.gameObject;

            // Remove FakeDoor
            Destroy(fake);

            // Add HorrorDoor
            HorrorDoor horrorDoor = obj.AddComponent<HorrorDoor>();
            horrorDoor.doorType = DoorType.Horror;

            registeredFakeDoors.RemoveAt(index);

            Debug.Log("[PenaltyManager] Converted FakeDoor to HorrorDoor.");
        }
    }
}
