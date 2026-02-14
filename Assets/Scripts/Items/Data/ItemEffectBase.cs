using UnityEngine;
using MazeGame.Items.Core;

namespace MazeGame.Items.Data
{
    // 아이템 효과 베이스
    public abstract class ItemEffectBase : ScriptableObject, ItemEffect
    {
        public abstract bool TryExecute(ItemUseContext context);
    }
}
