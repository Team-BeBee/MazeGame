namespace MazeGame.Items.Core
{
    // 아이템 입력 차단 인터페이스
    public interface ItemInputBlocker
    {
        bool IsItemSelectionBlocked();
        bool IsItemUseBlocked();
    }
}
