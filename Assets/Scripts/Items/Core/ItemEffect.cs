namespace MazeGame.Items.Core
{
    // 아이템 효과 인터페이스
    public interface ItemEffect
    {
        bool TryExecute(ItemUseContext context);
    }
}
