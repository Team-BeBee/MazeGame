using UnityEngine;
using MazeGame.Items.Core;

namespace MazeGame.Items.Data
{
    // 공용 아이템 정의
    public abstract class ItemDefinition : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemCategory category;
        [SerializeField] private ItemUseType useType = ItemUseType.Instant;

        [Header("스택 설정")]
        [SerializeField] private bool isStackable;
        [SerializeField] private int maxStack = 1;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public ItemCategory Category => category;
        public ItemUseType UseType => useType;
        public bool IsStackable => isStackable;
        public int MaxStack => Mathf.Max(1, maxStack);
    }
}
