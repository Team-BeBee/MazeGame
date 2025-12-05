using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalDoor : DoorBase
{
    [Header("골 문 설정")]
    public bool playOpenAnimation = true;
    public bool playSound = true;

    protected override void OnDoorOpened()
    {
        // 선택적으로 애니메이션 재생
        if (playOpenAnimation)
        {
            PlayOpenAnimation();
        }

        // 선택적으로 사운드 재생
        if (playSound)
        {
            PlayOpenSound();
        }

        // 레벨 종료로 인한 로컬 페널티 초기화
        PenaltyManager.Instance.ResetLocalPenalty();

        // 레벨 클리어 처리
        ClearLevel();

        Debug.Log("[GoalDoor] 골 문이 열렸습니다. 레벨 클리어 처리 완료.");
    }

    private void ClearLevel()
    {
        // 현재는 동일 씬을 다시 로드 (차후 LevelManager에서 다음 층 로드 예정)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
    //     SoundManager.Instance.Play("goal_door_open");
    // }
}
