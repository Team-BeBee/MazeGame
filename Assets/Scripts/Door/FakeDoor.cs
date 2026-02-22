using UnityEngine;

public class FakeDoor : DoorBase
{
    [Header("가짜 문 설정")]
    public bool playOpenAnimation = true;
    public bool playSound = true;

    protected override void OnDoorOpened()
    {
        // 선택적으로 오픈 애니메이션 재생
        if (playOpenAnimation)
        {
            PlayOpenAnimation();
        }

        // 선택적으로 사운드 재생
        if (playSound)
        {
            PlayOpenSound();
        }

        // 페널티 적용
        PenaltyManager.Instance.AddLocalPenalty();

        // 플레이어를 미로의 임의 위치로 이동
        TeleportUtility.TeleportPlayerRandom();

    }

    protected override void PlayOpenAnimation()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Open");
        }
    }

    // protected override void PlayOpenSound()
    // {
    //     SoundManager.Instance.Play("fake_door_open");
    // }
}
