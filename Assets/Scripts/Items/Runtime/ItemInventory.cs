using UnityEngine;

namespace MazeGame.Items.Runtime
{
    // 플레이어 인벤토리 루트
    public class ItemInventory : MonoBehaviour
    {
        [SerializeField] private QuickSlotManager quickSlotManager;

        public QuickSlotManager QuickSlotManager => quickSlotManager;

        private void Reset()
        {
            quickSlotManager = GetComponent<QuickSlotManager>();
        }
    }
}
