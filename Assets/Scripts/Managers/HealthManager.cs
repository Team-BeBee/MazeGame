using System;
using UnityEngine;

public enum HealthPhase
{
    Normal,
    Injured,
    Severe,
    Critical,
    Dead
}

public class HealthManager : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("아이템 등으로 변경될 수 있는 최대 체력")]
    public float maxHp = 100f;

    [Tooltip("현재 체력(최대 체력 초과 불가)")]
    public float currentHp = 100f;

    // 사망 이벤트 중복 호출 방지 플래그
    private bool isDead = false;
    private HealthPhase currentPhase = HealthPhase.Normal;

    // 체력/최대 체력 변경 알림
    public event Action<float, float, float> HealthChanged;

    // 체력 상태 구간 변경 알림
    public event Action<HealthPhase> PhaseChanged;

    // 사망 이벤트(1회)
    public event Action Died;

    private void Awake()
    {
        ClampCurrentHp();
        isDead = currentHp <= 0f;
        currentPhase = EvaluatePhase();
    }

    /// <summary>
    /// 체력 변경 적용(delta < 0: 데미지, delta > 0: 회복)
    /// </summary>
    public void ApplyDelta(float delta)
    {
        currentHp += delta;
        ClampCurrentHp();
        NotifyHealthChanged();
        TryInvokeDeath();
    }

    /// <summary>
    /// 최대 체력 증감
    /// </summary>
    public void AddMaxHp(float delta)
    {
        maxHp += delta;
        ClampCurrentHp();
        NotifyHealthChanged();
        TryInvokeDeath();
    }

    /// <summary>
    /// 외부 구독자를 위한 현재 상태 동기화
    /// </summary>
    public void RequestCurrentValues()
    {
        float ratio = CalculateRatio();
        currentPhase = EvaluatePhase(ratio);
        HealthChanged?.Invoke(currentHp, maxHp, ratio);
        PhaseChanged?.Invoke(currentPhase);
    }

    private void NotifyHealthChanged()
    {
        float ratio = CalculateRatio();
        HealthPhase nextPhase = EvaluatePhase(ratio);

        HealthChanged?.Invoke(currentHp, maxHp, ratio);

        if (nextPhase != currentPhase)
        {
            currentPhase = nextPhase;
            PhaseChanged?.Invoke(currentPhase);
        }
    }

    private float CalculateRatio()
    {
        if (Mathf.Approximately(maxHp, 0f))
            return 0f;

        return currentHp / maxHp;
    }

    private HealthPhase EvaluatePhase()
    {
        return EvaluatePhase(CalculateRatio());
    }

    private HealthPhase EvaluatePhase(float ratio)
    {
        if (currentHp <= 0f)
            return HealthPhase.Dead;
        if (ratio > 0.8f)
            return HealthPhase.Normal;
        if (ratio > 0.6f)
            return HealthPhase.Injured;
        if (ratio > 0.3f)
            return HealthPhase.Severe;
        if (ratio > 0f)
            return HealthPhase.Critical;

        return HealthPhase.Critical;
    }

    private void ClampCurrentHp()
    {
        currentHp = Mathf.Min(currentHp, maxHp);
    }

    private void TryInvokeDeath()
    {
        if (isDead)
            return;

        if (currentHp <= 0f)
        {
            isDead = true;
            Died?.Invoke();
        }
    }
}
