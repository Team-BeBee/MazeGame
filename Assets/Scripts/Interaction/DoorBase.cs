using UnityEngine;

public enum DoorType
{
    Fake,
    Horror,
    Goal
}

public abstract class DoorBase : MonoBehaviour
{
    [Header("문 종류")]
    public DoorType doorType;

    // PlayerInteraction에서 호출함
    public void Open()
    {
        OnDoorOpened();
    }

    // 자식 클래스에서 문이 열릴 때 처리
    protected abstract void OnDoorOpened();

    // 자식 클래스에서 필요하면 오픈 애니메이션 구현
    protected virtual void PlayOpenAnimation() { }

    // 자식 클래스에서 필요하면 오픈 사운드 구현
    protected virtual void PlayOpenSound() { }
}
