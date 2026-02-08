using MazeGame.Items.Core;
using MazeGame.Items.Data;
using UnityEngine;

namespace MazeGame.Items.Effects
{
    [CreateAssetMenu(
        menuName = "MazeGame/Items/Effects/Max Hp Multiplier Effect",
        fileName = "MaxHpMultiplierEffect"
    )]
    public class MaxHpMultiplierEffect : ItemEffectBase
    {
        [Header("Max Hp Multiplier\n예: 0.8 = 20% 감소, 1.2 = 20% 증가")]
        [SerializeField] private float maxHpMultiplier = 1f;

        public override bool TryExecute(ItemUseContext context)
        {
            if (context.PlayerObject == null)
                return false;

            HealthManager health = context.PlayerObject.GetComponent<HealthManager>();
            if (health == null)
                return false;

            // 현재 최대 체력 기준으로 배율 적용 -> delta로 변환해서 AddMaxHp 호출
            float currentMax = health.maxHp;
            float newMax = currentMax * maxHpMultiplier;
            float delta = newMax - currentMax;

            health.AddMaxHp(delta);
            return true;
        }
    }
}
