using System;
using UnityEngine;

public class SpeedModifierManager : MonoBehaviour
{
    // 가산/배율 누적값 캐시
    private float addSum = 0f;
    private float mulProduct = 1f;

    // 값 변경 시 PlayerController 등에 알림
    public event Action<float, float> ModifiersChanged;

    public void AddModifier(float addDelta, float mulDelta)
    {
        addSum += addDelta;
        mulProduct *= mulDelta;

        // 변경 시점에만 이벤트 발행
        ModifiersChanged?.Invoke(addSum, mulProduct);
    }

    public void GetCurrentValues(out float currentAdd, out float currentMul)
    {
        currentAdd = addSum;
        currentMul = mulProduct;
    }

    public void RequestCurrentValues()
    {
        ModifiersChanged?.Invoke(addSum, mulProduct);
    }
}
