using UnityEngine;
using UnityEngine.SceneManagement;

public class HorrorDoor : DoorBase
{
    [Header("공포 문 설정")]
    public string horrorSceneName = "HorrorScene";
    public bool playOpenAnimation = true;
    public bool playSound = true;

    protected override void OnDoorOpened()
    {
        // 선택적으로 애니메이션 재생
        if (playOpenAnimation)
            PlayOpenAnimation();

        // 선택적으로 사운드 재생
        if (playSound)
            PlayOpenSound();

        // 페널티 추가
        PenaltyManager.Instance.AddLocalPenalty();

        // 공포 씬 로드
        LoadHorrorScene();

        Debug.Log("[HorrorDoor] 공포 문이 열렸습니다. 페널티 적용 후 씬 로드: " + horrorSceneName);
    }

    private void LoadHorrorScene()
    {
        // 즉시 씬 전환
        SceneManager.LoadScene(horrorSceneName);
    }

    protected override void PlayOpenAnimation()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Open");
    }

    // protected override void PlayOpenSound()
    // {
    //     SoundManager.Instance.Play("horror_door_open");
    // }
}
