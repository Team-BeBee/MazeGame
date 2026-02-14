using UnityEngine;
using MazeGame.Items.Core;
using MazeGame.Items.Data;

namespace MazeGame.Items.Effects
{
    // 테스트용 아이템 효과 (항상 성공)
    [CreateAssetMenu(
        menuName = "MazeGame/Items/Effects/Log Test Effect",
        fileName = "LogTestEffect"
    )]
    public class LogTestEffect : ItemEffectBase
    {
        public override bool TryExecute(ItemUseContext context)
        {
            Debug.Log("아이템 사용 성공");

            return true; // 무조건 성공 처리
        }
    }
}
