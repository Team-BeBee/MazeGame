using UnityEngine;

public class SpeedModifierTrap : MonoBehaviour
{
    [Header("Speed Modifier")]
    [SerializeField] private float addDelta = 0f;
    [SerializeField] private float mulDelta = 1f;

    private void OnTriggerEnter(Collider other)
    {
        // 트리거 진입 시 1회 누적 적용
        SpeedModifierManager manager = other.GetComponentInParent<SpeedModifierManager>();
        if (manager == null)
            return;

        manager.AddModifier(addDelta, mulDelta);
    }
}
