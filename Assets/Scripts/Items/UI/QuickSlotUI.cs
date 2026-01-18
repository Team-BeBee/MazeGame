using UnityEngine;
using UnityEngine.UI;
using MazeGame.Items.Runtime;

namespace MazeGame.Items.UI
{
    // 퀵드로우 슬롯 UI
    public class QuickSlotUI : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private QuickSlotManager quickSlotManager;

        [Header("슬롯 이미지")]
        [SerializeField] private Image[] slotIcons;
        [SerializeField] private Image[] slotFrames;

        [Header("선택 표시")]
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color defaultColor = Color.gray;

        private void OnEnable()
        {
            if (quickSlotManager == null)
            {
                return;
            }

            quickSlotManager.SlotsChanged += RefreshAll;
            quickSlotManager.SelectionChanged += RefreshSelection;
            quickSlotManager.SlotChanged += RefreshSlot;

            RefreshAll();
            RefreshSelection(quickSlotManager.SelectedIndex);
        }

        private void OnDisable()
        {
            if (quickSlotManager == null)
            {
                return;
            }

            quickSlotManager.SlotsChanged -= RefreshAll;
            quickSlotManager.SelectionChanged -= RefreshSelection;
            quickSlotManager.SlotChanged -= RefreshSlot;
        }

        private void RefreshAll()
        {
            if (quickSlotManager == null || slotIcons == null)
            {
                return;
            }

            int max = Mathf.Min(slotIcons.Length, quickSlotManager.SlotCount);
            for (int i = 0; i < max; i++)
            {
                RefreshSlot(i, quickSlotManager.GetSlot(i));
            }
        }

        private void RefreshSlot(int index, ItemStack stack)
        {
            if (slotIcons == null || index < 0 || index >= slotIcons.Length)
            {
                return;
            }

            Image icon = slotIcons[index];
            if (icon == null)
            {
                return;
            }

            if (stack.IsEmpty || stack.Definition == null)
            {
                icon.enabled = false;
                icon.sprite = null;
                return;
            }

            icon.enabled = true;
            icon.sprite = stack.Definition.Icon;
        }

        private void RefreshSelection(int selectedIndex)
        {
            if (slotFrames == null)
            {
                return;
            }

            for (int i = 0; i < slotFrames.Length; i++)
            {
                Image frame = slotFrames[i];
                if (frame == null)
                {
                    continue;
                }

                frame.color = i == selectedIndex ? selectedColor : defaultColor;
            }
        }
    }
}
