using MazeGame.Items.Core;
using MazeGame.Items.Data;
using UnityEngine;

namespace MazeGame.Items.Effects
{
    [CreateAssetMenu(
        menuName = "MazeGame/Items/Effects/Current Hp Modifier Effect",
        fileName = "CurrentHpModifierEffect"
    )]

    public class CurrentHpModifierEffect : ItemEffectBase
    {
        [Header("Current Hp Modifier\n현재 체력 변화량 (+회복 / -데미지)")]
      
        [SerializeField] private float healAmount = 0f;

        public override bool TryExecute(ItemUseContext context)
        {
            if (context.PlayerObject == null)
                return false;

            HealthManager health = context.PlayerObject.GetComponent<HealthManager>();
            if (health == null)
                return false;

            health.ApplyDelta(healAmount);
            return true;
        }
    }
}
