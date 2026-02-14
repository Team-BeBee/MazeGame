using MazeGame.Items.Data;

namespace MazeGame.Items.Runtime
{
    // 슬롯에 들어가는 아이템 스택
    [System.Serializable]
    public struct ItemStack
    {
        public ItemDefinition Definition;
        public int Count;

        public bool IsEmpty => Definition == null || Count <= 0;

        public void Clear()
        {
            Definition = null;
            Count = 0;
        }
    }
}
