using UnityEngine;

namespace MazeGame.Items.Data
{
    // 스탯 아이템 정의 (효과는 추후 구현)
    [CreateAssetMenu(menuName = "MazeGame/Items/Stat")]
    public class StatItemDefinition : ItemDefinition
    {
        [Header("스탯 데이터")]
        [SerializeField] private string statKey;
        [SerializeField] private float statValue;

        public string StatKey => statKey;
        public float StatValue => statValue;
    }
}
