using System;
using UnityEngine;
using MazeGame.Items.Core;
using MazeGame.Items.Data;

namespace MazeGame.Items.Runtime
{
    // 퀵드로우 슬롯 관리
    public class QuickSlotManager : MonoBehaviour
    {
        public enum OverflowPolicy
        {
            ReplaceSelected,
            Fifo
        }

        [Header("슬롯 설정")]
        [SerializeField] private int slotCount = 4;
        [SerializeField] private OverflowPolicy overflowPolicy = OverflowPolicy.ReplaceSelected;

        private ItemStack[] slots;
        private int selectedIndex;

        public event Action SlotsChanged;
        public event Action<int> SelectionChanged;
        public event Action<int, ItemStack> SlotChanged;

        public int SlotCount => slots?.Length ?? 0;
        public int SelectedIndex => selectedIndex;

        private void Awake()
        {
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            slotCount = Mathf.Max(1, slotCount);
            slots = new ItemStack[slotCount];
            selectedIndex = Mathf.Clamp(selectedIndex, 0, slotCount - 1);
        }

        public ItemStack GetSlot(int index)
        {
            if (slots == null || index < 0 || index >= slots.Length)
            {
                return new ItemStack();
            }

            return slots[index];
        }

        public void SetSelectedIndex(int index)
        {
            if (slots == null || slots.Length == 0)
            {
                return;
            }

            int clamped = Mathf.Clamp(index, 0, slots.Length - 1);
            if (clamped == selectedIndex)
            {
                return;
            }

            selectedIndex = clamped;
            SelectionChanged?.Invoke(selectedIndex);
        }

        public void CycleSelection(int delta)
        {
            if (slots == null || slots.Length == 0)
            {
                return;
            }

            if (delta == 0)
            {
                return;
            }

            int newIndex = (selectedIndex + delta) % slots.Length;
            if (newIndex < 0)
            {
                newIndex += slots.Length;
            }

            SetSelectedIndex(newIndex);
        }

        public bool TryAddItem(ItemDefinition definition, int amount)
        {
            if (definition == null || amount <= 0)
            {
                return false;
            }

            if (slots == null)
            {
                InitializeSlots();
            }

            if (definition.IsStackable)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].Definition == definition && slots[i].Count < definition.MaxStack)
                    {
                        int addable = Mathf.Min(amount, definition.MaxStack - slots[i].Count);
                        slots[i].Count += addable;
                        amount -= addable;
                        SlotChanged?.Invoke(i, slots[i]);
                        SlotsChanged?.Invoke();
                        if (amount <= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty)
                {
                    slots[i].Definition = definition;
                    slots[i].Count = Mathf.Max(1, amount);
                    SlotChanged?.Invoke(i, slots[i]);
                    SlotsChanged?.Invoke();
                    return true;
                }
            }

            if (overflowPolicy == OverflowPolicy.ReplaceSelected)
            {
                slots[selectedIndex].Definition = definition;
                slots[selectedIndex].Count = Mathf.Max(1, amount);
                SlotChanged?.Invoke(selectedIndex, slots[selectedIndex]);
                SlotsChanged?.Invoke();
                return true;
            }

            // FIFO 방식은 구조만 준비
            return false;
        }

        public bool UseSelected(ItemUseContext context)
        {
            Debug.Log("UseSelected() 호출됨");
            if (slots == null || slots.Length == 0)
            {
                return false;
            }

            ItemStack stack = slots[selectedIndex];
            if (stack.IsEmpty)
            {
                return false;
            }

            if (stack.Definition is ConsumableItemDefinition consumable)
            {
                if (consumable.Effect == null)
                {
                    return false;
                }

                bool executed = consumable.Effect.TryExecute(context);
                if (executed)
                {
                    stack.Count -= 1;
                    if (stack.Count <= 0)
                    {
                        stack.Clear();
                    }

                    slots[selectedIndex] = stack;
                    SlotChanged?.Invoke(selectedIndex, stack);
                    SlotsChanged?.Invoke();
                }

                return executed;
            }

            // 스탯/패시브 효과는 추후 구현
            return false;
        }
    }
}
