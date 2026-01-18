using UnityEngine;

namespace MazeGame.Items.Data
{
    // 소모성 아이템 정의
    [CreateAssetMenu(menuName = "MazeGame/Items/Consumable")]
    public class ConsumableItemDefinition : ItemDefinition
    {
        [Header("소모성 효과")]
        [SerializeField] private ItemEffectBase effect;

        public ItemEffectBase Effect => effect;
    }
}
