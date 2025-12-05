using System.Collections.Generic;
using UnityEngine;

public class PenaltyManager : MonoBehaviour
{
    public static PenaltyManager Instance;

    [Header("페널티 카운터")]
    public int LevelPenalty { get; private set; } = 0;
    public int GlobalPenalty { get; private set; } = 0; // 향후 글로벌 페널티용

    [Header("설정")]
    public int penaltyThreshold = 3; // 이 값에 도달하면 문을 변환
    public int convertCount = 1;     // 한 번에 변환할 가짜 문의 수

    // 등록된 가짜 문 목록
    private readonly List<DoorBase> fakeDoors = new List<DoorBase>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // MazeRenderer나 DoorSpawner가 가짜 문을 배치할 때 호출
    public void RegisterFakeDoor(DoorBase door)
    {
        if (door != null && !fakeDoors.Contains(door))
        {
            fakeDoors.Add(door);
        }
    }

    public void AddLocalPenalty()
    {
        LevelPenalty++;
        Debug.Log($"[PenaltyManager] 로컬 페널티 증가: {LevelPenalty}");

        if (LevelPenalty >= penaltyThreshold)
        {
            ConvertFakeDoorsToHorror();
            LevelPenalty = 0; // 변환 후 리셋
        }
    }

    public void ResetLocalPenalty()
    {
        LevelPenalty = 0;
        Debug.Log("[PenaltyManager] 로컬 페널티 리셋.");
    }

    private void ConvertFakeDoorsToHorror()
    {
        if (fakeDoors.Count == 0)
        {
            Debug.LogWarning("[PenaltyManager] 등록된 가짜 문이 없습니다.");
            return;
        }

        int count = Mathf.Min(convertCount, fakeDoors.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, fakeDoors.Count);
            DoorBase selected = fakeDoors[index];

            GameObject doorObj = selected.gameObject;

            // 가짜 문 컴포넌트 제거
            Destroy(selected);

            // 공포 문으로 교체
            HorrorDoor horror = doorObj.AddComponent<HorrorDoor>();
            horror.doorType = DoorType.Horror;

            fakeDoors.RemoveAt(index);
        }

        Debug.Log($"[PenaltyManager] 가짜 문 {count}개를 공포 문으로 변환했습니다.");
    }
}
