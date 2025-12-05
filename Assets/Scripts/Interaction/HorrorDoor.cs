using UnityEngine;

public class HorrorDoor : DoorBase
{
    [SerializeField]
    [Tooltip("공포 효과를 재생할 오디오 소스")]
    private AudioSource horrorAudio;

    private void Reset()
    {
        doorType = DoorType.Horror;
    }

    protected override void OnDoorOpened()
    {
        if (horrorAudio != null)
        {
            horrorAudio.Play();
        }

        Debug.Log("공포 문을 열어 공포 효과를 재생했습니다.");
    }
}
