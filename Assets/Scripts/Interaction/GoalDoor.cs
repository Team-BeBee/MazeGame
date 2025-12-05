using UnityEngine;

public class GoalDoor : DoorBase
{
    [SerializeField]
    [Tooltip("골 도달 시 재생할 오디오 소스")]
    private AudioSource goalAudio;

    private void Reset()
    {
        doorType = DoorType.Goal;
    }

    protected override void OnDoorOpened()
    {
        if (goalAudio != null)
        {
            goalAudio.Play();
        }

        Debug.Log("골 문을 열어 클리어 처리를 실행했습니다.");
    }
}
