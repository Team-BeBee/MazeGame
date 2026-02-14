using UnityEngine;
using MazeGame.Items.Data;

namespace MazeGame.Items.Runtime
{
    // 맵에 배치되는 아이템 픽업
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemDefinition itemDefinition;
        [SerializeField] private int amount = 1;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (itemDefinition == null)
            {
                return;
            }

            ItemInventory inventory = other.GetComponentInParent<ItemInventory>();
            if (inventory == null || inventory.QuickSlotManager == null)
            {
                return;
            }

            bool added = inventory.QuickSlotManager.TryAddItem(itemDefinition, amount);
            if (added)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
