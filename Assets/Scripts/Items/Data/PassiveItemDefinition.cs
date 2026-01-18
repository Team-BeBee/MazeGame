using UnityEngine;

namespace MazeGame.Items.Data
{
    // 패시브 아이템 정의 (효과는 추후 구현)
    [CreateAssetMenu(menuName = "MazeGame/Items/Passive")]
    public class PassiveItemDefinition : ItemDefinition
    {
        [Header("패시브 설명")]
        [SerializeField] private string passiveDescription;

        public string PassiveDescription => passiveDescription;
    }
}
